using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("血量设置")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("是否死亡")]
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"敌人受到{damage}点伤害，剩余血量：{currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("敌人死亡");
        Destroy(gameObject, 1f); // 2秒后销毁
    }

    // 获取血量百分比
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}