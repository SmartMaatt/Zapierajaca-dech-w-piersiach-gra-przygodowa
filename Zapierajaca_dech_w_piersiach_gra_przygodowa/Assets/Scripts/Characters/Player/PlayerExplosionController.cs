using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionController : MonoBehaviour
{
    [SerializeField] GameObject RigidbodyPrefab;

    void Update()
    {
        transform.position = RigidbodyPrefab.transform.position;
    }
}
