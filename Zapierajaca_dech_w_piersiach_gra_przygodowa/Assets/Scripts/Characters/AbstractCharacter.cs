using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharacter: MonoBehaviour
{
    [Header("Health and armor")]
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _health;
    [SerializeField] protected float dieAwaitTime;
    [SerializeField] protected int _armour;
    [SerializeField] protected int _damage;
    [SerializeField] protected bool _blocking;
    [SerializeField] protected bool _immortal = false;

    [Header("Speed of moving")]
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runSpeed;
    [SerializeField] protected float _acceleration;
    [SerializeField] protected float _accelerationChaseBonus;
    [SerializeField] protected float attackRadius;

    [Header("Looting")]
    public List<DropInformation> dropList;

    public void drop(Vector3 characterPosition)
    {
        Vector3 dropPosition;
        foreach (DropInformation information in dropList)
        {
            for (int i = 0; i < information.maxAmount; i++)
            {
                if (Random.Range(0f, 100f) < information.dropRate)
                {
                    dropPosition = new Vector3(Random.Range(-2, 2), 0.5f, Random.Range(-2, 2)) + characterPosition;
                    information.item.Drop(dropPosition);
                }
            }
        }
    }

    public void changeHealth(int value)
    {
        _health += value;
        if (_health > _maxHealth)
            _health = _maxHealth;
        else if (_health <= 0)
        { 
            _health = 0;
            die();
        }

        Debug.Log("Current health: " + _health);
    }
    
    public bool isDead()
    {
        if (_health == 0)
        {
            return true;
        }
        return false;
    }

    public void changeArmour(int change)
    {
        _armour += change;
    }

    public void changeDamage(int change)
    {
        _damage = change;
    }

    [System.Serializable]
    public class DropInformation
    {
        public Items item;
        [Tooltip("In Percentes")]
        [Range(0,100)]
        public float dropRate;
        public int maxAmount;
    }

    /************ABSTRACT************/
    public abstract void die();
    public abstract void getHit(int damage);
    public abstract float getWalkSpeed();
    public abstract float getRunSpeed();
    public abstract float getAcceleration(bool isChasing);
    public abstract void attack();
    public abstract void setStateMachine(int state, int postState, float specialInfo);
    
}
