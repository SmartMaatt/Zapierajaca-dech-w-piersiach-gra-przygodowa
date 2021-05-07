using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireAreaLightShother : MonoBehaviour
{
    public bool startReducing = false;

    private void Update()
    {
        if (startReducing)
        {
            GetComponent<Light>().range -= 0.001f;
            GetComponent<Light>().intensity -= 0.001f;
        }
    }
}
