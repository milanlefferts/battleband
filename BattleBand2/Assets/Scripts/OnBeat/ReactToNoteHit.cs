using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToNoteHit : MonoBehaviour {

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.NoteHitEvent += ReactionToNoteHit;
    }

    private void OnDisable()
    {
        EventManager.NoteHitEvent -= ReactionToNoteHit;
    }

    private void ReactionToNoteHit()
    {
        anim.SetTrigger("Attack");
    }
}