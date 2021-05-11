using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeDamage : MonoBehaviour
{
    public float radius = 1.5f;
    [Range(1,10)]
    public float attackCooldown = 1.5f;
    [SerializeField] GameObject shieldMarker;

    private Animator _animator;
    private bool _canAttack;
    private bool _isAttacting;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _canAttack = true;
        _isAttacting = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            if (_animator.GetBool("WeaponEquipped"))
            {
                _isAttacting = true;
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
                                var collInfo = new CollisionInfo { Hit = hit };
                                shield.ShieldCollisionEnter(collInfo);
                            }
                        }
                        else
                        {
                            Debug.Log("Missed!");
                        }
                    }
                }
                _animator.SetTrigger("Attacking");
            }
            StartCoroutine(attackCooldownCor());
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Weapon equiped!");
            EquipeWeapon weapon = GetComponent<EquipeWeapon>();
            if (weapon != null)
            {
                weapon.Operate();
            }
        }
    }

    public bool isAttacting()
    {
        return _isAttacting;
    }

    private IEnumerator attackCooldownCor()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1f);
        _isAttacting = false;
        yield return new WaitForSeconds(attackCooldown - 1f);
        _canAttack = true;
    }
}