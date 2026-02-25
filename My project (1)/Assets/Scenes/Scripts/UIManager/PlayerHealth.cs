using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("数值设置")]
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxMana = 100f;
    public float currentMana;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        Debug.Log($"初始状态 - 血量：{currentHealth}/{maxHealth}，蓝量：{currentMana}/{maxMana}");
    }

    void Update()
    {
        // 测试按键
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentHealth -= 10f;
            if (currentHealth < 0) currentHealth = 0;
            Debug.Log($"扣血！当前血量：{currentHealth}/{maxHealth}");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            currentHealth += 10f;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            Debug.Log($"回血！当前血量：{currentHealth}/{maxHealth}");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (currentMana >= 5f)
            {
                currentMana -= 5f;
                Debug.Log($"消耗蓝量！当前蓝量：{currentMana}/{maxMana}");
            }
            else
            {
                Debug.Log("蓝量不足！");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            Debug.Log("重置满血满蓝！");
        }
    }
}