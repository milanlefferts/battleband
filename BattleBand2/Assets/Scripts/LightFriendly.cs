using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFriendly : MonoBehaviour {

    private Animator anim;

    public Light light1, light2;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.EnemyTurnIncomingEvent += Flash;
        if (anim != null)
            anim.SetBool("Flash", false);
    }

    private void OnDisable()
    {
        EventManager.EnemyTurnIncomingEvent -= Flash;
    }

    private void CloseLight()
    {
        light1.enabled = false;
        light2.enabled = false;
    }


    private void OpenLight()
    {
        light1.enabled = true;
        light2.enabled = true;
    }

    private void Reset()
    {
            anim.SetBool("Flash", false);
    }

    private void Flash()
    {
            anim.SetBool("Flash", true);
    }
}