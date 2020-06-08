using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {

    public float secondsUntilDestruction;

	void Start () {
        StartCoroutine(DestroyThis());
	}

    private IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(secondsUntilDestruction);
        Destroy(gameObject);
    }
}
