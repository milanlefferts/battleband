using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuEventManager : MonoBehaviour
{
    // Allows remote access for unique instance
    public static MenuEventManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static MenuEventManager instance;

    void Awake()
    {
        instance = this;
    }

    // Generic delegate w single parameter
    public delegate void SingleParameterDelegate<T>(T para);

    // Generic delegate w two parameters
    public delegate void TwoParameterDelegate<T, U>(T para1, U para2);

    // Generic delegate w three parameters
    public delegate void ThreeParameterDelegate<T, U, X>(T para1, U para2, X para3);


    // ###########
    // # General #
    // ###########

    public static event SingleParameterDelegate<string> AbilityUsedEvent;
    public void AbilityUsed(string ability)
    {
        AbilityUsedEvent(ability);
    }

    public static event Action BeatEvent;
    public void Beat()
    {
        BeatEvent();
    }

    public static event SingleParameterDelegate<bool> SetControllerUIEvent;
    public void SetControllerUI(bool val)
    {
        SetControllerUIEvent(val);
    }

}