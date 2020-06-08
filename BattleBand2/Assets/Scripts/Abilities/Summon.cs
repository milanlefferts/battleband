using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour {

    public float unsummonTime;

	void Start () {
        EventManager.RhythmGameStopEvent += Unsummon;
    }

    private void OnDestroy()
    {
        EventManager.RhythmGameStopEvent -= Unsummon;
    }

    private void Unsummon()
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("Death");
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(unsummonTime);
        Destroy(gameObject);
    }
}
