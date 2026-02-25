using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupWithUI : MonoBehaviour
{
    [Header("物品数据")]
    public string 物品名称;
    public int 物品数量 = 1;

    [Header("UI设置")]
    public GameObject uiPrefab;          // 世界UI预制体
    public float uiHeight = 1f;           // UI显示在物体上方多高
    public float detectionRadius = 5f;    // 玩家靠近检测范围

    private GameObject uiInstance;        // 动态生成的UI实例
    private Transform player;              // 玩家位置
    private bool isPlayerNear = false;

    void Start()
    {
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // 生成UI（默认隐藏）
        if (uiPrefab != null)
        {
            uiInstance = Instantiate(uiPrefab, transform.position + Vector3.up * uiHeight, Quaternion.identity);
            uiInstance.transform.SetParent(transform); // 让UI跟随物体移动
            UpdateUIText();
            uiInstance.SetActive(false); // 初始隐藏
        }
    }

    void Update()
    {
        if (player == null || uiInstance == null) return;

        // 检测玩家距离
        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldShow = distance <= detectionRadius;

        if (shouldShow != isPlayerNear)
        {
            isPlayerNear = shouldShow;
            uiInstance.SetActive(isPlayerNear);
        }

        // 让UI始终面向玩家
        if (isPlayerNear)
        {
            uiInstance.transform.LookAt(player);
            uiInstance.transform.rotation = Quaternion.LookRotation(uiInstance.transform.position - player.position);
        }
    }

    // 更新UI上的文字
    public void UpdateUIText()
    {
        if (uiInstance != null)
        {
            TextMeshProUGUI text = uiInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{物品名称} x{物品数量}";
        }
    }

    // 物品被捡起时调用
    public void OnPickup()
    {
        if (uiInstance != null)
            Destroy(uiInstance);
    }

    // 物品被丢弃时重新激活UI
    private void OnDestroy()
    {
        if (uiInstance != null)
            Destroy(uiInstance);
    }

    // 在编辑器中显示检测范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}