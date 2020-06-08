using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharacter : MonoBehaviour {

    Animator anim;

    private void OnEnable()
    {
        EventManager.AbilityUsedEvent += PlayAbilityAnimation;

    }

    private void OnDisable()
    {
        EventManager.AbilityUsedEvent -= PlayAbilityAnimation;

    }

    void Start () {
        anim = GetComponent<Animator>();
	}

    private void PlayAbilityAnimation(string ability)
    {
        switch (ability)
        {
            case "Jam":
                anim.SetTrigger("Jam");
                break;
            default:
                break;
        }
    }
}
