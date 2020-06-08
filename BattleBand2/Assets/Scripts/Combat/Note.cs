using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using General.ControllerInput;

public class Note : MonoBehaviour {

    // Assigned components
    public Conductor conductor;

    // Base data
    private RectTransform rectTransform;
    public RectTransform spawnPos, pathPos, targetPos;
    public float beatOfThisNote;
    public bool lastNote;
    private Animator anim;
    public int noteIndex;

    // Turn
    public Image turnIndicator;
    public Sprite[] turnIndicators;

    bool missed;
    private Coroutine destroying;
    bool isBeingDestroyed;

    // Direction
    public Sprite[] directionSprites;
    public Image direction1;
    private KeyCode[] keyCodes;
    private Dpad[] dpadCodes;
    public KeyCode key1;
    public Dpad dir1;
    public int firstDirection;
    public bool paused;
    bool hit;
    private new Collider2D collider;

    private void OnEnable()
    {
        EventManager.RhythmGameStopEvent += DestroyOnHit;

        EventManager.NoteUpdateEvent += UpdateNote;
    }

    private void OnDisable()
    {
        EventManager.RhythmGameStopEvent -= DestroyOnHit;

        EventManager.NoteUpdateEvent -= UpdateNote;
    }

    private void UpdateNote()
    {
        if (paused)
        {
            beatOfThisNote = Conductor.Instance.notes[noteIndex];
            paused = false;
        } else
        {
            paused = true;
        }
    }

    private void Start ()
    {
        collider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
        
        keyCodes = new KeyCode[] { CombatController.Instance.controllerInput.dpadUp, CombatController.Instance.controllerInput.dpadRight, CombatController.Instance.controllerInput.dpadDown, CombatController.Instance.controllerInput.dpadLeft };
        dpadCodes = new Dpad[] { Dpad.Up, Dpad.Right, Dpad.Down, Dpad.Left};

        GenerateDirection();
        SetTurnIndicator();
    }

    private void Update()
    {
        if (paused || hit)
        {
            return;
        }

        rectTransform.localPosition = Vector2.Lerp(
                                            spawnPos.localPosition,
                                            pathPos.localPosition, //pathPos
                                            ((conductor.beatsShownInAdvance - (beatOfThisNote - conductor.songPosInBeats)) / conductor.beatsShownInAdvance) / 2f);

        if (rectTransform.localPosition.y > targetPos.localPosition.y && !isBeingDestroyed)
        {
            isBeingDestroyed = true;
            StartCoroutine("DestroyNote");
        }
    }

    private void GenerateDirection()
    {
        // Generate first random direction
        firstDirection = Random.Range(0, 4);
        direction1.sprite = directionSprites[firstDirection];
        key1 = keyCodes[firstDirection];
        dir1 = dpadCodes[firstDirection];
    }

    private void SetTurnIndicator()
    {
        if (CombatController.Instance.playerTurn)
        {
            turnIndicator.sprite = turnIndicators[0];
        }
        else
        {
            turnIndicator.sprite = turnIndicators[1];
        }
    }

    // Miss
    private IEnumerator DestroyNote()
    {
        yield return new WaitForSeconds (0.1f);
        anim.SetTrigger("NoteMiss");
        if (collider != null && collider.isActiveAndEnabled)
            collider.enabled = false;
        EventManager.Instance.BeaterMiss();
        if (!missed) {
            missed = true;
            EventManager.Instance.NoteMiss();
        }
    }

    // Hit
    public void DestroyNoteImmediate()
    {
        hit = true;
        if (collider != null && collider.isActiveAndEnabled)
            collider.enabled = false;
        anim.SetTrigger("NoteHit");
        StopCoroutine("DestroyNote");
        EventManager.Instance.NoteHit();
    }

    public void DestroyOnHit()
    {
        hit = true;
        if (collider.isActiveAndEnabled && collider != null)
            collider.enabled = false;
        StopCoroutine("DestroyNote");
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (lastNote)
        {
            EventManager.Instance.RhythmGameStop();
        }
    }

}