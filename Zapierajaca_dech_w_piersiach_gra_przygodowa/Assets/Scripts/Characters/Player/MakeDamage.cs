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
    [SerializeField] private float attackStrength;
    [SerializeField] GameObject shieldMarker;
    [Header("UI")]
    [SerializeField] UIBar swordUI;
    [SerializeField] UIBar shieldUI;
    [SerializeField] UIBar magicSwordUI;
    [SerializeField] GameObject NoCristalError;
    [Header("Swords")]
    [SerializeField] GameObject normalSword;
    [SerializeField] GameObject magicSword;
    [SerializeField] private float magicSpellTime;
    [SerializeField] private float magicDamageBonus;
    [SerializeField] private float magicRadiusBonus;

    private Animator _animator;
    private PlayerManager _playerManager;
    private bool _canAttack;
    private bool _canBlock;
    private bool _canMagic;
    private bool _isAttacting;
    private bool _isBlocking;
    private bool _isMagic;
    private bool _isEquiped;
    private float _blockStamina;

    private float _magicRadius = 0.0f;
    private float _magicAttack = 0.0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerManager = GetComponent<PlayerManager>();

        _canAttack = true;
        _canBlock = true;
        _canMagic = true;
        _isAttacting = false;
        _isBlocking = false;
        _isMagic = false;
        _isEquiped = false;
        _blockStamina = shieldBlockMaxTime;
        _magicRadius = 0.0f;
        _magicAttack = 0.0f;

        swordUI.setUpBar(100);
        shieldUI.setUpBar((int)shieldBlockMaxTime * 100);
        magicSwordUI.setUpBar((int)magicSpellTime * 100);
        NoCristalError.SetActive(false);
    }
    void Update()
    {
        Attack();
        Block();
        MagicAttack();

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipeWeapon weapon = GetComponent<EquipeWeapon>();
            if (weapon != null)
            {
                _isEquiped = weapon.Operate();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.Save.Save(1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Managers.Save.Load();
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack && _isEquiped)
        {
            if (_animator.GetBool("WeaponEquipped"))
            {
                _isAttacting = true;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius + _magicRadius);
                foreach (Collider hitCollider in hitColliders)
                {
                    Vector3 direction = hitCollider.transform.position - transform.position;
                    if (Vector3.Dot(transform.forward, direction) > 0.5f)
                    {
                        AbstractCharacter enemy = hitCollider.GetComponent<AbstractCharacter>();
                        ShieldCollisionBehaviour shield = hitCollider.GetComponent<ShieldCollisionBehaviour>();

                        if (enemy)
                        {
                            enemy.getHit((int)(attackStrength + _magicAttack));
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
                    }
                }
                _animator.SetTrigger("Attacking");
            }
            StartCoroutine(attackCooldownCor());
            StartCoroutine(attackUIChange());
        }
    }

    private void Block()
    {
        if (_canBlock)
        {
            if (Input.GetMouseButton(1) && _blockStamina > 0.0f && _isEquiped)
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

                    shieldUI.setBarValue((int)(_blockStamina * 100));
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

                shieldUI.setBarValue((int)(_blockStamina * 100));
            }
        }
    }

    private void MagicAttack()
    {
        if (Input.GetKeyDown(KeyCode.G) && _canMagic && _isEquiped)
        {
            if (Managers.Inventory.equippedSpecial == "Power Crystal")
            {
                _canMagic = false;
                normalSword.SetActive(false);
                magicSword.SetActive(true);

                _magicAttack = magicDamageBonus;
                _magicRadius = magicRadiusBonus;

                StartCoroutine(MagicColDownCor());
                Managers.Inventory.InventoryView.transform.Find("Power CrystalSPECIAL_slot").gameObject.GetComponent<EquipButtonClick>().Drop(false);
            }
            else
            {
                StartCoroutine(errorPopup());
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

    public bool isEquiped()
    {
        return _isEquiped;
    }

    public void addAttackStrength(float _attackStrength)
    {
        attackStrength += _attackStrength;
    }

    private IEnumerator errorPopup()
    {
        NoCristalError.SetActive(true);
        yield return new WaitForSeconds(2);
        NoCristalError.SetActive(false);
    }

    private IEnumerator attackCooldownCor()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1f);
        _isAttacting = false;
        yield return new WaitForSeconds(attackCooldown - 1f);
        _canAttack = true;
    }

    private IEnumerator attackUIChange()
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / attackCooldown;
            swordUI.setBarValue((int)(elapsedTime*100));
            yield return new WaitForEndOfFrame();
        }
        swordUI.setBarValue(100);
    }

    private IEnumerator blockingCooldownCor()
    {
        _canBlock = false;
        yield return new WaitForSeconds(1f);
        _isBlocking = false;
        yield return new WaitForSeconds(blockCooldown - 1f);
        _canBlock = true;
    }

    private IEnumerator MagicColDownCor()
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / magicSpellTime;
            magicSwordUI.setBarValue((int)(elapsedTime * 100 * magicSpellTime));
            yield return new WaitForEndOfFrame();
        }

        magicSwordUI.setBarValue((int)magicSpellTime * 100);
        _canMagic = true;
        normalSword.SetActive(true);
        magicSword.SetActive(false);

        _magicAttack = 0.0f;
        _magicRadius = 0.0f;
    }
}