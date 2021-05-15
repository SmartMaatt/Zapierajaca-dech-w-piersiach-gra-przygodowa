using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerHeal : MonoBehaviour
{
    [SerializeField] float healingTime;
    [SerializeField] int maxHeal = 100;
    [SerializeField] int minHeal = 30;

    [SerializeField] GameObject healSpell;
    [SerializeField] UIBar potionIcon;
    [SerializeField] UIBar healthBar;
    [SerializeField] GameObject NoPotionErrorPopup;

    private PlayerManager _playerManager;
    private bool _canHeal;
    
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _canHeal = true;

        potionIcon.setUpBar((int)(healingTime * 100));
        NoPotionErrorPopup.SetActive(false);
    }

    void Update()
    {
        if (_canHeal && Input.GetKeyDown(KeyCode.H))
        {
            if (Managers.Inventory.equippedPotion == "Health Potion")
            {
                StartCoroutine(HealingProcess());
                Managers.Inventory.InventoryView.transform.Find("Health PotionPOTION_slot").gameObject.GetComponent<EquipButtonClick>().Drop(false);
            }
            else
            {
                StartCoroutine(errorPopup());
            }
        }
    }

    public bool isHealing()
    {
        return !_canHeal;
    }

    private IEnumerator errorPopup()
    {
        NoPotionErrorPopup.SetActive(true);
        yield return new WaitForSeconds(2);
        NoPotionErrorPopup.SetActive(false);
    }

    private IEnumerator HealingProcess()
    {
        bool stopParticle = false;
        float particleLimit = 3 / healingTime;

        _canHeal = false;
        healSpell.SetActive(true);
        healSpell.GetComponent<EffectSettings>().IsVisible = true;
        float elapsedTime = 0.0f;

        int healthPoint = Random.Range(minHeal, maxHeal);
        _playerManager.changeHealth(healthPoint);
        healthBar.addBarValue(healthPoint);

        while(elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / healingTime;
            potionIcon.setBarValue((int)(elapsedTime * 100 * healingTime));

            if(elapsedTime > particleLimit && !stopParticle)
            {
                stopParticle = true;
                healSpell.GetComponent<EffectSettings>().IsVisible = false;
            }

            yield return new WaitForEndOfFrame();
        }

        _canHeal = true;
        potionIcon.setUpBar((int)(healingTime * 100));
        healSpell.SetActive(false);
    }
}
