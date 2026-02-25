using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("UI组件")]
    public Image healthBar;           // 红色血条
    public TextMeshProUGUI healthText; // TMP文字 - 血量

    public Image manaBar;             // 蓝色蓝条
    public TextMeshProUGUI manaText;   // TMP文字 - 蓝量

    [Header("玩家")]
    public PlayerHealth playerHealth;

    // 记录血条原始宽度
    private float healthBarWidth;
    private float manaBarWidth;

    void Start()
    {
        // 如果没有指定玩家，自动查找
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        // 记录血条的原始宽度
        if (healthBar != null)
        {
            healthBarWidth = healthBar.rectTransform.rect.width;
            Debug.Log($"血条初始宽度：{healthBarWidth}");
        }

        if (manaBar != null)
        {
            manaBarWidth = manaBar.rectTransform.rect.width;
            Debug.Log($"蓝条初始宽度：{manaBarWidth}");
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        // 更新血条
        UpdateHealthBar();

        // 更新蓝条
        UpdateManaBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercent = playerHealth.currentHealth / playerHealth.maxHealth;

            // 直接修改宽度
            RectTransform rect = healthBar.rectTransform;
            Vector2 size = rect.sizeDelta;
            size.x = healthBarWidth * healthPercent;
            rect.sizeDelta = size;
        }

        // 更新TMP文字
        if (healthText != null)
        {
            healthText.text = $"{playerHealth.currentHealth}/{playerHealth.maxHealth}";
        }
    }

    void UpdateManaBar()
    {
        if (manaBar != null)
        {
            float manaPercent = playerHealth.currentMana / playerHealth.maxMana;

            RectTransform rect = manaBar.rectTransform;
            Vector2 size = rect.sizeDelta;
            size.x = manaBarWidth * manaPercent;
            rect.sizeDelta = size;
        }

        // 更新TMP文字
        if (manaText != null)
        {
            manaText.text = $"{playerHealth.currentMana}/{playerHealth.maxMana}";
        }
    }
    public void RefreshUI()
    {
        UpdateHealthBar();
        UpdateManaBar();
    }
}