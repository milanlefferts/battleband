using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToBeat : MonoBehaviour {

    private Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}

    private void OnEnable()
    {
        EventManager.BeatEvent += ReactionToBeat;
    }

    private void OnDisable()
    {
        EventManager.BeatEvent -= ReactionToBeat;
    }

    private void ReactionToBeat()
    {
        if (anim.GetBool("Beat"))
        {
            anim.SetBool("Beat", false);
        } else
        {
            if (gameObject.activeInHierarchy)
                anim.SetBool("Beat", true);
        }
    }

    private void ResetBeat()
    {
        anim.SetBool("Beat", false);
        
    }
}