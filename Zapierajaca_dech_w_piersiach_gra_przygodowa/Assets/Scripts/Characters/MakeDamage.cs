using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeDamage : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    [Range(1,10)]
    [SerializeField] private float attackCooldown;
    [Range(1,10)]
    [SerializeField] private float blockCooldown;
    [SerializeField] private int shieldArmor;
    [SerializeField] private float shieldBlockMaxTime;
    [SerializeField] GameObject shieldMarker;

    private Animator _animator;
    private PlayerManager _playerManager;
    private bool _canAttack;
    private bool _canBlock;
    private bool _isAttacting;
    private bool _isBlocking;
    private float _blockStamina;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerManager = GetComponent<PlayerManager>();

        _canAttack = true;
        _canBlock = true;
        _isAttacting = false;
        _isBlocking = false;
        _blockStamina = shieldBlockMaxTime;
    }
    void Update()
    {
        Attack();
        Block();

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

    private void Attack()
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
    }

    private void Block()
    {
        if (_canBlock)
        {
            if (Input.GetMouseButton(1) && _blockStamina > 0.0f)
            {
                if (_animator.GetBool("WeaponEquipped"))
                {
                    if (!_isBlocking)
                    {
                        _playerManager.changeArmour(shieldArmor);
                        _playerManager.setBlock(true);
                    }

                    _isBlocking = true;
                    _blockStamina -= Time.deltaTime;
                    _animator.SetBool("isBlocking", true);

                    if (_blockStamina < 0.0f)
                        StartCoroutine(blockingCooldownCor());

                    Debug.Log("Block: " + _blockStamina);
                }
            }

            if (_blockStamina < 0.0f || (!Input.GetMouseButton(1) && _blockStamina < shieldBlockMaxTime))
            {
                if (_animator.GetBool("isBlocking"))
                {
                    _playerManager.changeArmour(-shieldArmor);
                    _playerManager.setBlock(false);
                }

                _isBlocking = false;
                _blockStamina += Time.deltaTime;
                _animator.SetBool("isBlocking", false);

                Debug.Log("Not-block: " + _blockStamina);
            }
        }
    }

    public bool isAttacting()
    {
        return _isAttacting;
    }

    public bool isBlocking()
    {
        return _isBlocking;
    }

    private IEnumerator attackCooldownCor()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1f);
        _isAttacting = false;
        yield return new WaitForSeconds(attackCooldown - 1f);
        _canAttack = true;
    }

    private IEnumerator blockingCooldownCor()
    {
        _canBlock = false;
        yield return new WaitForSeconds(1f);
        _isBlocking = false;
        yield return new WaitForSeconds(blockCooldown - 1f);
        _canBlock = true;
    }
}