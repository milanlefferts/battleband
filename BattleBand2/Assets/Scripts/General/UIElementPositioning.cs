using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementPositioning : MonoBehaviour {
    public Transform UIElementTransform;
    private void Update()
    {
        Vector3 wantedPos = Camera.main.WorldToScreenPoint(UIElementTransform.position);
        transform.position = new Vector3(wantedPos.x, wantedPos.y, transform.position.z);
    }
}