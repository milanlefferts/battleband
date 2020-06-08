using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using General.Colors;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public string characterName;
    public int healthCurrent, healthMax;
    public StatusEffect status;
    public int statusDuration;
    public int attack, defense, defenseModifier;
    public float attackBoost = 1f;
    public float defensePenalty = 0f;
    public float attackComboModifier, defenseComboModifier;
    public bool isAlive = true;

    [Header("Camera")]
    public Vector3 cameraZoomPosition;

    [Header("Attacking")]
    public bool isBeingAttacked;
    public int attackTimer;

    public bool isRhythmTarget, isRhythmUser;
    public bool isFriendlyCharacter;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Animator anim, animHealthbar;
    public AudioSource audioSource; 

    [Header("Health & Status")]
    public GameObject damageNumber, healthbarBase, spotlight;
    private Coroutine damageNumberRoutine;
    public Image healthbar;
    public GameObject statusObject;
    public Image statusIcon;
    public TextMeshProUGUI statusDurationText;
    public Transform effectPos;
    public GameObject statsObject;

    [Header("Triggers")]
    public bool hasTrigger;
    public float triggerThreshold;
    public int triggerVal;
    private bool triggered;


    public virtual void OnEnable()
    {
        EventManager.BeatEvent += UpdateStatus;
    }

    public virtual void OnDisable()
    {
        EventManager.BeatEvent -= UpdateStatus;
    }

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = spriteRenderer.GetComponent<Animator>();
        animHealthbar = healthbarBase.GetComponent<Animator>();
    }

    // #######################
    // # Character Selection #
    // #######################
    public virtual void CharacterSelect()
    {

        if (spotlight != null)
            spotlight.SetActive(true);
        StartCoroutine(Deselect());
    }

    private IEnumerator Deselect()
    {
        spriteRenderer.material.SetColor("_Color2", Colors.outlineWhite);

        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetColor("_Color2", Colors.outlineBasic);

    }

    public virtual void CharacterDeselect()
    {
        if (spotlight != null)
            spotlight.SetActive(false);
    }

    public virtual void SetAsSoloTarget(bool status)
    {
        isRhythmTarget = status;
    }

    public virtual void SetAsSoloUser(bool status)
    {
        isRhythmUser = status;
    }

    // ###################
    // # Ability Effects #
    // ###################
    public virtual void Defend(Character attacker, int dam)
    {
        int totalDefense = Mathf.RoundToInt((defense + defenseModifier) * (1 + defenseComboModifier));

        int damAdjusted = 0;
        //int preventedDamage = 0;

        if (totalDefense >= dam) // if all damage is blocked
        {
            //preventedDamage = totalDefense;
            damAdjusted = 0;
        }
        else // if not all damage is blocked
        {
            damAdjusted = Mathf.RoundToInt((dam - totalDefense) + (dam * defensePenalty));
            //preventedDamage = dam - damAdjusted;
        }

        //if (preventedDamage < 0)
            //print(string.Format("{0} blocked {1} damage", characterName, preventedDamage));
        TakeDamage(damAdjusted);
    }

    public virtual void Heal(Character healer, int heal)
    {
        healthCurrent += heal;

        if (healthCurrent >= healthMax)
        {
            healthCurrent = healthMax;
            //healthbarBase.SetActive(false);
        }
        else if (healthCurrent > Mathf.RoundToInt(healthMax * 0.25f))
        {
            animHealthbar.SetBool("HealthLow", false);
        }

        UpdateHealth();

        if (damageNumberRoutine != null)
            StopCoroutine(damageNumberRoutine);
        damageNumberRoutine = StartCoroutine(SpawnText(heal + "", Colors.colorGreenLight));

        //print(string.Format("{0} was healed for {1}", characterName, heal));
    }

    // ###########
    // # Status #
    // ##########
    public virtual void StatusGain(Character inflicter, StatusEffect gainedStatus, int duration)
    {
        if (!isAlive)
            return;
        status = gainedStatus;
        statusDuration = duration;
        SetStatus();
        //print(string.Format("{0} gained {1}", characterName, status));
    }

    public virtual void StatusRemove(Character remover, StatusEffect lostStatus)
    {
        if (status == lostStatus)
        {
            status = StatusEffect.None;
            statusDuration = 0;
            SetStatus();
            //print(string.Format("{0}'s {1} status was removed", characterName, status));
        }
    }

    private void SetStatus()
    {
        attackBoost = 1f; // reset attack boost
        defensePenalty = 0f;
        defenseModifier = defense;

        if (status == StatusEffect.None)
        {
            statusObject.SetActive(false);
            //statusIcon.gameObject.SetActive(false);
            statusDurationText.enabled = false;
        }
        else
        {
            statusObject.SetActive(true);
            // set status icon
            //statusIcon.gameObject.SetActive(true);
            statusDurationText.enabled = true;
        }

        switch (status)
        {
            case StatusEffect.AttackUp:
                attackBoost = 1.25f;
                statusIcon.sprite = AbilityManager.Instance.statusEffectDB[status];
                break;
            case StatusEffect.DefenseDown:
                defensePenalty = 0.5f;
                statusIcon.sprite = AbilityManager.Instance.statusEffectDB[status];
                break;
            case StatusEffect.DefenseUp:
                defenseModifier = defense * 5;
                statusIcon.sprite = AbilityManager.Instance.statusEffectDB[status];
                break;
            default:
                break;
        }
        statusIcon.SetNativeSize();

        /*
        if (damageNumberRoutine != null)
            StopCoroutine(damageNumberRoutine);
        damageNumberRoutine = StartCoroutine(SpawnStatusEffectText(status, colorBlue));*/
    }

    private void UpdateStatus()
    {
        if (status != StatusEffect.None)
        {
            statusDuration -= 1;
            statusDurationText.text = statusDuration + "";

            if (statusDuration <= 0)
            {
                StatusRemove(this, status);
            }
        }
    }
    // #############
    // # Attacking #
    // #############

    public virtual void AttackAnimation(Ability currAbility)
    {
        if (currAbility == null)
        {
            PlayAbilityAnimation("Attack");
        }
        else
        {
            if (currAbility.hasAttackMode) // Determine whether an Attack Mode animation must play
            {
                PlayAbilityAnimation("AttackMode");
            }
            else
            {
                PlayAbilityAnimation("Attack");
            }
        }

    }

    public virtual void PlayAbilityAnimation(string ability)
    {
        switch (ability)
        {
            case "Jam":
                anim.SetTrigger("Jam");
                break;
            case "Attack":
                anim.SetTrigger("Attack");
                break;
            case "AttackMode":
                //if (anim.GetBool("AttackMode"))
                anim.SetBool("AttackMode", true);
                EventManager.RhythmGameStopEvent += ResetAttackMode;
                break;
            default:
                break;
        }
    }

    public virtual void ResetAttackMode()
    {
        //if (anim.GetBool("AttackMode"))
        //{
            anim.SetBool("AttackMode", false);
            EventManager.RhythmGameStopEvent -= ResetAttackMode;
        //}
    }


    // #################
    // # Taking Damage #
    // #################
    public virtual void TakeDamage(int dam)
    {
        if (!isAlive)
        {
            return;
        }

        anim.SetTrigger("Damage");
        healthCurrent -= dam;

        if (healthCurrent < healthMax)
        {
            //healthbarBase.SetActive(true);
            if (healthCurrent <= Mathf.RoundToInt(healthMax * 0.25f))
            {
                animHealthbar.SetBool("HealthLow", true);
            }
        }

        UpdateHealth();

        CheckForHealthTrigger();

        if (damageNumberRoutine != null)
            StopCoroutine(damageNumberRoutine);

        damageNumberRoutine = StartCoroutine(SpawnText(dam + "", Color.white));

        if (healthCurrent <= 0) // Character dies :(
        {
            Death();
        }
    }

    public void UpdateHealth()
    {
        if (healthbar != null)
        {
            healthbar.fillAmount = (float)healthCurrent / (float)healthMax;
        }
    }

    public virtual IEnumerator TakeDamageEffect()
    {
        ScreenShake.Instance.ShakeScreen(0.05f, 0.2f);
        spriteRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.color = Color.white;
    }

    public IEnumerator SpawnText(string damNr, Color32 color)
    {
        damageNumber.SetActive(false);
        damageNumber.GetComponent<TextMeshProUGUI>().text = damNr;
        damageNumber.GetComponent<TextMeshProUGUI>().color = color;
        damageNumber.GetComponent<TextMeshProUGUI>().fontSize = 36f;

        damageNumber.SetActive(true);
        yield return new WaitForSeconds(1f);
        damageNumber.SetActive(false);
    }

    public virtual void CheckForHealthTrigger()
    {
        if (hasTrigger && !triggered && healthCurrent <= Mathf.RoundToInt(healthMax * triggerThreshold))
        {
            triggered = true;
            hasTrigger = false;
            EventManager.Instance.SpawnNextEnemySet(triggerVal);
        }
    }


/*
public IEnumerator SpawnStatusEffectText(StatusEffect eff, Color32 color)
{
    string effString = GetStatusName(eff);

    damageNumber.SetActive(false);
    damageNumber.GetComponent<TextMeshProUGUI>().text = effString;
    damageNumber.GetComponent<TextMeshProUGUI>().color = color;
    damageNumber.GetComponent<TextMeshProUGUI>().fontSize = 24f;

    damageNumber.SetActive(true);
    yield return new WaitForSeconds(2f);
    damageNumber.SetActive(false);
}

private string GetStatusName(StatusEffect eff)
{
    string effString = "";
    switch (eff)
    {
        case StatusEffect.None:
            break;
        case StatusEffect.AttackDown:
            effString = "Attack down";
            break;
        case StatusEffect.DefenseDown:
            effString = "Defense down";
            break;
        case StatusEffect.AttackUp:
            effString = "Attack up";
            break;
        case StatusEffect.DefenseUp:
            effString = "Defense up";
            break;
    }
    return effString;
}*/

public virtual void Death()
    {
        anim.SetTrigger("Death");
        isAlive = false;
        //statusIcon.gameObject.SetActive(false);
        //statusDurationText.enabled = false;
        //animHealthbar.SetBool("Dead", true);
        statsObject.SetActive(false);
        EventManager.Instance.Death(this);
    }
}