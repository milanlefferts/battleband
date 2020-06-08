using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarFlashColors : MonoBehaviour {

    private Image sImage;
    private Color32 purple;
	// Use this for initialization
	void Start () {
        purple = new Color32(114, 78, 133, 255);
        sImage = GetComponent<Image>();
        StartCoroutine(ColorShifter());
	}

    private IEnumerator ColorShifter()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            sImage.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            sImage.color = purple;
            //sImage.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
        }
    }
}
