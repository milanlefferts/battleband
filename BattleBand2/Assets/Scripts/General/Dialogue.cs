using UnityEngine;
using System;

[Serializable]
public class Dialogue
{
    public string id;
    public string[] speakers;
    public string[] speeches;
    public AudioClip[] voices;

    public Dialogue(string dId, string[] dSpeakers, string[] dSpeeches, AudioClip[] dVoices)
    {
        id = dId;
        speakers = dSpeakers;
        speeches = dSpeeches;
        voices = dVoices;
    }
}
