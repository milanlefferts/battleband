using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBoss : CharacterEnemy
{
    //private int phase = 1;
    /*
    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);

        if (phase < 2) // Check if boss is already in Phase 2
        {
            if (healthCurrent < Mathf.RoundToInt(healthMax * 0.25f))
            //if (healthCurrent < healthMax) // Testing
            {
                EventManager.Instance.RhythmGameStop();
                EventManager.Instance.Dialogue("rooftop_bosslow");

                phase++;
            }
        }
    }*/
}
