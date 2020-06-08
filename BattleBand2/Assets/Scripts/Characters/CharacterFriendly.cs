using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using General.Colors;

public class CharacterFriendly : Character {

    public CharacterEnemy attacker;
    public int characterNumber;

    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.NoteHitEvent += NoteHit;
        EventManager.NoteMissEvent += NoteMiss;

        EventManager.AbilityUsedEvent += PlayAbilityAnimation;

        EventManager.SetFriendlyAsSoloTargetEvent += SetAsSoloTarget;

    }

    public override void OnDisable()
    {
        base.OnDisable();

        EventManager.NoteHitEvent -= NoteHit;
        EventManager.NoteMissEvent -= NoteMiss;

        EventManager.AbilityUsedEvent -= PlayAbilityAnimation;

        EventManager.SetFriendlyAsSoloTargetEvent -= SetAsSoloTarget;

    }

    public override void Start () {
        base.Start();

        // Attributes
        isFriendlyCharacter = true;
        healthMax = 100;
        healthCurrent = healthMax;
        attack = 1;
        defense = 1;
        UpdateHealth();

    }

    // #############
    // # Selection #
    // #############

    // #############
    // # Attacking #
    // #############

    private void NoteMiss()
    {
        if (isRhythmUser)
        {
            // Do nothing
        }
        if (isRhythmTarget && !CombatController.Instance.playerTurn) // take full damage
        {
            audioSource.PlayOneShot(CombatController.Instance.currentAbility.sound);
            CombatController.Instance.currentAbility.strongEffect(this, this);
            Instantiate(CombatController.Instance.currentAbility.visualEffect, effectPos.position, Quaternion.identity);
        } else
        {
            // Miss
        }
    }

    private void NoteHit()
    {
        if (isRhythmUser)
        {
            anim.SetTrigger("Attack");
        } 
        if (isRhythmTarget)
        {
            audioSource.PlayOneShot(CombatController.Instance.currentAbility.sound);
            Instantiate(CombatController.Instance.currentAbility.visualEffect, effectPos.position, Quaternion.identity);

            if (!CombatController.Instance.playerTurn) // take half damage
            {
                CombatController.Instance.currentAbility.weakEffect(this, this);
                // Block
            }
            else if (CombatController.Instance.playerTurn)
            {
                CombatController.Instance.currentAbility.effect(this, this);
            }
        }

    }

    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);
        StartCoroutine(TakeDamageEffect());
    }

    public override void Death()
    {
        anim.SetTrigger("Death");
        //isAlive = false;
        //statusIcon.gameObject.SetActive(false);
        //statusDurationText.enabled = false;
        //animHealthbar.SetBool("Dead", true);
        EventManager.Instance.Death(this);

        Heal(this, healthMax);
        CombatController.Instance.friendlyDeathCounter += 1;
        CombatController.Instance.CheckForGameEnding();
    }
}