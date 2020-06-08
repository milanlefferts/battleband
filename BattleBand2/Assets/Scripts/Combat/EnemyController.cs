using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public CharacterEnemy[] rooftop01, rooftop02, rooftop03, rooftop04;
    private CharacterEnemy[][] chars;
    private int currentPhase;

    private void OnEnable()
    {
        EventManager.SpawnNextEnemySetEvent += TriggerNextPhase;
    }

    private void OnDisable()
    {
        EventManager.SpawnNextEnemySetEvent -= TriggerNextPhase;
    }

    private void Start()
    {
        currentPhase = 0;
        chars = new CharacterEnemy[][] { rooftop01, rooftop02, rooftop03, rooftop04 };
    }

    private void TriggerNextPhase(int phase)
    {
        if (phase > currentPhase)
        {
            currentPhase += 1;
            Trigger(currentPhase);
        }
    }

    private void Trigger(int phase)
    {
        CharacterEnemy[] tempChars = chars[phase - 1];

        foreach (CharacterEnemy obj in tempChars)
        {
            obj.gameObject.SetActive(true);
        }
        EventManager.Instance.EnemyAppear(tempChars);
    }
}
