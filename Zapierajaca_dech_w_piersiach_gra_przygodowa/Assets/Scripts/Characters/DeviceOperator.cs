using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    public float radius = 1.5f;

    void Update()
    {
        if(Input.GetButtonDown("Fire3"))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius); // zwraca liczbe obiektów w sferze.
            foreach(Collider hitColider in hitColliders)
            {
                Vector3 direction = hitColider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction) > 0.5f)
                    if (hitColider.GetComponent<InteractOperator>() != null)
                    hitColider.GetComponent<InteractOperator>().Operate();
            }
        }
    }
}

