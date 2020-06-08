using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBeatColorChange : MonoBehaviour {

    private Light slight;
    private Image sImage;
    private bool isLight;

    private bool paused;

    private void Start()
    {
        if (GetComponent<Light>() != null)
        {
            slight = GetComponent<Light>();
            isLight = true;
        }
        else
        {
            sImage = GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        EventManager.BeatEvent += ReactionToBeat;
        EventManager.PauseGameEvent += PauseGame;
    }

    private void OnDisable()
    {
        EventManager.BeatEvent -= ReactionToBeat;
        EventManager.PauseGameEvent -= PauseGame;
    }

    private void PauseGame()
    {
        if (paused)
        {
            paused = false;
            EventManager.BeatEvent += ReactionToBeat;
        }
        else
        {
            paused = true;
            EventManager.BeatEvent -= ReactionToBeat;
        }
    }

    private void ReactionToBeat()
    {
        if (isLight)
        {
            slight.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
        } else
        {
            sImage.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
        }
    }
}