using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    private void OnGUI()
    {
        int posX = 10;
        int posY = 10;
        int width = 250;
        int height = 50;
        int buffer = 10;

        // Create style for a button
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 20;
        // Load and set Font
        Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
        myButtonStyle.font = myFont;
        // Set color for selected and unselected buttons
        myButtonStyle.normal.textColor = Color.white;
        myButtonStyle.hover.textColor = Color.white;

        List<Items> itemList = Managers.Inventory.GetItemList();
        if (itemList.Count == 0)
            GUI.Box(new Rect(posX, posY, width, height), "Brak Przedmiotów", myButtonStyle);
        Debug.Log(itemList.Count);

        List<string> alreadyDisplayed = new List<string>();
        foreach (Items item in itemList)
        {
            int count = Managers.Inventory.GetItemCount(item);
            
            if (!alreadyDisplayed.Contains(item.itemName + item.type))
            {
                alreadyDisplayed.Add(item.itemName + item.type);
                GUI.Box(new Rect(posX, posY, width, height), item.itemName + " (" + count + ")", myButtonStyle);
                posX += width + buffer;
            }
        }


        string equipped = Managers.Inventory.equippedItem;
        if (equipped != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped " + equipped), myButtonStyle);
            posY += height + buffer;
        }
        

        string equippedWeapon = Managers.Inventory.equippedWeapon;
        if (equippedWeapon != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped We " + equippedWeapon), myButtonStyle);
            posY += height + buffer;
        }
        

        string equippedShield = Managers.Inventory.equippedShield;
        if (equippedShield != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Sh " + equippedShield), myButtonStyle);
            posY += height + buffer;
        }
        

        string equippedPotion = Managers.Inventory.equippedPotion;
        if (equippedPotion != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Po " + equippedPotion), myButtonStyle);
            posY += height + buffer;
        }
        

        string equippedSpecial = Managers.Inventory.equippedSpecial;
        if (equippedSpecial != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped Sp " + equippedSpecial), myButtonStyle);
            posY += height + buffer;
        }

        posX = buffer;


        foreach (string name in alreadyDisplayed)
        {
            posY = 2 * buffer + height;
            if (GUI.Button(new Rect(posX, posY, width, height), "Przygotuj " + name, myButtonStyle))
            {
                Managers.Inventory.EquipItem(Managers.Inventory.getItem(name));
            }

            if (name == "health")
            {
                if (GUI.Button(new Rect(posX, posY + height + buffer, width, height), "Use Potion", myButtonStyle))
                {
                    Managers.Inventory.ConsumeItem(Managers.Inventory.getItem(name));
                    Managers.Player.ChangeHealth(25);
                }
            }
            posX += width + buffer;
        }

    }
}
