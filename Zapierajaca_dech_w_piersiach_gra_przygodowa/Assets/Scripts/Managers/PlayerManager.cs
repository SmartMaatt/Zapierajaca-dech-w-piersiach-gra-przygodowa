using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class PlayerManager : AbstractCharacter, IGameManager
{
    public ManagerStatus status { get; private set; }
    public int health { get; private set; }
    public int maxHealth { get; private set; }

    [Header("Special - Player")]
    [SerializeField] CameraFollow cameraScript;
    [SerializeField] UIBar _healthBar;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] ElementFade LevelUpSprite;
    [SerializeField] LayerMask PortalMask;
    [Space]
    public int money;
    public int exp;
    public int firstXPFactor;
    public int level;
    public int maxLevel;

    private Animator _animator;
    private RelativeMovement _movementScript;
    private MakeDamage _damageScript;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _damageScript = GetComponent<MakeDamage>();
        _movementScript = GetComponent<RelativeMovement>();

        if (_healthBar)
        {
            _healthBar.setUpBar(_maxHealth);
            _healthBar.setBarValue(_health);
        }
        if(moneyText)
            moneyText.text = money.ToString();

        if(levelText)
            levelText.text = level.ToString();
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

        UIController UI = FindObjectsOfType<UIController>()[0];
        if (UI) { UI.playerDiedScene(); }

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
            float armorBonus;
            if (_armour > 0)
                armorBonus = (float)_armour / 100 * (float)damage;
            else
                armorBonus = 0;

            changeHealth(-damage + (int)armorBonus);
            _healthBar.setBarValue(_health);

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

    public void changeMoney(int amound)
    {
        money += amound;
        moneyText.text = money.ToString();
    }

    public void changeExp(int amound)
    {
        exp += amound;
        levelUp();
    }

    public void levelUp()
    {
        if((exp > (firstXPFactor * level)) && (level <= maxLevel))
        {
            Debug.Log(exp + " " + firstXPFactor + " " + level);
            exp -= firstXPFactor * level;
            level++;
            levelText.text = level.ToString();

            _maxHealth += 15;
            _healthBar.setUpBar(_maxHealth);
            _healthBar.setBarValue(_health);

            _movementScript.addRunStamina(1);

            _damageScript.addAttackStrength(1);

            LevelUpSprite.RunFace(3, 0.5f);
            levelUp();
        }
    }
}
