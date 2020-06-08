using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterEnemy : Character {

    public bool isAttacking;

    public GameObject targetingDirectionObject;
    public Sprite[] targetingDirections;
    public int targetingDirection;

    // Abilities
    public Ability[] abilities;
    private int lastAttack;

    public AudioClip taunt, deathCry;
    public bool playTaunt;
    public int pos;

    // UI
    public TextMeshProUGUI nameText;

    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.NoteHitEvent += NoteHit;
        EventManager.NoteMissEvent += NoteMiss;

        EventManager.SetEnemyAsSoloTargetEvent += SetAsSoloTarget;
        EventManager.RhythmGameStopEvent += StopAttacking;

        EventManager.TargetEnemyToggleEvent += TargetEnemyToggle;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        EventManager.NoteHitEvent -= NoteHit;
        EventManager.NoteMissEvent -= NoteMiss;

        EventManager.SetEnemyAsSoloTargetEvent -= SetAsSoloTarget;

        EventManager.RhythmGameStopEvent -= StopAttacking;

        EventManager.TargetEnemyToggleEvent -= TargetEnemyToggle;
    }

    public override void Start() {
        base.Start();

        isFriendlyCharacter = false;
        healthMax = 250;
        attack = 1;
        defense = 1;

        SetEnemy();
        lastAttack = 0;

        nameText.text = characterName;

        if (playTaunt)
            StartCoroutine(PlayTaunt());
    }

    private void SetEnemy()
    {
        switch (characterName)
        {
            // Garage
            case "The Suit ":
                healthMax = 500;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Void Strike"] };
                break;
            case "Doggy Guard ":
                healthMax = 60;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Void Strike"] };
                break;

            // Rooftop
            case "Fame Leech":
                healthMax = 240;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Selfie"], AbilityManager.Instance.abilityDB["Leech"] };
                break;
            case "Doggy Guard":
                healthMax = 300;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Bite"], AbilityManager.Instance.abilityDB["Puppy Love"] };
                break;
            case "The Suit":
                healthMax = 400;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Null Blast"], AbilityManager.Instance.abilityDB["Void Gaze"] };
                break;

            default:
                characterName = "No-Name Nancy";
                healthMax = 100;
                abilities = new Ability[] { AbilityManager.Instance.abilityDB["Harp"] };
                break;
        }
        healthCurrent = healthMax;
        UpdateHealth();
    }

    private void TargetEnemyToggle()
    {
        if (isAlive)
        {
            if (targetingDirectionObject.activeSelf)
            {
                targetingDirectionObject.SetActive(false);
            }
            else
            {
                targetingDirectionObject.SetActive(true);
            }
        }
    }

    public virtual void SetTargetingDirection(int dir)
    {
        targetingDirection = dir;
        targetingDirectionObject.GetComponent<Image>().sprite = targetingDirections[targetingDirection];
    }

    private void NoteMiss() // Player missed note; received NO damage
    {
        if (isAttacking && isRhythmUser)
        {
            AttackAnimation(null); // less damage anim
        }
        else if (isRhythmTarget) // isSoloTarget
        {
            // Do nothing
        }
    }

    private void NoteHit () // Player hit note; receives damage
    {
        if (isAttacking && isRhythmUser)
        {
            AttackAnimation(null); // more damage anim
        }
        else if (isRhythmTarget) // isSoloTarget
        {
            audioSource.PlayOneShot(CombatController.Instance.currentAbility.sound);
            CombatController.Instance.currentAbility.effect(this, this);
            Instantiate(CombatController.Instance.currentAbility.visualEffect, effectPos.position, Quaternion.identity);
        }
    }

    public void SetAttacking(bool attacking)
    {
        if (attacking)
        {
            isAttacking = true;
            spriteRenderer.material.color = Color.red;
        }
        else
        {
            isAttacking = false;
            spriteRenderer.material.color = Color.white;
        }
    }

    public Ability SelectAttack()
    {
        lastAttack = (lastAttack + 1) > abilities.Length - 1 ? 0 : lastAttack + 1; // Cycle attacks
        //int rand = Random.Range(0, abilities.Length); // Random attacks

        Ability tempAbility = abilities[lastAttack];
        if (tempAbility.hasAttackMode)
        {
            AttackAnimation(tempAbility);
        }
        return (tempAbility);
    }

    public void StopAttacking()
    {
        SetAttacking(false);
    }

    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);
        StartCoroutine(TakeDamageEffect());
    }

    public override void Death()
    {
        base.Death();

        audioSource.PlayOneShot(deathCry);

        targetingDirectionObject.SetActive(false);
        healthbarBase.SetActive(false);

        EventManager.NoteHitEvent -= NoteHit;
        EventManager.NoteMissEvent -= NoteMiss;

        EventManager.RhythmGameStopEvent -= StopAttacking;
    }

    private IEnumerator PlayTaunt()
    {
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(taunt);
    }
}