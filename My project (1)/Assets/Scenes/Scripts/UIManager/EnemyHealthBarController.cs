using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBarController : MonoBehaviour
{
    [Header("血条组件")]
    public Image healthBarFill;           // 红色填充条
    public TextMeshProUGUI healthText;    // 血量文字

    [Header("设置")]
    public float visibleDistance = 10f;   // 可见距离
    public Vector3 offset = new Vector3(0, 2.5f, 0);  // 血条在敌人头顶的偏移
    public float barWidth = 2f;           // 血条宽度

    [Header("引用")]
    public EnemyHealth enemyHealth;        // **这个会在生成时被赋值**

    private Canvas canvas;
    private Camera mainCamera;
    private Transform playerTransform;
    private bool isVisible = false;
    private RectTransform canvasRect;
    private RectTransform fillRect;

    void Start()
    {
        // 获取组件
        canvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();
        mainCamera = Camera.main;

        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // 设置血条
        SetupHealthBar();

        // 设置Canvas
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;
            canvasRect.sizeDelta = new Vector2(barWidth, 0.3f);
            canvasRect.localScale = Vector3.one;

            // 初始隐藏
            canvas.enabled = false;
            isVisible = false;
        }
    }

    void SetupHealthBar()
    {
        if (healthBarFill == null) return;

        fillRect = healthBarFill.rectTransform;

        // 设置fillRect的锚点和偏移
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // 设置为红色
        healthBarFill.color = Color.red;
        healthBarFill.raycastTarget = false;

        // 设置文字
        if (healthText != null)
        {
            RectTransform textRect = healthText.rectTransform;
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.localPosition = Vector3.zero;

            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;
            healthText.fontSize = 0.5f;
        }
    }

    void LateUpdate()
    {
        // **如果没有指定敌人，或者敌人被销毁了，就销毁血条**
        if (enemyHealth == null)
        {
            Destroy(gameObject);
            return;
        }

        if (playerTransform == null || canvas == null) return;

        // 将血条放在敌人正上方
        Vector3 targetPosition = enemyHealth.transform.position + offset;
        transform.position = targetPosition;

        // 让血条面向摄像机
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward, Vector3.up);
        }

        // 计算距离
        float distance = Vector3.Distance(enemyHealth.transform.position, playerTransform.position);

        // 根据距离显示/隐藏
        if (distance <= visibleDistance && !isVisible && !enemyHealth.isDead)
        {
            ShowHealthBar();
        }
        else if ((distance > visibleDistance || enemyHealth.isDead) && isVisible)
        {
            HideHealthBar();
        }

        // 更新血量显示
        if (isVisible)
        {
            UpdateHealthDisplay();
        }
    }

    void ShowHealthBar()
    {
        canvas.enabled = true;
        isVisible = true;
    }

    void HideHealthBar()
    {
        canvas.enabled = false;
        isVisible = false;
    }

    void UpdateHealthDisplay()
    {
        if (healthBarFill != null && fillRect != null && canvasRect != null && enemyHealth != null)
        {
            float healthPercent = enemyHealth.GetHealthPercent();

            // 修改右边界的偏移
            float rightOffset = (1 - healthPercent) * canvasRect.rect.width;
            fillRect.offsetMax = new Vector2(-rightOffset, 0);

            healthBarFill.color = Color.red;
        }

        if (healthText != null && enemyHealth != null)
        {
            healthText.text = $"{enemyHealth.currentHealth:F0}/{enemyHealth.maxHealth:F0}";
        }
    }
}