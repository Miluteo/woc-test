using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarSpawner : MonoBehaviour
{
    [Header("血条预制体")]
    public GameObject healthBarPrefab;  // 血条预制体

    [Header("偏移设置")]
    public Vector3 offset = new Vector3(0, 2.5f, 0);  // 血条在敌人头顶的偏移

    private GameObject currentHealthBar;
    private EnemyHealth enemyHealth;

    void Start()
    {
        // 获取敌人的血量脚本
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("敌人身上没有EnemyHealth脚本！");
            return;
        }

        // 如果没指定预制体，从Resources加载
        if (healthBarPrefab == null)
        {
            healthBarPrefab = Resources.Load<GameObject>("Prefabs/EnemyHealthBar");
        }

        // 生成血条
        if (healthBarPrefab != null)
        {
            // 生成血条
            currentHealthBar = Instantiate(healthBarPrefab, transform.position + offset, Quaternion.identity);
            currentHealthBar.name = $"HealthBar_{gameObject.name}";

            // 获取血条上的控制器
            EnemyHealthBarController controller = currentHealthBar.GetComponent<EnemyHealthBarController>();
            if (controller != null)
            {
                // 把当前敌人赋值给血条控制器
                controller.enemyHealth = enemyHealth;
                controller.offset = offset;  // 传递偏移量

                Debug.Log($"为敌人 {gameObject.name} 生成了血条");
            }
        }
        else
        {
            Debug.LogError("找不到血条预制体！请检查 Resources/Prefabs/EnemyHealthBar");
        }
    }

    void OnDestroy()
    {
        // 当敌人销毁时，同时销毁它的血条
        if (currentHealthBar != null)
        {
            Destroy(currentHealthBar);
        }
    }
}