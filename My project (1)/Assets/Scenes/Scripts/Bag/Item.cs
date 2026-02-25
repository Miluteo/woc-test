using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class Item
{
    public string itemName;
    public int amount;
    public Sprite icon;

    public Item(string name, int amt)
    {
        itemName = name;
        amount = amt;
    }

    // ÅÐ¶ÏÊÇ·ñÎª¿Õ
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(itemName) || amount <= 0;
    }
}