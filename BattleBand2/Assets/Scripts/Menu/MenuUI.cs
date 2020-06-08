using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

    [Header("Input")]
    public Sprite[] ps4ControllerUI, xboxControllerUI;
    public Sprite[] keyboardUI;
    public RuntimeAnimatorController[] arrowAnimators;
    public Animator[] arrowsUI;

    [Header("Keys Shown")]
    public Image abilityDirection1, abilityDirection2, abilityDirection3, jamIcon;

    private void OnEnable()
    {
        EventManager.SetControllerUIEvent += SetControllerUI;
    }

    private void OnDisable()
    {
        EventManager.SetControllerUIEvent -= SetControllerUI;
    }

    private void SetControllerUI(int val)
    {
        if (val == 2) // PS
        {
            abilityDirection1.sprite = ps4ControllerUI[0];
            abilityDirection2.sprite = ps4ControllerUI[1];
            abilityDirection3.sprite = ps4ControllerUI[2];
            jamIcon.sprite = ps4ControllerUI[3];
            foreach (Animator anim in arrowsUI)
            {
                anim.runtimeAnimatorController = arrowAnimators[2];
            }
            //switchIcon.sprite = ps4ControllerUI[4];
        }
        else if (val == 1) // XBOX
        {
            abilityDirection1.sprite = xboxControllerUI[0];
            abilityDirection2.sprite = xboxControllerUI[1];
            abilityDirection3.sprite = xboxControllerUI[2];
            jamIcon.sprite = xboxControllerUI[3];
            foreach (Animator anim in arrowsUI)
            {
                anim.runtimeAnimatorController = arrowAnimators[1];
            }            //switchIcon.sprite = ps4ControllerUI[4];
        }
        else if (val == 0) // Keyboard
        {
            abilityDirection1.sprite = keyboardUI[0];
            abilityDirection2.sprite = keyboardUI[1];
            abilityDirection3.sprite = keyboardUI[2];
            jamIcon.sprite = keyboardUI[3];
            foreach (Animator anim in arrowsUI)
            {
                anim.runtimeAnimatorController = arrowAnimators[0];
            }            //switchIcon.sprite = keyboardUI[4];
        }
    }
}
