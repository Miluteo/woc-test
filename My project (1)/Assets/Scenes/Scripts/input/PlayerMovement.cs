using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public InputActionReference moveAction;
    public InputActionReference attackAction;

    [Header("特效设置")]
    public GameObject[] attackVFX;  // 存放4段攻击对应的刀光特效预制体
    public Transform vfxSpawnPoint;  // 特效生成的位置（比如武器位置）
    public float vfxDestroyDelay = 0.2f; // 特效自动销毁延迟

    public float comboTimeWindow = 2.5f;    // 连击窗口时间（秒）

    [Header("挥刀音效")]
    public AudioClip[] waveSladeSounds;  // 长度设为4，分别对应4段攻击的挥刀音效

    // 攻击判定参数
    [Header("攻击判定设置")]
    public float attackAngle = 60f;          // 扇形角度（度）
    public float attackRadius = 2f;           // 扇形半径（米）
    public LayerMask enemyLayer;              // 敌人层级
    public Transform attackOrigin;             // 攻击判定原点

    // 跑步音效参数
    [Header("跑步音效")]
    public AudioClip[] footstepSounds;
    public float footstepVolume = 0.5f;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private AudioSource audioSource;

    // 连击系统变量
    private int currentCombo = 0;           // 当前连击数
    private float lastAttackTime;            // 上次按J的时间
    private bool canMove = true;              // 是否能移动
    private bool isAttacking = false;         // 是否正在攻击
    private bool canInputNext = false;        // 是否可以输入下一段连击（由EnablePreInput控制）

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (attackOrigin == null)
            attackOrigin = transform;
    }

    void OnEnable()
    {
        moveAction.action.performed += OnMovement;
        moveAction.action.canceled += OnMovement;
        moveAction.action.Enable();

        if (attackAction != null)
        {
            attackAction.action.performed += OnAttack;
            attackAction.action.Enable();
        }
    }

    void OnDisable()
    {
        moveAction.action.Disable();

        if (attackAction != null)
        {
            attackAction.action.performed -= OnAttack;
            attackAction.action.Disable();
        }
    }

    void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        float timeSinceLast = Time.time - lastAttackTime;

        // 如果能预输入，或者不在攻击中，或者超时了
        if (canInputNext || !isAttacking || timeSinceLast > comboTimeWindow)
        {
            if (timeSinceLast < comboTimeWindow && currentCombo > 0 && currentCombo < 5)
            {
                // 连击下一段
                currentCombo++;
            }
            else
            {
                // 新连击
                currentCombo = 1;
            }

            // 触发攻击动画
            animator.SetTrigger("Attack" + currentCombo);
            lastAttackTime = Time.time;
            canMove = false;
            isAttacking = true;
            canInputNext = false;  // 重置预输入

            // 停止之前的协程
            StopCoroutine("AttackTimeout");
            StartCoroutine("AttackTimeout");
        }
    }

    IEnumerator AttackTimeout()
    {
        yield return new WaitForSeconds(comboTimeWindow);

        if (isAttacking || !canMove)
        {
            canMove = true;
            isAttacking = false;
            currentCombo = 0;
            canInputNext = false;
        }
    }

    // 动画事件：挥刀音效
    public void WaveSladeSound()
    {
        if (waveSladeSounds != null && waveSladeSounds.Length >= currentCombo && currentCombo > 0)
        {
            AudioClip sound = waveSladeSounds[currentCombo - 1];
            if (sound != null && audioSource != null)
            {
                audioSource.PlayOneShot(sound, 0.5f); // 0.8是音量，可以自己调整
            }
        }
    }

    // 动画自带事件：允许预输入下一段连击
    public void EnablePreInput()
    {
        canInputNext = true;
    }

    // 动画自带事件：取消攻击冷却（重置计时器）
    public void CancelAttackColdTime()
    {
        lastAttackTime = Time.time;  // 重置计时，延长连击窗口
    }

    //动画自带事件：播放特效

    public void PlayVFX()
    {
        if (attackVFX != null && attackVFX.Length >= currentCombo && currentCombo > 0)
        {
            // 获取当前连击对应的特效
            GameObject vfxPrefab = attackVFX[currentCombo - 1];

            if (vfxPrefab != null && vfxSpawnPoint != null)
            {
                //获取角色面向方向，生成朝向角色的特效
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

                //在原有位置基础上，向上偏移一定高度
                Vector3 spawnPosition = vfxSpawnPoint.position + new Vector3(0, 0.8f, 0);

                // 在指定位置生成特效，使用角色的朝向
                GameObject vfx = Instantiate(vfxPrefab, spawnPosition, targetRotation);

                // 让特效跟随武器
                vfx.transform.SetParent(vfxSpawnPoint);

                // 自动销毁
                Destroy(vfx, vfxDestroyDelay);
            }
        }
    }



    //动画自带事件：攻击判定
    public void ATK()
    {
        OnAttackHit();
    }

    // 跑步音效
    public void PlayFootSound()
    {
        if (canMove && moveInput.magnitude > 0.1f && footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex], footstepVolume);
        }
    }

    // 攻击判定
    public void OnAttackHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(attackOrigin.position, attackRadius, enemyLayer);

        foreach (Collider enemy in hitColliders)
        {
            Vector3 directionToEnemy = (enemy.transform.position - attackOrigin.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle <= attackAngle * 0.5f)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(20);  // 这里会自动更新血量
                    Debug.Log("攻击到了敌人！");
                }
            }
        }
    }

    private bool CheckEnemyInSector()
    {
        Collider[] hitColliders = Physics.OverlapSphere(attackOrigin.position, attackRadius, enemyLayer);

        if (hitColliders.Length == 0)
            return false;

        foreach (Collider enemy in hitColliders)
        {
            Vector3 directionToEnemy = (enemy.transform.position - attackOrigin.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle <= attackAngle * 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    public void OnAttackEnd()
    {
        canMove = true;
    }

    public void PlayWeaponBackSound()
    { 
    
    }

    public void PlayWeaponEndSound()
    {

    }

    public void DisableLinkCombo()
    { 
    
    }

    void Update()
    {
        if (canMove)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            animator.SetFloat("Speed", moveInput.magnitude);

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
        else
        {
            controller.Move(Vector3.zero);
            animator.SetFloat("Speed", 0f);
        }

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            animator.SetTrigger("ATK");        
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;

        Gizmos.color = Color.red;

        Vector3 forward = transform.forward;
        Vector3 right = Quaternion.Euler(0, attackAngle * 0.5f, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -attackAngle * 0.5f, 0) * forward;

        Gizmos.DrawLine(attackOrigin.position, attackOrigin.position + right * attackRadius);
        Gizmos.DrawLine(attackOrigin.position, attackOrigin.position + left * attackRadius);

        int segments = 20;
        float angleStep = attackAngle / segments;
        Vector3 prevPoint = attackOrigin.position + right * attackRadius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -attackAngle * 0.5f + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            Vector3 point = attackOrigin.position + direction * attackRadius;

            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }
}