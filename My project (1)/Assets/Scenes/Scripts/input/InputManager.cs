using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    //设置成单例模式
    private static InputManager _instance = new InputManager();

    public static InputManager Instance => _instance;

    private MainInput mainInput;


    //是否按下攻击键
    public bool ATK => mainInput.GameInput.ATK.triggered;
    //读取Value
    public Vector2 Move => mainInput.GameInput.Movement.ReadValue<Vector2>();
    //检测按键是否处于按下状态

    //构造函数初始化
    private InputManager()
    {
        //初始化Input
        mainInput = new MainInput();
        mainInput.Enable();
    }

    ~InputManager()
    {
        //释放Input
        mainInput.Disable();
        mainInput = null;

    }
    public Vector2 CameraMove => mainInput.GameInput.CameraMove.ReadValue<Vector2>();
}