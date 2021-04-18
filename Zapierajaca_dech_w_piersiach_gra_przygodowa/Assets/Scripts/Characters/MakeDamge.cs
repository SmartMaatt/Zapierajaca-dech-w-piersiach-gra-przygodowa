using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeDamge : MonoBehaviour
{
    public float radius = 1.5f;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _animator.GetBool("WeaponEquipped"))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                Vector3 direction = hitCollider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction) > 0.5f)
                {
                    TakeDamage target = hitCollider.GetComponent<TakeDamage>();
                    if (target != null)
                    {
                        target.Operate();
                    }
                    else
                    {
                        Debug.Log("mis");
                    }
                }
            }
            _animator.SetBool("Attacking", true);
            StartCoroutine(changeToIdle());
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("dupa");
            EquipeWeapon weapon = GetComponent<EquipeWeapon>();
            if (weapon != null)
            {
                weapon.Operate();
            }
        }
    }

    private IEnumerator changeToIdle()
    {
        yield return new WaitForSeconds(1.12f);
        _animator.SetBool("Attacking", false);
    }
}