using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    int posX = 10;
    int posY = 10;
    int width = 250;
    int height = 50;
    int buffer = 10;
    GUIStyle style;
    List<Items> itemList;
    List<string> alreadyDisplayed;

    private void OnGUI()
    {
        // Create style for a button
        style = new GUIStyle(GUI.skin.button);
        style.fontSize = 20;
        Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
        style.font = myFont;
        style.normal.textColor = Color.white;
        style.hover.textColor = Color.white;
        itemList = Managers.Inventory.GetItemList();
        if (itemList.Count == 0)
            GUI.Box(new Rect(buffer, buffer, width, height), "Brak Przedmiotów", style);

        alreadyDisplayed = new List<string>();

        displayInventory();

        displayEquipped();

        displayPrepareAndUse();

    }

    void displayInventory()
    {
        posX = buffer;
        posY = buffer;
        foreach (Items item in itemList)
        {
            int count = Managers.Inventory.GetItemCount(item);

            if (!alreadyDisplayed.Contains(item.itemName + item.type))
            {
                alreadyDisplayed.Add(item.itemName + item.type);
                GUI.Box(new Rect(posX, posY, width, height), item.itemName + " (" + count + ")", style);
                posX += width + buffer;
            }
        }
    }

    void displayEquipped()
    {
        string equipped = Managers.Inventory.equippedItem;
        if (equipped != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped " + equipped), style);
            posY += height + buffer;
        }

        string equippedWeapon = Managers.Inventory.equippedWeapon;
        if (equippedWeapon != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped We " + equippedWeapon), style);
            posY += height + buffer;
        }

        string equippedShield = Managers.Inventory.equippedShield;
        if (equippedShield != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Sh " + equippedShield), style);
            posY += height + buffer;
        }

        string equippedPotion = Managers.Inventory.equippedPotion;
        if (equippedPotion != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Po " + equippedPotion), style);
            posY += height + buffer;
        }

        string equippedSpecial = Managers.Inventory.equippedSpecial;
        if (equippedSpecial != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Sp " + equippedSpecial), style);
            posY += height + buffer;
        }
    }

    void displayPrepareAndUse()
    {
        posX = buffer;
        posY = height + 2 * buffer;
        foreach (string name in alreadyDisplayed)
        {
            posY = 2 * buffer + height;
            if (GUI.Button(new Rect(posX, posY, width, height), "Przygotuj " + name, style))
            {
                Managers.Inventory.EquipItem(Managers.Inventory.getItem(name));
            }

            if (name == "health")
            {
                if (GUI.Button(new Rect(posX, posY + height + buffer, width, height), "Use Potion", style))
                {
                    Managers.Inventory.ConsumeItem(Managers.Inventory.getItem(name));
                    Managers.Player.ChangeHealth(25);
                }
            }
            this.posX += width + buffer;
        }
    }
}
