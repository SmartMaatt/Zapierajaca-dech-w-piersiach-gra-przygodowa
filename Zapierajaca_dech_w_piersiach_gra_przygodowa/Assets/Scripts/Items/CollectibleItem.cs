using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, InteractOperator
{
    [SerializeField] private string itemName;

    public void Operate()
    {
        Managers.Inventory.AddItem(itemName);
        Destroy(this.gameObject);
    }
}
