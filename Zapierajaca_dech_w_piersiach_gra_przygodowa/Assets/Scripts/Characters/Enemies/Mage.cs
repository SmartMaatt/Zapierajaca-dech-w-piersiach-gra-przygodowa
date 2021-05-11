using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(TargetHeadAim))]
public class Mage : AbstractCharacter
{
    Animator _animator;
    EnemyAI _enemyInteligence;
    TargetHeadAim _headTarget;

    GameObject _player;
    bool _fireBallCasting;
    bool _handAttackCasting;
    bool _areaAttackCasting;
    float _remWalkSpeed, _remRunSpeed;

    [Header("Special - Mage")]
    public float magicPower;
    public float limitOfPowerToBoost;
    [Space]
    public float coolDownFireball;
    public int FireballDamage;
    public float coolDownHand;
    public float timeOfHeal;
    public float explosionRadius;
    public float explosionPower;
    [Space]
    [Header("Fire attacks")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject fireballPrefabNonAttack;
    [SerializeField] GameObject fireburstPrefab;
    [Header("Shields")]
    [SerializeField] GameObject healSpell;
    [SerializeField] GameObject fireShield;
    [Header("Fire area")]
    [SerializeField] GameObject areaSpellPrefab;
    [SerializeField] GameObject fireGlyph;
    [Header("Other")]
    [SerializeField] GameObject soulPrefab;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyInteligence = GetComponent<EnemyAI>();
        _headTarget = GetComponent<TargetHeadAim>();
        _player = FindObjectsOfType<RelativeMovement>()[0].transform.gameObject;
        _fireBallCasting = false;
        _handAttackCasting = false;
        _areaAttackCasting = false;
        _remWalkSpeed = _walkSpeed;
        _remRunSpeed = _runSpeed;
    }

    public override void getHit(int damage)
    {
        if (!_immortal)
        {
            StartCoroutine(isHit());
            changeHealth(-damage);

            //cancelFireball();
            //cancelBurstOnArmor();
        }

        if (_health <= _maxHealth * 0.3 && _health > 0 && Random.Range(1,10) <= 3)
        {
            StartCoroutine(healSpellCast());
        }
    }

    public override void die()
    {
        _animator.SetTrigger("isDead");
        StartCoroutine(dieAwait(dieAwaitTime));
    }

    private IEnumerator dieAwait(float dieAwaitTime)
    {
        _enemyInteligence.enabled = false;
        yield return new WaitForSeconds(1f);
        GetComponent<NavMeshAgent>().enabled = false;

        float elampsedTime = 0.0f;
        float min = transform.position.y;
        float max = min - 1f;

        Instantiate(soulPrefab, transform.position + new Vector3(0,0.5f,0), Quaternion.identity).GetComponent<EffectSettings>().MoveVector = new Vector3(transform.position.x, transform.position.y + 100f, transform.position.z);
        while(elampsedTime < 1)
        {
            elampsedTime += Time.deltaTime / dieAwaitTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(min, max, elampsedTime), transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
    }

    private IEnumerator hitCoolDown(float time)
    {
        _immortal = true;
        yield return new WaitForSeconds(time);
        _immortal = false;
    }

    /*****OVERRIDE*****/

    public override float getWalkSpeed()
    {
        return _walkSpeed;
    }

    public override float getRunSpeed()
    {
        return _runSpeed;
    }

    public override float getAcceleration(bool isChasing)
    {
        if (isChasing)
            return _acceleration * _accelerationChaseBonus;
        else
            return _acceleration;
    }

    public override void setStateMachine(int state, int postState, float specialInfo)
    {
        //Patroling
        if (state == 0)
        {
            _headTarget.changeWeight(0.0f, 0.0f);
            fireHands(false);
        }
        //Chasing
        else if (state == 1)
        {
            _headTarget.changeTargetToPlayer(1.75f);
            _headTarget.changeWeight(1.0f, 1.0f);
            fireHands(true);
        }
        //Attacking
        else if (state == 2)
        {
            _headTarget.changeTargetToPlayer(1.75f);
            _headTarget.changeWeight(1.0f, 1.0f);
            fireHands(true);
        }
    }

  
    public override void attack()
    {
        if (magicPower > limitOfPowerToBoost)
        {
            StartCoroutine(areaSpell());
        }
        else
        {
            if (Vector2.Distance(convertToFlat(transform.position), convertToFlat(_player.transform.position)) > 1.5f)
            {
                StartCoroutine(fireballShoot(1.2f));
            }
            else
            {
                float combatRand = Random.Range(0, 5);
                if (combatRand > 2)
                {
                    StartCoroutine(burstOnArmor());
                }
                else
                {
                    StartCoroutine(Explosion());
                }
            }
        }
    }


    /***SPELLS***/
    IEnumerator areaSpell()
    {
        _areaAttackCasting = true;
        _immortal = true;
        fireShield.SetActive(true);
        magicPower -= limitOfPowerToBoost;

        float rememberAttackRange = _enemyInteligence.attackRange;
        float rememberMagicPower = magicPower;

        _walkSpeed = 0.0f;
        _runSpeed = 0.0f;
        _enemyInteligence.attackRange = 0.0f;

        _animator.SetInteger("AttackType", 2);
        yield return new WaitForSeconds(1f);
        Instantiate(fireGlyph, transform.position, Quaternion.identity);
        GameObject fireArea = Instantiate(areaSpellPrefab, transform.position, Quaternion.Euler(-90, 0, 0));

        if(Vector2.Distance(convertToFlat(transform.position), convertToFlat(_player.transform.position)) < 1.5f)
            StartCoroutine(_player.GetComponent<RelativeMovement>().Explosion(1f, 0.1f, 0.1f, -0.1f, 0.15f));

        yield return new WaitForSeconds(1f);

        float angle = 0.0f;
        GameObject fireball = null;
        Vector3 chestPos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                fireball = Instantiate(fireballPrefab, chestPos, Quaternion.Euler(0, angle, 0));
                fireball.GetComponent<EffectSettings>().MoveVector = chestPos + fireball.transform.forward * 50f;
                angle += 45f;
                yield return new WaitForSeconds(0.1f);
            }
            angle += 15f;
        }

        yield return new WaitForSeconds(1f);
        _enemyInteligence.attackRange = rememberAttackRange;
        fireArea.GetComponent<ParticleSystem>().Stop();
        fireArea.transform.GetChild(1).transform.gameObject.GetComponent<fireAreaLightShother>().startReducing = true;

        yield return new WaitForSeconds(2f);
        Destroy(fireArea);

        yield return new WaitForSeconds(5f);
        _immortal = false;
        _areaAttackCasting = false;
        fireShield.SetActive(false);
        _walkSpeed = _remWalkSpeed;
        _runSpeed = _remRunSpeed;
        _animator.SetTrigger("leaveAreaSpell");
        magicPower = rememberMagicPower;
    }

    private void cancelFireball()
    {
        _fireBallCasting = false;
        int leftBall = leftHand.transform.childCount;
        int rightBall = rightHand.transform.childCount;

        Debug.Log(leftBall + " " + rightBall);

        if (leftBall > 6)
            Destroy(leftHand.transform.GetChild(6).transform.gameObject);

        if (rightBall > 6)
            Destroy(rightHand.transform.GetChild(6).transform.gameObject);
    }

    IEnumerator fireballShoot(float time)
    {
        _fireBallCasting = true;
        if (!_areaAttackCasting)
        {
            _walkSpeed = 0.0f;
            _runSpeed = 0.0f;
        }

        _animator.SetInteger("AttackType", 1);
        _enemyInteligence.timeBetweenAttacks = coolDownFireball;
        magicPower += Random.Range(10f, 20f);

        GameObject _fireBall = Instantiate(fireballPrefabNonAttack, leftHand.transform.position + leftHand.transform.forward * 0.1f, Quaternion.identity);
        _fireBall.transform.SetParent(leftHand.transform);
        GameObject _fireBallRight = Instantiate(fireballPrefabNonAttack, rightHand.transform.position + rightHand.transform.forward * 0.1f, Quaternion.identity);
        _fireBallRight.transform.SetParent(rightHand.transform);

        yield return new WaitForSeconds(time);

        if (_fireBallCasting)
        {
            Destroy(_fireBall);
            Destroy(_fireBallRight);
            _fireBall = Instantiate(fireballPrefab, sightPosition(transform.position), Quaternion.identity);
            _fireBall.transform.GetChild(0).transform.gameObject.GetComponent<ProjectileCollisionBehaviour>().damage = FireballDamage;
            _fireBall.GetComponent<EffectSettings>().MoveVector = _player.transform.GetChild(1).position - transform.position;
            _fireBallCasting = false;
        }
        _walkSpeed = _remWalkSpeed;
        _runSpeed = _remRunSpeed;
    }

    private void cancelBurstOnArmor()
    {
        _handAttackCasting = false;
    }

    IEnumerator burstOnArmor()
    {
        _handAttackCasting = true;
        _animator.SetInteger("AttackType", 0);
        _enemyInteligence.timeBetweenAttacks = coolDownHand;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 direction = hitCollider.transform.position - transform.position;
            if (Vector3.Dot(transform.forward, direction) > 0.5f)
            {
                AbstractCharacter target = hitCollider.GetComponent<PlayerManager>();
                if (target != null && _handAttackCasting)
                {
                    target.getHit(_damage);
                    magicPower += Random.Range(5f, 10f);

                    Instantiate(fireburstPrefab, sightPosition(target.transform.position) + target.transform.forward * 0.2f, Quaternion.identity);
                    yield return new WaitForSeconds(0.3f);
                    Instantiate(fireburstPrefab, sightPosition(target.transform.position) + target.transform.forward * 0.2f, Quaternion.identity);

                }
            }
        }
        _handAttackCasting = false;
    }

    IEnumerator healSpellCast()
    {
        _immortal = true;
        float maxHealth = (float)_maxHealth;
        float attackRange = _enemyInteligence.attackRange;

        _walkSpeed = 0.0f;
        _runSpeed = 0.0f;
        _enemyInteligence.attackRange = 0.0f;

        healSpell.SetActive(true);
        _animator.SetBool("isHealing", true);
        _animator.ResetTrigger("isHit");
        healSpell.GetComponent<EffectSettings>().IsVisible = true;
        _health += (int)Random.Range(maxHealth * 0.3f, maxHealth * 0.7f);

        yield return new WaitForSeconds(timeOfHeal - 2f);

        healSpell.GetComponent<EffectSettings>().IsVisible = false;

        _walkSpeed = _remWalkSpeed;
        _runSpeed = _remRunSpeed;
        _enemyInteligence.attackRange = attackRange;

        yield return new WaitForSeconds(2f);

        _immortal = false;
        _animator.SetBool("isHealing", false);
        healSpell.SetActive(false);
    }

    IEnumerator isHit()
    {
        yield return new WaitForSeconds(0.3f);
        _animator.SetTrigger("isHit");
    }

    /***ADDITIONAL***/
    private void fireHands(bool flame)
    {
        leftHand.transform.GetChild(5).transform.gameObject.SetActive(flame);
        rightHand.transform.GetChild(5).transform.gameObject.SetActive(flame);
    }

    private Vector3 sightPosition(Vector3 vec)
    {
        return new Vector3(vec.x, vec.y + 1.7f, vec.z);
    }

    private Vector2 convertToFlat(Vector3 currentVec)
    {
        return new Vector2(currentVec.x, currentVec.z);
    }

    private IEnumerator Explosion()
    {
        _animator.SetInteger("AttackType", 3);
        magicPower += Random.Range(5f, 10f);

        GameObject _fireBall = Instantiate(fireballPrefabNonAttack, leftHand.transform.position + leftHand.transform.forward * 0.1f, Quaternion.identity);
        _fireBall.transform.SetParent(leftHand.transform);
        GameObject _fireBallRight = Instantiate(fireballPrefabNonAttack, rightHand.transform.position + rightHand.transform.forward * 0.1f, Quaternion.identity);
        _fireBallRight.transform.SetParent(rightHand.transform);

        Instantiate(fireburstPrefab, sightPosition(_player.transform.position) + _player.transform.forward * 0.2f, Quaternion.identity);

        StartCoroutine(_player.GetComponent<RelativeMovement>().Explosion(1f, 0.1f, 0.15f, -0.1f, 0.15f));

        yield return new WaitForSeconds(1f);

        Destroy(_fireBall);
        Destroy(_fireBallRight);
    }
}
