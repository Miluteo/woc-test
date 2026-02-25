using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider 其他碰撞体)
    {
        if (其他碰撞体.CompareTag("Player"))
        {
            PlayerInventory 玩家背包 = 其他碰撞体.GetComponent<PlayerInventory>();
            ItemData 物品数据 = GetComponent<ItemData>();

            if (玩家背包 != null && 物品数据 != null)
            {
                物品数据.OnPickedUp();  // 通知UI销毁
                玩家背包.捡起物品(物品数据);
            }
        }
    }
}