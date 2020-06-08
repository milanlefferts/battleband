using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTarget : MonoBehaviour {

    private Animator anim;
    public GameObject turnIndicator;
    private bool showHits;

    private void OnEnable()
    {
        EventManager.RhythmGameStartEvent += RhythmGameStart;
       
        EventManager.RhythmGameStopEvent += RhythmGameEnd;
        EventManager.NoteHitEvent += NoteHit;
    }

    private void OnDisable()
    {
        EventManager.RhythmGameStartEvent -= RhythmGameStart;
        EventManager.RhythmGameStopEvent -= RhythmGameEnd;
            EventManager.NoteHitEvent -= NoteHit;
            ResetNoteTarget();
    }

    private void Start()
    {
        if (this.name != "NotePath")
        {
            showHits = true;
        }
            anim = GetComponent<Animator>();
    }

    private void RhythmGameStart()
    {
        anim.SetBool("RhythmGameActive", true);
    }

    private void NoteHit()
    {
        if (showHits)
            anim.SetTrigger("NoteHit");
    }

    private void RhythmGameEnd()
    {
        anim.SetBool("RhythmGameActive", false);
    }

    private void ResetNoteTarget()
    {
        if (showHits)
            anim.ResetTrigger("NoteHit");
    }

    private void EnableTurnIndicator()
    {
        turnIndicator.SetActive(true);
    }

    private void DisableTurnIndicator()
    {
        turnIndicator.SetActive(false);
    }
}