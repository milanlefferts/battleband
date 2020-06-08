using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using General.ControllerInput;
using UnityEngine.UI;

// Tracks the beat and timing, handles songPosition variable and spawning of notes
public class Conductor : MonoBehaviour {

    // Allows remote access for unique instance
    public static Conductor Instance
    {
        get
        {
            return instance;
        }
    }
    private static Conductor instance;

    private bool isPlayingSong;
    // Beats
    public float bpm; // beats per minute
    public float secPerBeat; // duration of a beat
    public float offset; // 0.170 or 0.2
    public float dspTimeSong; // accurate time of the audio
    public float lastBeat; // moving reference point of the beat
    public float lastOffbeat;

    public bool beatPressWindow; // can a note be hit?
    public float beatLingerTime; // leeway in hitting notes correctly
    Coroutine beatLinger;

    public float songPosition; // accurate position of the song
    public float songPosInBeats; // accurate current beat number

    public int nextIndex = 0;
    public float[] notes; // position-in-beats of every note
    public int beatsShownInAdvance;

    public GameObject note, noteSmall, noteEmptyBeat, noteEmptyOffbeat;
    private AudioSource audioSource;

    public bool rhythmGameActive;
    [SerializeField]
    private bool rhythmGameStopping;

    // Hitting Notes
    private Note currentNote;
    [SerializeField]
    private bool noteCanBeHit;
    //private KeyCode lastInput;

    public bool beatSpawned;

    private bool songStarted;

    public CombatController rhythmGameController;
    public RectTransform noteTarget, noteSpawnLocation, notePath, noteParent;

    public Image turnIndicator;
    public Sprite[] turnIndicators;

    private Coroutine fading;
    private bool paused;

    [Header("Beat Pulse")]
    public Color startPulse, jamPulse, abilityPulse;
    public GameObject beatPulse;
    public bool preBeatTime, postBeatTime;
    private GameObject currentBeatPulse;
    //private Color incomingBeatPulseColor;

    private void OnEnable()
    {
        SceneController.FadeInMusicEvent += FadeinSong;
        SceneController.FadeOutMusicEvent += FadeoutSong;
        EventManager.DampenMusicEvent += DampenMusic;

        EventManager.RhythmGameStartEvent += RhythmGameStart;
        EventManager.RhythmGameStopEvent += RhythmGameEnd;

        EventManager.BeaterHitEvent += AbilityUsed;

        EventManager.EndGameEvent += StopMusic;

        EventManager.LoadSongEvent += LoadSong;

        EventManager.PauseGameEvent += UpdateNotes;

        EventManager.AbilityUsedEvent += SetBeatPulseColor;
    }

    private void OnDisable()
    {
        SceneController.FadeInMusicEvent -= FadeinSong;
        SceneController.FadeOutMusicEvent -= FadeoutSong;
        EventManager.DampenMusicEvent -= DampenMusic;

        EventManager.RhythmGameStartEvent -= RhythmGameStart;
        EventManager.RhythmGameStopEvent -= RhythmGameEnd;

        EventManager.BeaterHitEvent -= AbilityUsed;

        EventManager.EndGameEvent -= StopMusic;

        EventManager.LoadSongEvent -= LoadSong;

        EventManager.PauseGameEvent -= UpdateNotes;

        EventManager.AbilityUsedEvent -= SetBeatPulseColor;

    }

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.25f;
        //bpm = 115; // soothsayer 151, necrodancer 115, 136 youtube
        //secPerBeat = 60f / bpm;
        offset = 0f; // 0.17
        beatsShownInAdvance = 2;
        beatLingerTime = 0.75f;
    }

    private void Start()
    {
        //incomingBeatPulseColor = startPulse;
        //SongManager.Instance.PlayRandomSong();
        //SongManager.Instance.PlaySong("Necro"); // Sloan, Kalax, Morse, Necro        
    }

    private void Update()
    {
        if (!isPlayingSong)
        {
            return;
        }
        // Sets song position based on actual song time
        songPosition = (float)(AudioSettings.dspTime - dspTimeSong) * audioSource.pitch - offset;
        songPosInBeats = songPosition / secPerBeat;

        // Only spawn notes after a Rhythm event has been triggered
        if (rhythmGameActive && !rhythmGameStopping &&  !CombatController.Instance.paused)
        {
            // if there are notes left in the song, check if corresponding note-spawning beat has been reached
            if (nextIndex < notes.Length && notes[nextIndex] < songPosInBeats + beatsShownInAdvance)
            {
                GameObject tempNote = null;

               if (notes[nextIndex] == Mathf.Floor(notes[nextIndex]))
                {
                    tempNote = Instantiate(note, noteSpawnLocation.localPosition, Quaternion.identity);
                    beatSpawned = true;
                }
                else
                {
                    tempNote = Instantiate(noteSmall, noteSpawnLocation.localPosition, Quaternion.identity);
                    beatSpawned = true;
                }

                tempNote.transform.SetParent(noteParent, true);
                tempNote.GetComponent<Note>().conductor = this;
                tempNote.GetComponent<Note>().pathPos = notePath;
                tempNote.GetComponent<Note>().targetPos = noteTarget;
                tempNote.GetComponent<Note>().spawnPos = noteSpawnLocation;
                tempNote.GetComponent<Note>().noteIndex = nextIndex;
                tempNote.GetComponent<Note>().beatOfThisNote = notes[nextIndex];

                // Check if the final note has been reached
                if (notes[nextIndex] == notes[notes.Length - 1])
                {
                    tempNote.GetComponent<Note>().lastNote = true; // set note to final
                    rhythmGameStopping = true;
                    StartCoroutine(StoppingRhythmGame());
                }
                nextIndex++;
            } 
        }

        //if (songPosition < lastBeat - secPerBeat)

        // Allow player leeway in hitting a bit early
        if (songPosition < lastBeat + (secPerBeat * beatLingerTime))
        {
            if (!beatPressWindowClosed)
            {
                beatPressWindow = true;
                //SetPreBeat();
            }
        }

        if (songPosition > lastBeat + secPerBeat - (secPerBeat * beatLingerTime) && songPosition < lastBeat + secPerBeat)
        {
            preBeatTime = true;
            postBeatTime = false;
        }

        // Play a beat, set lastBeat
        if (songPosition > lastBeat + secPerBeat)
        {
            abilityUsed = false;
            SpawnBeatPulse();
            EventManager.Instance.Beat();
            SpawnEmptyNote();
            lastBeat += secPerBeat;
            preBeat = (lastBeat + secPerBeat);

            if (beatLinger != null)
                StopCoroutine(beatLinger);
            beatLinger = StartCoroutine(BeatLinger());
            preBeatTime = false;
            postBeatTime = true;
        }

        // Play a offbeat
        if (songPosition > lastOffbeat + (secPerBeat / 2f))
        {
            SpawnEmptyNoteOffbeat();
            lastOffbeat += secPerBeat;
        }

        if (rhythmGameActive && (Input.anyKeyDown || CombatController.Instance.controllerInput.currentDirection != Dpad.None))
        {
            // Correct entry
            if (noteCanBeHit && currentNote != null 
                && (Input.GetKeyDown(currentNote.key1) ||currentNote.dir1 == CombatController.Instance.controllerInput.currentDirection))
            {
                currentNote.DestroyNoteImmediate();
                currentNote = null;
                CombatController.Instance.controllerInput.currentDirection = Dpad.None;
                EventManager.Instance.BeaterHit();
            }

            // Jam on beat
            else if (beatPressWindow && currentNote == null 
                     && Input.GetKeyDown(CombatController.Instance.controllerInput.buttonDown))
            {
                // nothing happens
            }

            // Missed beat or note
            else
            {
                EventManager.Instance.BeaterMiss();
            }
        }

        if (!audioSource.isPlaying && !paused && CombatController.Instance != null && !CombatController.Instance.tutorial)
        {
            EventManager.Instance.EndGame(false);
        }
    }
    float preBeat;
    private void SetPreBeat()
    {
        if (preBeat == lastBeat)
        {
            preBeatTime = true;
            postBeatTime = false;
        }
    }




    private void SetBeatPulseColor(string ability)
    {
        Color tempColor;
        if (ability == "Jam")
        {
            tempColor = jamPulse;
        } else
        {
            tempColor = abilityPulse;
        }

        if (preBeatTime)
        {
            //incomingBeatPulseColor = tempColor;
        } else if (postBeatTime && currentBeatPulse != null)
        {
            currentBeatPulse.GetComponentInChildren<BeatPulse>().SetColor(tempColor);
        }
    }

    private void SpawnBeatPulse()
    {
        if (CombatController.Instance != null && CombatController.Instance.combat)
        {
            //currentBeatPulse = Instantiate(beatPulse);
            //currentBeatPulse.GetComponentInChildren<BeatPulse>().SetColor(incomingBeatPulseColor);
            //incomingBeatPulseColor = startPulse;
        }
    }

    private void SpawnEmptyNote()
    {
        if (rhythmGameActive && !rhythmGameStopping && !CombatController.Instance.paused && !notes.Contains(Mathf.RoundToInt(songPosInBeats) + 2))
        {
    
            //if (!beatSpawned)
            //{
            GameObject tempNote = Instantiate(noteEmptyBeat, noteSpawnLocation.localPosition, Quaternion.identity);
            tempNote.transform.SetParent(noteParent, true);
            tempNote.GetComponent<NoteEmpty>().conductor = this;
            tempNote.GetComponent<NoteEmpty>().pathPos = notePath;
            tempNote.GetComponent<NoteEmpty>().targetPos = noteTarget;
            tempNote.GetComponent<NoteEmpty>().spawnPos = noteSpawnLocation;
            tempNote.GetComponent<NoteEmpty>().beatOfThisNote = Mathf.RoundToInt(songPosInBeats) + 2;
            //}
            //beatSpawned = false;
        }
    }

    private void SpawnEmptyNoteOffbeat()
    {
        if (rhythmGameActive && !rhythmGameStopping && !CombatController.Instance.paused && !notes.Contains(Mathf.RoundToInt(songPosInBeats) + 2.5f))
        {
            //if (!beatSpawned)
            //{
            GameObject tempNote = Instantiate(noteEmptyOffbeat, noteSpawnLocation.localPosition, Quaternion.identity);
            tempNote.transform.SetParent(noteParent, true);
            tempNote.GetComponent<NoteEmpty>().conductor = this;
            tempNote.GetComponent<NoteEmpty>().pathPos = notePath;
            tempNote.GetComponent<NoteEmpty>().targetPos = noteTarget;
            tempNote.GetComponent<NoteEmpty>().spawnPos = noteSpawnLocation;
            tempNote.GetComponent<NoteEmpty>().beatOfThisNote = Mathf.RoundToInt(songPosInBeats) + 2.5f;
            //}
            //beatSpawned = false;
        }
    }


    // ###############
    // # Rhythm Game #
    // ###############
    // Grants an extra window to correctly hit a beat
    public IEnumerator BeatLinger()
    {
        beatPressWindow = true;
        beatPressWindowClosed = false;

        yield return new WaitForSeconds(secPerBeat * beatLingerTime);
        CloseBeatPressWindow();
    }

    private bool beatPressWindowClosed;
    private void CloseBeatPressWindow()
    {
        beatPressWindowClosed = true;
        beatPressWindow = false;
    }
    public bool abilityUsed;
    private void AbilityUsed()
    {
        abilityUsed = true;
    }

    private void RhythmGameStart()
    {
        if (CombatController.Instance.playerTurn)
        {
            turnIndicator.sprite = turnIndicators[0];
        }
        else
        {
            turnIndicator.sprite = turnIndicators[1];
        }

        if (!rhythmGameActive)
        {
            EventManager.Instance.AbilityIndicatorStatus(false); // turn off UI elements

            float[] tempNotes;

            int rand = Random.Range(0, 3);
            // TODO: Make each attack have its own pattern
            switch (rand)
            {
                case 0:
                    tempNotes = new float[] { 3f, 4f, 5f, 5.5f, 7f, 8f, 9f, 9.5f };
                    break;
                case 1:
                    tempNotes = new float[] { 3f, 3.5f, 4f, 5f, 6f, 6.5f, 7f, 8f };
                    break;
                case 2:
                    tempNotes = new float[] { 3f, 4f, 5f, 5.5f, 6f, 7f, 7.5f, 8f };
                    break;
                case 3:
                    tempNotes = new float[] { 3f, 3.5f, 4f, 5f, 5.5f, 6f, 7f, 7.5f };
                    break;
                default:
                    tempNotes = new float[] { 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f };
                    break;
            }
            //tempNotes = new float[] { 3f, 6f, 8f, 10f}; // test


            if (CombatController.Instance.tutorial)
            {
                tempNotes = new float[2000];
                for (int i = 0; i < 2000; i++)
                {
                    tempNotes[i] = i + 4;
                }
            }

            // Convert notes to most recent Beat
            notesTemp = tempNotes;
            notes = tempNotes.Select(a => a + (int)Mathf.Floor(songPosInBeats)).ToArray(); // lastBeat?

            // Reset RhythmGame
            nextIndex = 0;
            rhythmGameStopping = false;

            // Start RhythmGame
            rhythmGameActive = true;
        }
    }

    float[] notesTemp;
    private void UpdateNotes()
    {
        if (rhythmGameActive)
        {
            notes = notesTemp.Select(a => a + (int)Mathf.Floor(songPosInBeats)).ToArray();
            EventManager.Instance.NoteUpdate();
        }
    }

    private void RhythmGameEnd()
    {
        rhythmGameActive = false;
        rhythmGameStopping = false;

        // Reset Solo targets
        EventManager.Instance.SetEnemyAsSoloTarget(false); 
        EventManager.Instance.SetFriendlyAsSoloTarget(false);

        EventManager.Instance.AbilityIndicatorStatus(true);
    }

    private IEnumerator StoppingRhythmGame()
    {
        yield return new WaitUntil(() => !rhythmGameStopping);
        rhythmGameActive = false;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Note")
        {
            noteCanBeHit = true;
            currentNote = other.GetComponent<Note>();
        }
        else
        {
            noteCanBeHit = false;
            currentNote = null;
        }
    }

    public void StopMusic(bool useless)
    {
        StartCoroutine(FadeoutMusic());
    }

    private IEnumerator FadeoutMusic()
    {
        while (audioSource.volume > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            audioSource.volume -= 0.1f;
        }
    }

    public void StartSong()
    {
        if (!songStarted)
        {
            audioSource.Play();
            dspTimeSong = (float)AudioSettings.dspTime;
            isPlayingSong = true;
        }
    }

    private void LoadSong(AudioClip newSong, int newBpm)
    {
        isPlayingSong = false;

        bpm = newBpm;
        secPerBeat = 60f / bpm;

        audioSource.clip = newSong;
        songStarted = false;
        StartSong();

        /* TODO: remove if old timer deleted;
        if (CombatController.Instance != null && !CombatController.Instance.tutorial)
            EventManager.Instance.NewSong();
            */
    }

    public void FadeoutSong()
    {
        paused = true;
        if (fading != null)
        {
            StopCoroutine(fading);
        }
        fading = StartCoroutine(Fadeout());
    }
    private IEnumerator Fadeout()
    {
        float dur = 1f;
        while (audioSource.volume > 0.25f)
        {
            audioSource.volume -= (dur / 20f);
            yield return null;
        }
        if (audioSource.volume == 0.25f)
        {
            audioSource.Pause();
        }
    }

    public void FadeinSong()
    {
        paused = false;
        if (fading != null)
        {
            StopCoroutine(fading);
        }
        fading = StartCoroutine(Fadein());
    }
    private IEnumerator Fadein()
    {
        if (!audioSource.isPlaying)
            audioSource.UnPause();
        float dur = 1f;
        while (audioSource.volume >= 0.0f)
        {
            audioSource.volume += (dur / 20f);
            yield return null;
            if (audioSource.volume >= 1.0f)
                break;
        }
    }

    public void DampenMusic()
    {
        paused = false;
        if (fading != null)
        {
            StopCoroutine(fading);
        }
        fading = StartCoroutine(Dampen());
    }

    private IEnumerator Dampen()
    {
        float dur = 1f;
        while (audioSource.volume > 0.25f)
        {
            audioSource.volume -= (dur / 20f);
            yield return null;
        }
    }

}