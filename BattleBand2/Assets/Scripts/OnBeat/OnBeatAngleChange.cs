using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBeatAngleChange : MonoBehaviour {

    private Light sLight;
    private float baseAngle;
    private float angleMultiplier;

    private void Start()
    {
        sLight = GetComponent<Light>();
        baseAngle = sLight.spotAngle;
        angleMultiplier = 1.5f;
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
        StartCoroutine(ChangeAngle());
    }

    private IEnumerator ChangeAngle()
    {
        sLight.spotAngle = baseAngle * angleMultiplier;
        yield return new WaitForSeconds(0.1f);
        sLight.spotAngle = baseAngle;

    }
}