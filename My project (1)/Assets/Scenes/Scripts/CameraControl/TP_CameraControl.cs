using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TP_CameraControl : MonoBehaviour
{
    [SerializeField, Header("跟随目标")]
    public Transform target;

    [SerializeField, Header("TP_Camera与目标的偏移距离")]
    public float OffsetDistance;

    [SerializeField, Header("相机位置移动的移动插值系统")]
    public float PositionSmoothTime;

    [SerializeField, Header("相机位置移动的移动插值系统")]
    public float RotationSmoothTime;

    [SerializeField, Header("相机最大俯仰角限制")]
    public float MaxVerticalRotationAngle;

    [SerializeField, Header("相机最小俯仰角限制")]
    public float MinVerticalRotationAngle;

    [SerializeField, Header("相机最大水平角限制")]
    public float MaxHorizontalRotationAngle;

    [SerializeField, Header("相机最小水平角限制")]
    public float MinHorizontalRotationAngle;

    private Vector2 _input = new Vector2(0, 0);
    private Vector3 _cameraRotation = new Vector3(0, 0, 0);

    //检测输入
    private void Update()
    {
        CameraInput();
    }

    //插值移动
    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateRotation();
    }

    private void CameraInput()
    {
        _input.x -= InputManager.Instance.CameraMove.y;
        _input.y += InputManager.Instance.CameraMove.x;

        _input.x = Mathf.Clamp(_input.x,MinVerticalRotationAngle,MaxVerticalRotationAngle);
        _input.y = Mathf.Clamp(_input.y, MinHorizontalRotationAngle, MaxHorizontalRotationAngle);
    }

    private void UpdateRotation()
    {
        // 计算目标旋转（四元数）
        Quaternion targetRotation = Quaternion.Euler(_input.x, _input.y, 0);

        // 使用 Slerp 平滑旋转
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,RotationSmoothTime * Time.deltaTime);

        // 锁定 Z 轴旋转（防止相机倾斜）
        Vector3 euler = transform.eulerAngles;
        euler.z = 0;
        transform.eulerAngles = euler;
    }




    private void UpdateCameraPosition()
    {
        //计算相机下一帧要移动到那个位置
        Vector3 nesPos = target.position + (-transform.forward * OffsetDistance);

        //插值
        transform.position = Vector3.Lerp(transform.position, nesPos, Time.deltaTime * PositionSmoothTime );
    }



}
