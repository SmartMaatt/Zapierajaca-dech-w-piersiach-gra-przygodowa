using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour, InteractOperator
{
    public string itemName;
    public itemType type;
    public enum itemType
    {
        WEAPON = 0,
        SHIELD = 1,
        POTION = 2,
        SPECIAL = 3
    }

    public void Operate()
    {
        if(Managers.Inventory.AddItem(this))
            Destroy(this.gameObject);
    }

   
}
