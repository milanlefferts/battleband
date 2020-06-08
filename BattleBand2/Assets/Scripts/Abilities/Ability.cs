using UnityEngine;
using System;

[Serializable]
public class Ability {
    public string name;
    public AbilityType type;
    public int cost;
    public string targets;   //EnemyTarget, EnemyAoE, Self, FriendlyAoE, RhythmEnemy, RhythmFriendly
    public delegate void AbilityEffect(Character target, Character user);
    public AbilityEffect effect, weakEffect, strongEffect;
    public GameObject visualEffect;
    public Sprite icon;
    public AudioClip sound;
    public bool hasAttackMode;

    // Ability Use
    public Ability(string aName, AbilityType aType, string aTargets, bool aAttackMode, AbilityEffect aEffect, GameObject aVisualEffect, Sprite aIcon, AudioClip aSound)
    {
        name = aName;
        type = aType;
        targets = aTargets;
        hasAttackMode = aAttackMode;
        effect = aEffect;
        visualEffect = aVisualEffect;
        icon = aIcon;
        sound = aSound;
    }

    // Enemy Rhythm Ability
    public Ability(string aName, AbilityType aType, string aTargets, bool aAttackMode, AbilityEffect aWeakEffect, AbilityEffect aStrongEffect, GameObject aVisualEffect, Sprite aIcon, AudioClip aSound)
    {
        name = aName;
        type = aType;
        targets = aTargets;
        hasAttackMode = aAttackMode;
        weakEffect = aWeakEffect;
        strongEffect = aStrongEffect;
        visualEffect = aVisualEffect;
        icon = aIcon;
        sound = aSound;
    }

    public string GetName()
    {
        return (string.Format("{0}", name));
    }

}