using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using General.ControllerInput;

public class NoteEmpty : MonoBehaviour {

    // Assigned components
    public Conductor conductor;

    // Base data
    private RectTransform rectTransform;
    public RectTransform spawnPos, pathPos, targetPos;
    public float beatOfThisNote;
    public bool lastNote;
    //private Animator anim;
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

    //private new Collider2D collider;

    private void OnEnable()
    {
        EventManager.RhythmGameStopEvent += DestroyNoteImmediate;
        EventManager.NoteUpdateEvent += UpdateNote;

    }

    private void OnDisable()
    {
        EventManager.RhythmGameStopEvent -= DestroyNoteImmediate;
        EventManager.NoteUpdateEvent -= UpdateNote;

    }

    private void UpdateNote()
    {
        if (paused)
        {
            //beatOfThisNote = Conductor.Instance.notes[noteIndex];
            paused = false;
        } else
        {
            paused = true;
        }
    }

    private void Start ()
    {
        //collider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (paused)
        {
            return;
        }

        rectTransform.localPosition = Vector2.Lerp(
                                                    spawnPos.localPosition,
                                                    pathPos.localPosition, //pathPos
                                                    ((conductor.beatsShownInAdvance - (beatOfThisNote - conductor.songPosInBeats)) / conductor.beatsShownInAdvance) / 2f // / 2f
        );
        /*
        if (rectTransform.localPosition.y > targetPos.localPosition.y && !isBeingDestroyed)
        {
            isBeingDestroyed = true;
            Destroy(this.gameObject);
        }*/
    }

    public void DestroyNoteImmediate()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Conductor")
        {
            Destroy(this.gameObject);
        }
    }
}