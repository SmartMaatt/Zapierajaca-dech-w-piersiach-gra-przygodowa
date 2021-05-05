using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(TargetHeadAim))]
public class Mage : AbstractCharacter
{
    Animator _animator;
    EnemyAI _enemyInteligence;
    TargetHeadAim _headTarget;
    GameObject _player;

    [Header("Special - Mage")]
    public float coolDownFireball;
    public int FireballDamage;
    public float coolDownHand;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject fireballPrefabNonAttack;
    [SerializeField] GameObject fireburstPrefab;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyInteligence = GetComponent<EnemyAI>();
        _headTarget = GetComponent<TargetHeadAim>();
        _player = _enemyInteligence.getPlayerObject();
    }

    public override void getHit(int damage)
    {
        if (!_immortal)
        {
            _animator.SetTrigger("isHit");
            changeHealth(-damage);
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
        yield return new WaitForSeconds(dieAwaitTime);
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

    public override void attack()
    {
        if (Vector2.Distance(convertToFlat(transform.position), convertToFlat(_player.transform.position)) > 1.5f)
        {
            _animator.SetInteger("AttackType", 1);
            _enemyInteligence.timeBetweenAttacks = coolDownFireball;
            StartCoroutine(fireballShoot(1.2f));
        }
        else
        {
            _animator.SetInteger("AttackType", 0);
            _enemyInteligence.timeBetweenAttacks = coolDownHand;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                Vector3 direction = hitCollider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction) > 0.5f)
                {
                    AbstractCharacter target = hitCollider.GetComponent<PlayerManager>();
                    if (target != null)
                    {
                        target.getHit(_damage);
                        Instantiate(fireburstPrefab, sightPosition(target.transform.position) + target.transform.forward * 0.2f, Quaternion.identity);
                    }
                }
            }
        }
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

    IEnumerator fireballShoot(float time)
    {
        GameObject _fireBall = Instantiate(fireballPrefabNonAttack, leftHand.transform.position + leftHand.transform.forward * 0.5f, Quaternion.identity);
        _fireBall.transform.SetParent(leftHand.transform);

        yield return new WaitForSeconds(time);

        Destroy(_fireBall);
        _fireBall = Instantiate(fireballPrefab, sightPosition(transform.position), Quaternion.identity);
        _fireBall.transform.GetChild(0).transform.gameObject.GetComponent<ProjectileCollisionBehaviour>().damage = FireballDamage;
        _fireBall.GetComponent<EffectSettings>().MoveVector = _player.transform.GetChild(1).position - transform.position;
    }
}
