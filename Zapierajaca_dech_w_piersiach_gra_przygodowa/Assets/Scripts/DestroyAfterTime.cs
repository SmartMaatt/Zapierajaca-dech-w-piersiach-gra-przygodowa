using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeToDestroy;

    void Start()
    {
        StartCoroutine(DestroyElement(timeToDestroy));
    }

   private IEnumerator DestroyElement(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this);
    }
}
