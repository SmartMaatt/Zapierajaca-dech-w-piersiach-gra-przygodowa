using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbstractCharacter))]
public class WaterDrown : MonoBehaviour
{
    [SerializeField] LayerMask Water;
    [SerializeField] float radius;
    [SerializeField] int waterDamage;

    private AbstractCharacter _characterController;

    private void Start()
    {
        _characterController = GetComponent<AbstractCharacter>();
    }

    private void Update()
    {
        if(Physics.CheckSphere(transform.position + new Vector3(0,2,0), radius, Water))
        {
            _characterController.getHit(waterDamage);
        }
    }
}
