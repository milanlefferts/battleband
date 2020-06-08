using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBeatSizeChange : MonoBehaviour
{
    public float sizeModifier;
    private Vector3 sizeOriginal;
    private Image image;
    private bool beat;
    private Coroutine reset;


    void Start()
    {
        image = GetComponent<Image>();
        sizeOriginal = transform.localScale;
        reset = null;
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
        if (beat)
        {
            if (reset != null)
                StopCoroutine(reset);
            reset = StartCoroutine(Reset());
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                beat = true;
                image.transform.localScale = image.transform.localScale * sizeModifier;

                if (reset != null)
                    StopCoroutine(reset);
                reset = StartCoroutine(Reset());
            }
        }
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.1f);
        beat = false;
        image.transform.localScale = sizeOriginal;
    }
}