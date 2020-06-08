using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPulse : MonoBehaviour {

    //private Material mat;
    //public Color color;
    public Renderer rend;

    public void SetColor (Color color)
    {
        rend = GetComponent<Renderer>();
        rend.material.SetColor("_TintColor", color);
    }


}
