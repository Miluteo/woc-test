using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackpackUI : MonoBehaviour
{
    public GameObject backpackPanel;
    public PlayerInventory playerInventory;  // 引用背包数据

    private bool isOpen = false;
    private MainInput inputActions;

    void Awake()
    {
        inputActions = new MainInput();
        inputActions.GameInput.Bag.performed += OnToggleBackpack;
    }

    void Start()
    {
        // 初始关闭
        if (backpackPanel != null)
        {
            backpackPanel.SetActive(false);
            isOpen = false;
        }

        // 如果没有手动拖拽，尝试自动查找
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }
    }

    private void OnToggleBackpack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleBackpack();
        }
    }

    public void ToggleBackpack()
    {
        isOpen = !isOpen;

        if (backpackPanel != null)
        {
            backpackPanel.SetActive(isOpen);

            // 打开时刷新UI
            if (isOpen && playerInventory != null)
            {
                playerInventory.更新UI();  
            }
        }
    }

    void OnEnable()
    {
        inputActions?.Enable();
    }

    void OnDisable()
    {
        inputActions?.Disable();
    }

    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.GameInput.Bag.performed -= OnToggleBackpack;
        }
    }
}