using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBeatMaterialChange : MonoBehaviour
{

    private Material material;
    public Color basicColor;
    public Color halfColor;

    public Color beatColor;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
            
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

        material.color = halfColor;
        yield return new WaitForSeconds(0.05f);
        material.color = beatColor;
        yield return new WaitForSeconds(0.05f);
        material.color = halfColor;
        yield return new WaitForSeconds(0.05f);
        material.color = basicColor;
    }
}
