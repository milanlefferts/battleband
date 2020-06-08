using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        SceneController.StopLoadingEvent += StopLoading;
    }

    private void OnDisable()
    {
        SceneController.StopLoadingEvent -= StopLoading;
    }

    private void StopLoading()
    {
        anim.SetBool("StopLoading", true);
    }
}