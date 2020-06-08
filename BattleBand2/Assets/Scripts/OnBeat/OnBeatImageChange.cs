using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBeatImageChange : MonoBehaviour
{
    public Sprite small, large;
    private Image image;
    private SpriteRenderer sprite;
    private bool beat;
    private Coroutine reset;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
            sprite = GetComponent<SpriteRenderer>();
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
                if (image == null)
                {
                    sprite.sprite = large;
                } else
                {
                    image.sprite = large;
                    image.SetNativeSize();
                }


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
        if (image == null)
        {
            sprite.sprite = small;
        } else
        {
            image.sprite = small;
            image.SetNativeSize();
        }
    }
}