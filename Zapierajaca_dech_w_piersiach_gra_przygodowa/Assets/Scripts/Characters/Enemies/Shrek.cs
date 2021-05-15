using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAI))]
public class Shrek : AbstractCharacter
{
    Animator _animator;
    EnemyAI _enemyInteligence;
    [SerializeField] float hitCoolDownTime;
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
        _animator.SetTrigger("isDead");
        Managers.Player.changeMoney(givenMoney);
        Managers.Player.changeExp(givenExp);
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

        while (elampsedTime < 1)
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
                    target.getHit(_damage);

                }
            }
        }
    }

    public override void setStateMachine(int state, int postState, float specialInfo)
    {
        //Patroling
        if (state == 0)
        {
            //Walking
            if (postState == 0)
            {

            }
            //Resting
            else if (postState == 1)
            {
               
            }
        }
        //Chasing
        else if (state == 1)
        {
          
        }
        //Attacking
        else if (state == 2)
        {
            
        }
    }
}
