using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffect
{
    None,
    DefenseDown,
    DefenseUp,
    AttackUp,
    AttackDown
};

public enum AbilityType
{
    Heal,
    AttackSingleTarget,
    AttackAoE,
    Debuff,
    Buff
}

public class AbilityManager : MonoBehaviour {

    public Dictionary<string, Ability> abilityDB = new Dictionary<string, Ability>();
    public Dictionary<string, GameObject> summonDB = new Dictionary<string, GameObject>();

    public Dictionary<StatusEffect, Sprite> statusEffectDB = new Dictionary<StatusEffect, Sprite>();

    // Allows remote access of unique instance
    public static AbilityManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static AbilityManager instance;

    public GameObject[] abilityEffects, summonEffects;
    public Sprite[] abilityIcons;
    public AudioClip[] abilityAudio;
    public Sprite[] statusEffects;

    //public AudioClip switchAudio;
    public GameObject switchEffect;

    private Ability[] singerAbilities, drummerAbilities, guitaristAbilities;

    private void OnEnable()
    {
        EventManager.SwitchAbilitiesEvent += SwitchAbility;
        EventManager.SummonEvent += SummonCreature;
    }

    private void OnDisable()
    {
        EventManager.SwitchAbilitiesEvent -= SwitchAbility;
        EventManager.SummonEvent -= SummonCreature;

    }

    private void Awake()
    {
        instance = this;

        FillAbilityDatabase();
        AddStatusEffects();
        singerAbilities = new Ability[] { abilityDB["Soul Flare"], abilityDB["Tinnitus"], abilityDB["Tinnitus"], abilityDB["Tinnitus"] };
        drummerAbilities = new Ability[] { abilityDB["Thunderous Solo"], abilityDB["Sonic Boom"], abilityDB["Tinnitus"], abilityDB["Tinnitus"] };
        guitaristAbilities = new Ability[] { abilityDB["Soothing Melody"], abilityDB["Crowd Pleaser"], abilityDB["Tinnitus"], abilityDB["Tinnitus"] };
    }

    private void AddAbility(Ability ability)
    {
        abilityDB.Add(ability.GetName(), ability);
    }

    private void FillAbilityDatabase()
    {
        // Primary
        AddAbility(new Ability("Soothing Melody", AbilityType.Heal, "FriendlyAll", false, Heal, abilityEffects[4], abilityIcons[4], abilityAudio[4]));
        AddAbility(new Ability("Soul Flare", AbilityType.AttackSingleTarget, "EnemyTarget", true, BurningSensation, abilityEffects[0], abilityIcons[0], abilityAudio[5]));
        AddAbility(new Ability("Thunderous Solo", AbilityType.AttackAoE, "EnemyRhythm", true, ThunderousSolo, abilityEffects[1], abilityIcons[1], abilityAudio[1]));

        // Secondary 
        AddAbility(new Ability("Crowd Pleaser", AbilityType.Buff, "FriendlyAll", false, DefenseBoostAll, abilityEffects[3], abilityIcons[3], abilityAudio[3]));
        AddAbility(new Ability("Tinnitus", AbilityType.Debuff, "EnemyAll", false, DefenseLowerAll, abilityEffects[0], abilityIcons[5], abilityAudio[0]));
        AddAbility(new Ability("Sonic Boom", AbilityType.AttackSingleTarget, "EnemyTarget", true, SonicBoom, abilityEffects[1], abilityIcons[2], abilityAudio[1]));

        // Special
        AddAbility(new Ability("Black Hole", AbilityType.AttackAoE, "EnemyRhythm", true, BlackHole, abilityEffects[5], null, abilityAudio[3]));
        summonDB.Add("Black Hole", summonEffects[0]);

        // Rooftop
        AddAbility(new Ability("Selfie", AbilityType.AttackAoE, "EnemyRandom", false, Selfie, abilityEffects[6], null, abilityAudio[6]));
        AddAbility(new Ability("Leech", AbilityType.AttackSingleTarget, "EnemyRandom", false, Leech, abilityEffects[9], null, abilityAudio[9]));

        AddAbility(new Ability("Bite", AbilityType.AttackSingleTarget, "EnemyRandom", false, Bite, abilityEffects[7], null, abilityAudio[7]));
        AddAbility(new Ability("Puppy Love", AbilityType.Buff, "FriendlyRandom", false, PuppyLove, abilityEffects[8], null, abilityAudio[8]));

        AddAbility(new Ability("Void Gaze", AbilityType.AttackAoE, "EnemyRhythm", true, DamageRhythmSimpleWeak, DamageRhythmSimpleStrong, abilityEffects[5], null, abilityAudio[10]));
        AddAbility(new Ability("Null Blast", AbilityType.AttackSingleTarget, "EnemyRandom", false, NullBlast, abilityEffects[5], null, abilityAudio[3]));

        //Garage
        AddAbility(new Ability("Void Strike", AbilityType.AttackSingleTarget, "EnemyRandom", false, NullBlast, abilityEffects[5], null, abilityAudio[10]));
        AddAbility(new Ability("Shocking Solo", AbilityType.Heal, "EnemyRhythm", true, TutorialRhythm, abilityEffects[1], abilityIcons[1], abilityAudio[1]));
    }

    private void AddStatusEffects()
    {
        statusEffectDB.Add(StatusEffect.AttackUp, statusEffects[0]);
        statusEffectDB.Add(StatusEffect.AttackDown, statusEffects[1]);
        statusEffectDB.Add(StatusEffect.DefenseUp, statusEffects[2]);
        statusEffectDB.Add(StatusEffect.DefenseDown, statusEffects[3]);
    }

    public Ability LookupAbility(int character, int ability)
    {
        Ability temp = null;
        switch (character)
        {
            case 0:
                temp = singerAbilities[ability];
                break;
            case 1:
                temp = drummerAbilities[ability];
                break;
            case 2:
                temp = guitaristAbilities[ability];
                break;
            default:
                print("Incorrect character int in LookupAbility");
                break;
        }
        return temp;
    }

    private void SummonCreature(string creature)
    {
        Instantiate(summonDB[creature]);
    }

    // ##########
    // # Switch #
    // ##########

    private void SwitchAbility()
    {
        if (CombatController.Instance != null)
        {
            if (CombatController.Instance.combat)
                Instantiate(switchEffect);
        } else {
            Instantiate(switchEffect);
        }
    }

    // #################
    // # Basic effects #
    // ################# 

    // # General #
    private int RandomizeValue(int val)
    {
        int randomDamage = Mathf.RoundToInt(Random.Range(val * 0.75f, val * 1.25f));
        return randomDamage;
    }

    // # Damage #
    private void DamageTarget(Character target, Character user, int damage)
    {
        target.Defend(user, damage);
    }

    private void DamageEnemyAll(Character user, int damage)
    {
        foreach (CharacterEnemy character in CombatController.Instance.enemyCharacters)
        {
            if (character.isAlive)
                character.Defend(user, damage);
        }
    }

    private void DamageFriendlyAll(Character user, int damage)
    {
        foreach (CharacterFriendly character in CombatController.Instance.friendlyCharacters)
        {
            if (character.isAlive)
                character.Defend(user, damage);
        }
    }

    // # Healing #
    private void HealTarget(Character target, Character user, int heal)
    {
        target.Heal(user, heal);
    }

    private void HealFriendlyAll(Character user, int heal)
    {
        foreach (CharacterFriendly character in CombatController.Instance.friendlyCharacters)
        {
            if (character.isAlive)
                character.Heal(user, heal);
        }
    }

    private void HealEnemyAll(Character user, int heal)
    {
        foreach (CharacterEnemy character in CombatController.Instance.enemyCharacters)
        {
            if (character.isAlive)
                character.Heal(user, heal);
        }
    }

    // # Status #
    private void StatusGainTarget(Character target, Character user, StatusEffect status, int duration)
    {
        target.StatusGain(user, status, duration);
    }

    private void StatusRemoveTarget(Character target, Character user, StatusEffect status)
    {
        target.StatusRemove(user, status);
    }

    private void StatusGainFriendlies(Character user, StatusEffect status, int duration)
    {
        foreach (CharacterFriendly character in CombatController.Instance.friendlyCharacters)
        {
            if (character.isAlive)
                character.StatusGain(user, status, duration);
        }
    }

    private void StatusRemoveFriendlies(Character user, StatusEffect status)
    {
        foreach (CharacterFriendly character in CombatController.Instance.friendlyCharacters)
        {
            if (character.isAlive)
                character.StatusRemove(user, status);
        }
    }

    private void StatusGainEnemies(Character user, StatusEffect status, int duration)
    {
        foreach (CharacterEnemy character in CombatController.Instance.enemyCharacters)
        {
            if (character.isAlive)
                character.StatusGain(user, status, duration);
        }
    }

    private void StatusRemoveEnemies(Character user, StatusEffect status)
    {
        foreach (CharacterEnemy character in CombatController.Instance.enemyCharacters)
        {
            if (character.isAlive)
                character.StatusRemove(user, status);
        }
    }

// #############
// # Abilities #
// ############# 

    // ############
    // # Friendly #
    private void BurningSensation(Character target, Character user) // EnemyTarget
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(70) + user.attack) * (user.attackBoost + user.attackComboModifier))) );
        StatusGainFriendlies(user, StatusEffect.AttackUp, 16);
    }

    private void SonicBoom(Character target, Character user) // EnemyTarget
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(65) + user.attack) * (user.attackBoost + user.attackComboModifier))));
        StatusGainTarget(target, user, StatusEffect.AttackDown, 16);
    }

    private void ThunderousSolo(Character target, Character user) // EnemyTarget
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(10) + (user.attack * 0.5f)) * (user.attackBoost + user.attackComboModifier))) );
    }

    private void BlackHole(Character target, Character user) // RhythmFriendly
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(15) + (user.attack * 0.5f)) * (user.attackBoost + user.attackComboModifier))));
    }

    private void Heal(Character target, Character user) // FriendlyAll
    {
        HealFriendlyAll(user, Mathf.RoundToInt(RandomizeValue(Mathf.RoundToInt(user.healthMax * 0.25f)) * (user.attackBoost + user.attackComboModifier)));
    }

    private void AttackBoostAll(Character target, Character user) // Self
    {
        StatusGainFriendlies(user, StatusEffect.AttackUp, 32);
    }

    private void DefenseBoostAll(Character target, Character user) // Self
    {
        StatusGainFriendlies(user, StatusEffect.DefenseUp, 32);
    }

    private void DefenseLowerAll(Character target, Character user) // Self
    {
        StatusGainEnemies(user, StatusEffect.DefenseDown, 32);
    }

    // ##########
    // # Garage #
    private void NullBlast(Character target, Character user)
    {
        DamageTarget(target, user, 50);
    }

    private void TutorialRhythm(Character target, Character user)
    {
        DamageTarget(target, user, 15);
    }

    // ###########
    // # Rooftop #
    private void Bite(Character target, Character user)
    {
        DamageTarget(target, user, RandomizeValue(25));
        StatusGainTarget(target, user, StatusEffect.DefenseDown, 32);
    }

    private void PuppyLove(Character target, Character user)
    {
        StatusGainTarget(target, user, StatusEffect.DefenseUp, 32);
    }

    private void Selfie(Character target, Character user)
    {
        DamageFriendlyAll(user, Mathf.RoundToInt(((RandomizeValue(20) + user.attack) * (user.attackBoost + user.attackComboModifier))));
    }

    private void Leech(Character target, Character user)
    {
        int leechVal = RandomizeValue(30);
        DamageTarget(target, user, leechVal);
        HealTarget(user, user, Mathf.RoundToInt(leechVal / 2f));
    }

    private void DamageRhythmSimpleWeak(Character target, Character user)
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(5) + (user.attack * 0.5f)) * (user.attackBoost + user.attackComboModifier)) * 0.5f));
    }

    private void DamageRhythmSimpleStrong(Character target, Character user)
    {
        DamageTarget(target, user, Mathf.RoundToInt(((RandomizeValue(5) + (user.attack * 0.5f)) * (user.attackBoost + user.attackComboModifier))));
    }
}