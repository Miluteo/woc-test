using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public int 数量 = 1;

    private PickupWithUI pickupUI;

    void Start()
    {
        pickupUI = GetComponent<PickupWithUI>();
        if (pickupUI != null)
        {
            pickupUI.物品名称 = itemName;
            pickupUI.物品数量 = 数量;
            pickupUI.UpdateUIText();
        }
    }

    public Item 转换为物品数据()
    {
        return new Item(itemName, 数量);
    }

    // 当物品被捡起时调用（由Pickup脚本触发）
    public void OnPickedUp()
    {
        if (pickupUI != null)
            pickupUI.OnPickup();
    }
}