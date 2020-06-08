using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour {

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(1.5f);
        EventManager.BeatEvent -= PlayAudioClip;
        Destroy(this.gameObject);

    }

    public void DestroyThis()
    {
        StartCoroutine(DestroyDelay());
    }

    private void CameraShake()
    {
        ScreenShake.Instance.ShakeScreen(0.1f, 0.2f);
    }

    private void OnDisable()
    {
        EventManager.BeatEvent += PlayAudioClip;
    }

    private void OnEnable()
    {
        EventManager.BeatEvent -= PlayAudioClip;
    }

    private void PlayAudioClip()
    {
        if (audioSource != null)
            audioSource.Play();
        EventManager.BeatEvent -= PlayAudioClip;
    }

}