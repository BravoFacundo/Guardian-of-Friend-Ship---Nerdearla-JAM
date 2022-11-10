using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterRandomDelay : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(nameof(DestroyAfter), Random.Range(3f,10f));
    }

    IEnumerator DestroyAfter(float delay) 
    {
        yield return new WaitForSeconds(delay);
        Destroy(transform.parent.gameObject);
    }
}
