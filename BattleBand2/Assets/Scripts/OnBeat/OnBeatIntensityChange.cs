using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBeatIntensityChange : MonoBehaviour {

    private Light sLight;
    private float baseIntensity;
    public float intensityMultiplier;
    private void Start()
    {
        sLight = GetComponent<Light>();
        baseIntensity = sLight.intensity;
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
        StartCoroutine(ChangeIntensity());

    }

    private IEnumerator ChangeIntensity()
    {
        sLight.intensity = baseIntensity * intensityMultiplier;
        yield return new WaitForSeconds(0.1f);
        sLight.intensity = baseIntensity;

    }
}