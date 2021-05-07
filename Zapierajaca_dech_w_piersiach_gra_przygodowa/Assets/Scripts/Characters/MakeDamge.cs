using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeDamge : MonoBehaviour
{
    public float radius = 1.5f;
    [SerializeField] GameObject shieldMarker;

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
                    AbstractCharacter enemy = hitCollider.GetComponent<AbstractCharacter>();
                    ShieldCollisionBehaviour shield = hitCollider.GetComponent<ShieldCollisionBehaviour>();
                    if (enemy)
                    {
                        enemy.getHit(10);
                    }
                    else if (shield)
                    {
                        Vector3 playerShowder = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
                        Ray ray = new Ray(playerShowder, direction);
                        RaycastHit hit;
                        if (Physics.SphereCast(ray, 0.75f, out hit))
                        {
                            var collInfo = new CollisionInfo { Hit = hit};
                            shield.ShieldCollisionEnter(collInfo);
                        }
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