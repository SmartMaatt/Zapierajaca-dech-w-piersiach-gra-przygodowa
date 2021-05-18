using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipeWeapon : MonoBehaviour
{
    public List<EquippedItem> equippedItems;

    private bool isEquiped;
    private Animator _animator;
    private AudioManager _audioManager;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _audioManager = GetComponent<AudioManager>();
    }

    public bool Operate()
    {
        isEquiped = !isEquiped;
        if(isEquiped)
        {
            //pick up each item
            foreach (EquippedItem equippedItem in equippedItems)
            {
                equippedItem.Item.transform.parent = equippedItem.PickUpParent.transform;
                equippedItem.Item.transform.localPosition = equippedItem.pickUpPosition;
                equippedItem.Item.transform.localEulerAngles = equippedItem.pickUpRotation;
            }

            _animator.SetBool("WeaponEquipped", true);
            _audioManager.Play("SwordEquip");
        }
        else
        {
            //put away each item
            foreach (EquippedItem equippedItem in equippedItems)
            {
                equippedItem.Item.transform.parent = equippedItem.PutAwayParent.transform;
                equippedItem.Item.transform.localPosition = equippedItem.putAwayPosition;
                equippedItem.Item.transform.localEulerAngles = equippedItem.putAwayRotation;
            }

            _audioManager.Play("SwordUnequip");
            _animator.SetBool("WeaponEquipped", false);
        }

        return isEquiped;
    }

    [System.Serializable]
    public class EquippedItem
    {
        public GameObject Item;
        public GameObject PickUpParent;
        public GameObject PutAwayParent;
        public Vector3 pickUpPosition;
        public Vector3 pickUpRotation;
        public Vector3 putAwayPosition;
        public Vector3 putAwayRotation;
    }
}
