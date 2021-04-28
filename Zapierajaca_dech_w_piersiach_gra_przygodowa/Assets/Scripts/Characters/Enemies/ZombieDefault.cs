using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAI))]
public class ZombieDefault : AbstractCharacter
{
    Animator _animator;
    EnemyAI _enemyInteligence;
    [Header("Special - Zombie")]
    public int attackPoints;
    public float attackRadius;
    public float dieAwaitTime;
    public float hitCoolDownTime;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyInteligence = GetComponent<EnemyAI>();
    }

    public override void getHit(int damage)
    {
        if (!_immortal)
        {
            StartCoroutine(hitCoolDown(hitCoolDownTime));
            _animator.SetTrigger("isHit");
            changeHealth(-damage);
        }
    }

    public override void die()
    {
        _animator.SetBool("isDead", true);
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

    public override void attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 direction = hitCollider.transform.position - transform.position;
            if (Vector3.Dot(transform.forward, direction) > 0.5f)
            {
                AbstractCharacter target = hitCollider.GetComponent<PlayerManager>();
                if (target != null)
                {
                    target.getHit(attackPoints);
                }
            }
        }
    }
}
