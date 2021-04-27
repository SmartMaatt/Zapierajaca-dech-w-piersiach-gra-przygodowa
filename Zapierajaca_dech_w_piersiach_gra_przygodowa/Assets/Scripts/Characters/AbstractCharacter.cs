using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharacter: MonoBehaviour
{
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _health;
    [SerializeField] protected int _armour;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _lowSpeed;
    [SerializeField] protected int _highSpeed;
    [SerializeField] protected bool _blocking;

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
        else if (_health < 0)
            _health = 0;
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

    public abstract void die();

    [System.Serializable]
    public class DropInformation
    {
        public Items item;
        [Tooltip("In Percentes")]
        public float dropRate;
        public int maxAmount;
    }
}
