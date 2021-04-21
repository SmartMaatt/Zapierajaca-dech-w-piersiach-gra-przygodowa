using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField] private int _health = 100;
    [SerializeField] private int _maxHealth = 100;
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
}
