using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Tutorial : CharacterEnemy
{
    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);
        if (healthCurrent <= (healthMax * 0.5f))
        {
            if (Tutorial.Instance.currentPhase < 20)
            {
                EventManager.Instance.TutorialNextPhase();
                EventManager.Instance.RhythmGameStop();

            }
       
        }
    }

    public override void AttackAnimation(Ability currAbility)
    {
        base.AttackAnimation(currAbility);
        EventManager.Instance.TutorialNextPhase();
    }

    public override void SetTargetingDirection(int dir)
    {
        targetingDirectionObject.GetComponent<Image>().sprite = targetingDirections[targetingDirection];
    }

}