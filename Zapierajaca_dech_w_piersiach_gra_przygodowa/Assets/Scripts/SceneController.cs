using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SceneController : MonoBehaviour
{
    private BoxCollider _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<AbstractCharacter>())
        {
            other.GetComponent<AbstractCharacter>().die();
        }
    }
}
