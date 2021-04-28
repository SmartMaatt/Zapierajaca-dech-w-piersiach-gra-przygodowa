using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : AbstractCharacter, IGameManager
{
    public ManagerStatus status { get; private set; }

    public int health { get; private set; }
    public int maxHealth { get; private set; }

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera gracza...");

        health = _health;
        maxHealth = _maxHealth;

        status = ManagerStatus.Started;
    }

    public void ChangeHealth(int value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        else if (health < 0)
            health = 0;

        Debug.Log("Kondycja: " + health + " / " + maxHealth);
    }

    public override void die()
    {
        throw new System.NotImplementedException();
    }

    public override void getHit(int damage)
    {
        Debug.Log("Player get hit: " + damage);
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
}
