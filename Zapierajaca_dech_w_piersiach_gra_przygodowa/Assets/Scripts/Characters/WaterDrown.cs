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
    private AudioManager _audioManager;

    private void Start()
    {
        _characterController = GetComponent<AbstractCharacter>();
        _audioManager = GetComponent<AudioManager>();
    }

    private void Update()
    {
        if(Physics.CheckSphere(transform.position + new Vector3(0,2,0), radius, Water) && !_characterController.isDead())
        {
            _audioManager.Play("Drawning");
            _characterController.getHit(waterDamage);
        }
    }
}
