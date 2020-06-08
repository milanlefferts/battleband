using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicNote : MonoBehaviour {

    public Sprite[] notes;
    public Color color;
    public RectTransform parent;

    void Start ()
    {
        GetComponent<Image>().sprite = notes[Random.Range(0, notes.Length - 1)];
        GetComponent<Image>().SetNativeSize();
        //GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
        GetComponent<Image>().color = color;

        transform.localPosition = (Vector3)Random.insideUnitCircle * 75;
        parent.transform.localScale = parent.transform.localScale * Random.Range(0.5f, 1f);
    }

    public void DestroyThis()
    {
        Destroy(parent.gameObject);
        //this.gameObject.SetActive(false);
    }
}