using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{

    [Header("世界UI设置")]
    public GameObject 世界UIPrefab;

    [Header("背包设置")]
    public int 最大格子数 = 10;
    public List<Item> 物品列表 = new List<Item>();

    [Header("UI")]
    public TextMeshProUGUI 背包文本;
    public GameObject 背包面板;

    [Header("物品预制体")]
    public List<ItemPrefabMapping> 物品预制体列表;

    private bool 背包是否打开 = false;
    private MainInput 输入控制;

    void Awake()
    {
        输入控制 = new MainInput();
        输入控制.GameInput.Bag.performed += 切换背包;
    }

    void Start()
    {
        for (int i = 0; i < 最大格子数; i++)
        {
            物品列表.Add(new Item("", 0));
        }

        if (背包文本 == null)
        {
            Debug.LogError("请把TextMeshPro组件拖到脚本上！");
            return;
        }

        if (背包面板 != null)
        {
            背包面板.SetActive(false);
            背包是否打开 = false;
        }

        更新UI();
        Debug.Log("初始化完成");
    }

    private void 切换背包(InputAction.CallbackContext 上下文)
    {
        if (上下文.performed)
        {
            背包是否打开 = !背包是否打开;

            if (背包面板 != null)
            {
                背包面板.SetActive(背包是否打开);

                if (背包是否打开)
                {
                    更新UI();
                }
            }
        }
    }

    public void 捡起物品(ItemData 物品数据)
    {
        Debug.Log("=== 捡起物品 ===");
        Debug.Log("物品: " + 物品数据.itemName + " x" + 物品数据.数量);

        bool 已堆叠 = false;
        for (int i = 0; i < 物品列表.Count; i++)
        {
            if (物品列表[i].itemName == 物品数据.itemName)
            {
                物品列表[i].amount += 物品数据.数量;
                已堆叠 = true;
                Debug.Log("堆叠到格子 " + i + "，现在数量: " + 物品列表[i].amount);
                break;
            }
        }

        if (!已堆叠)
        {
            for (int i = 0; i < 物品列表.Count; i++)
            {
                if (string.IsNullOrEmpty(物品列表[i].itemName))
                {
                    物品列表[i].itemName = 物品数据.itemName;
                    物品列表[i].amount = 物品数据.数量;
                    Debug.Log("放到空格子 " + i);
                    已堆叠 = true;
                    break;
                }
            }
        }

        if (已堆叠)
        {
            更新UI();
            Destroy(物品数据.gameObject);
            Debug.Log("捡起成功");
        }
        else
        {
            Debug.Log("背包满了！");
        }
    }

    public void 更新UI()
    {
        if (背包文本 == null) return;

        string 显示文本 = "背包：\n";
        bool 有物品 = false;

        for (int i = 0; i < 物品列表.Count; i++)
        {
            if (!string.IsNullOrEmpty(物品列表[i].itemName) && 物品列表[i].amount > 0)
            {
                显示文本 += "[" + i + "] " + 物品列表[i].itemName + " x" + 物品列表[i].amount + "\n";
                有物品 = true;
            }
        }

        if (!有物品)
        {
            显示文本 += "空";
        }

        背包文本.text = 显示文本;
    }

    public void Update()
    {
        for (int i = 0; i < 最大格子数 && i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                Debug.Log("按下了数字键 " + i);
                丢弃物品(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("空格键，丢弃第一个物品");
            for (int i = 0; i < 物品列表.Count; i++)
            {
                if (!string.IsNullOrEmpty(物品列表[i].itemName))
                {
                    丢弃物品(i);
                    break;
                }
            }
        }
    }

    void 丢弃物品(int 格子索引)
    {
        Debug.Log("尝试丢弃格子 " + 格子索引);

        if (格子索引 < 0 || 格子索引 >= 物品列表.Count)
        {
            Debug.Log("格子索引无效");
            return;
        }

        Item 要丢弃的物品 = 物品列表[格子索引];

        if (string.IsNullOrEmpty(要丢弃的物品.itemName) || 要丢弃的物品.amount <= 0)
        {
            Debug.Log("这个格子是空的");
            return;
        }

        Debug.Log("丢弃: " + 要丢弃的物品.itemName + " x" + 要丢弃的物品.amount);

        GameObject 要生成的预制体 = null;
        foreach (var 映射 in 物品预制体列表)
        {
            if (映射.itemName == 要丢弃的物品.itemName)
            {
                要生成的预制体 = 映射.prefab;
                break;
            }
        }

        if (要生成的预制体 != null)
        {
            Vector3 丢弃位置 = transform.position + transform.forward * 1.5f + Vector3.up * 0.2f;
            丢弃位置 += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

            GameObject 丢弃物品 = Instantiate(要生成的预制体, 丢弃位置, Quaternion.identity);

            // 设置物品数据
            ItemData 物品数据 = 丢弃物品.GetComponent<ItemData>();
            if (物品数据 != null)
            {
                物品数据.itemName = 要丢弃的物品.itemName;
                物品数据.数量 = 要丢弃的物品.amount;
            }

            // 添加UI组件
            PickupWithUI ui = 丢弃物品.GetComponent<PickupWithUI>();
            if (ui == null)
            {
                ui = 丢弃物品.AddComponent<PickupWithUI>();
            }

            // 设置UI参数 - 使用Inspector中设置的预制体
            if (世界UIPrefab != null)
            {
                ui.uiPrefab = 世界UIPrefab;
            }
            else
            {
                Debug.LogWarning("世界UIPrefab没有设置，丢弃的物品将不会显示头顶UI");
            }

            ui.物品名称 = 要丢弃的物品.itemName;
            ui.物品数量 = 要丢弃的物品.amount;
            ui.detectionRadius = 3f;
            ui.uiHeight = 2f;
            //UI设置完成

            丢弃物品.transform.rotation = Random.rotation;

            Debug.Log("生成了: " + 要丢弃的物品.itemName + " 并添加了UI");
        }
        else
        {
            Debug.LogWarning("找不到物品 " + 要丢弃的物品.itemName + " 对应的预制体！");
        }

        物品列表[格子索引] = new Item("", 0);
        更新UI();
    }

    void OnEnable()
    {
        输入控制?.Enable();
    }

    void OnDisable()
    {
        输入控制?.Disable();
    }

    void OnDestroy()
    {
        if (输入控制 != null)
        {
            输入控制.GameInput.Bag.performed -= 切换背包;
        }
    }
}
[System.Serializable]
public class ItemPrefabMapping
{
    public string itemName;      // 物品名字
    public GameObject prefab;    // 对应的预制体
}
