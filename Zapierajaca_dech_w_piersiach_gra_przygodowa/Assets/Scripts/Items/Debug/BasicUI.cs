using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    private void OnGUI()
    {
        int posX = 10;
        int posY = 10;
        int width = 200;
        int height = 50;
        int buffer = 10;

        // Create style for a button
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 20;
        // Load and set Font
        Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
        myButtonStyle.font = myFont;
        // Set color for selected and unselected buttons
        myButtonStyle.normal.textColor = Color.red;
        myButtonStyle.hover.textColor = Color.red;

        List<string> itemList = Managers.Inventory.GetItemList();
        if (itemList.Count == 0)
            GUI.Box(new Rect(posX, posY, width, height), "Brak Przedmiotów", myButtonStyle);

        foreach (string item in itemList)
        {
            int count = Managers.Inventory.GetItemCount(item);
            GUI.Box(new Rect(posX, posY, width, height), item + " (" + count + ")", myButtonStyle);
            posX += width + buffer;
        }

        string equipped = Managers.Inventory.equippedItem;
        if (equipped != null)
        {
            posX = Screen.width - (width + buffer);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped + " + equipped), myButtonStyle);
        }

        posX = 10;
        posY += height + buffer;

        foreach (string item in itemList)
        {
            if (GUI.Button(new Rect(posX, posY, width, height), "Przygotuj " + item, myButtonStyle))
                Managers.Inventory.EquipItem(item);

            if (item == "health")
            {
                if (GUI.Button(new Rect(posX, posY + height + buffer, width, height), "Use Potion", myButtonStyle))
                {
                    Managers.Inventory.ConsumeItem("health");
                    Managers.Player.ChangeHealth(25);
                }
            }
            posX += width + buffer;
        }

    }
}
