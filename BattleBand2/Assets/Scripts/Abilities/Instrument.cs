using UnityEngine;
using System;

[Serializable]
public class Instrument
{
    public string name;
    //public Ability ability;
    public string ability;
    public string abilityText;
    public string flavorText;   //EnemyTarget, EnemyAoE, Self, FriendlyAoE, RhythmEnemy, RhythmFriendly
    public Sprite icon;
    public string sheet;

    public Instrument()
    {

    }

    public Instrument(string name, string ability, string abilityText, string flavorText, Sprite icon, string sheet)
    {
        this.name = name;
        this.ability = ability;
        this.abilityText = abilityText;
        this.flavorText = flavorText;
        this.icon = icon;
        this.sheet = sheet;
    }
}