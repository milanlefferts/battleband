using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    private float speed;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.EnemyTurnEvent += StartAnimation;
        EventManager.PlayerTurnEvent += Reset;
        EventManager.SetupCombatEvent += SetSpeed;

        EventManager.BeatEvent += PulseOnBeat;

        EventManager.TutorialPlayerTurnEvent += Reset;

        EventManager.NewSongEvent += SetSpeed;
    }

    private void OnDisable()
    {
        EventManager.EnemyTurnEvent -= StartAnimation;
        EventManager.PlayerTurnEvent -= Reset;
        EventManager.SetupCombatEvent -= SetSpeed;

        EventManager.BeatEvent -= PulseOnBeat;

        EventManager.TutorialPlayerTurnEvent -= Reset;

        EventManager.NewSongEvent -= SetSpeed;
    }

    private void StartAnimation()
    {
        anim.SetBool("EnemyTurn", true);
    }

    private void Reset()
    {
        anim.SetBool("EnemyTurn", false);
    }

    private void SoundEffect()
    {
        audioSource.Play();
    }

    private void SetSpeed()
    {
        speed = 1f * (Conductor.Instance.bpm / 100);
        anim.speed = speed;
    }

    private void PulseOnBeat()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Pulsing"))
        {
            anim.SetBool("Beat", true);
        }
    }

    private void ResetBeat()
    {
        anim.SetBool("Beat", false);
    }

}