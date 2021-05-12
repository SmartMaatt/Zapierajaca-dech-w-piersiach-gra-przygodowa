using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerManager : AbstractCharacter, IGameManager
{
    public ManagerStatus status { get; private set; }
    public int health { get; private set; }
    public int maxHealth { get; private set; }

    [Header("Special - Player")]
    [SerializeField] CameraFollow cameraScript;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera gracza...");

        health = _health;
        maxHealth = _maxHealth;

        status = ManagerStatus.Started;
    }

    public override void die()
    {
        _animator.SetTrigger("isDead");
        GetComponent<MakeDamage>().enabled = false;
        GetComponent<RelativeMovement>().enabled = false;
        GetComponent<PlayerManager>().enabled = false;
        cameraScript.isDead = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f);
        foreach (Collider hit in colliders)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy)
            {
                Debug.Log(enemy);
                enemy.attackRange = 0;
                enemy.hearRange = 0;
                enemy.sightRange = 0;
            }
        }
    }

    public override void getHit(int damage)
    {
        if (!_immortal)
        {
            int armorBonus = (100 / _armour) * damage;
            changeHealth(-damage + armorBonus);

            if(_health > 0)
            {
                _animator.SetTrigger("isHit");
            }
        }
    }

    public override float getWalkSpeed()
    {
        throw new System.NotImplementedException();
    }

    public override float getRunSpeed()
    {
        throw new System.NotImplementedException();
    }

    public override void attack()
    {
        throw new System.NotImplementedException();
    }

    public override float getAcceleration(bool isChasing)
    {
        throw new System.NotImplementedException();
    }

    public override void setStateMachine(int state, int postState, float specialInfo)
    {
        throw new System.NotImplementedException();
    }


}
