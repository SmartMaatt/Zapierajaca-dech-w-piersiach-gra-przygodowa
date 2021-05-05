using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFieldControll : MonoBehaviour
{
    public float timeToDestroy;
    [SerializeField] private new Light light;

    void Start()
    {
        StartCoroutine(deleteField(timeToDestroy));
    }

    private IEnumerator deleteField(float timeToDelete)
    {
        float elapsedTime = 0;
        float lightRange = light.range;

        while(elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / timeToDelete;
            if(elapsedTime > 0.5f)
            {
                light.range = Mathf.Lerp(lightRange, 0, (elapsedTime - 0.5f)*2);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
