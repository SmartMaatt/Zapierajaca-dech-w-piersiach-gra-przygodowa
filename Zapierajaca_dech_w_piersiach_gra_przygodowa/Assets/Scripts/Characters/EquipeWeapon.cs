using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipeWeapon : MonoBehaviour
{
    [SerializeField] GameObject RightHand;
    [SerializeField] GameObject LeftHand;
    [SerializeField] GameObject Weapon;
    [SerializeField] GameObject Shield;
    [SerializeField] GameObject Pelvis;
    [SerializeField] GameObject Back;

    public Vector3 weaponPickUpPosition;
    public Vector3 weaponPickUpRotation;
    public Vector3 weaponPutAwayPosition;
    public Vector3 weaponPutAwayRotation;

    public Vector3 shieldPickUpPosition;
    public Vector3 shieldPickUpRotation;
    public Vector3 shieldPutAwayPosition;
    public Vector3 shieldPutAwayRotation;

    private bool isEquiped;
    private Animator _animator;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        Debug.Log(weaponPickUpPosition.x);
    }

    public void Operate()
    {
        isEquiped = !isEquiped;
        if(isEquiped)
        {
            Weapon.transform.parent = RightHand.transform;
            Weapon.transform.localPosition = weaponPickUpPosition;
            Weapon.transform.localEulerAngles = weaponPickUpRotation;
            
            Shield.transform.parent = LeftHand.transform;
            Shield.transform.localPosition = shieldPickUpPosition;
            Shield.transform.localEulerAngles = shieldPickUpRotation;

            _animator.SetBool("WeaponEquipped", true);
        }
        else
        {
            Weapon.transform.parent = Pelvis.transform;
            Weapon.transform.localPosition = weaponPutAwayPosition;
            Weapon.transform.localEulerAngles = weaponPutAwayRotation;

            Shield.transform.parent = Back.transform;
            Shield.transform.localPosition = shieldPutAwayPosition;
            Shield.transform.localEulerAngles = shieldPutAwayRotation;

            _animator.SetBool("WeaponEquipped", false);
        }
    }
}
