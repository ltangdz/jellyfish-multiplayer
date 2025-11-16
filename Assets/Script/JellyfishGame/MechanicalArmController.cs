using System;
using Script.EventSystem;
using UnityEngine;

/// <summary>
/// 机械臂移动控制器
/// 负责机械臂的移动控制和位置约束
/// </summary>
public class MechanicalArmController : MonoBehaviour
{
    [Header("移动范围设置")]
    [SerializeField] public float minX = -5f;  // X轴最小值
    [SerializeField] public float maxX = 5f;   // X轴最大值

    [Header("点击设置")]
    [SerializeField] private float clickPlaneHeight = 0f;  // 点击平面的高度
    
    [Header("键盘控制设置")]
    [SerializeField] private float keyboardMoveSpeed = 5f;  // 键盘移动速度
    
    [Header("虚线设置")]
    [SerializeField] private GameObject dottedLine;       // 虚线游戏对象
    
    
    private bool isReadyToSpawn = false;
    
    private void Start()
    {
        // 监听预览轨迹事件
        EventManager.Instance.AddListener(EventName.trailPreviewed, OnTrailPreviewed);
        
        // 监听水母下落事件
        EventManager.Instance.AddListener(EventName.jellyfishDropped, OnJellyfishDropped);
        
        // 设置点击平面高度为机械臂的Y位置
        clickPlaneHeight = transform.position.y;
        
        // 初始时隐藏虚线
        HideDottedLine();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.trailPreviewed, OnTrailPreviewed);
        
        EventManager.Instance.RemoveListener(EventName.jellyfishDropped, OnJellyfishDropped);
    }

    private void OnTrailPreviewed(object sender, EventArgs e)
    {
        if(!SpawnManager.Instance.CanClick || GameManager.Instance.IsGamePaused) return;
        
        Debug.Log("预览轨迹中");
        isReadyToSpawn = true;
        ShowDottedLine();
    }

    private void OnJellyfishDropped(object sender, EventArgs e)
    {
        if(!isReadyToSpawn || GameManager.Instance.IsGamePaused) return;
        
        Debug.Log("生成中");
        SpawnManager.Instance.SpawnJellyfishAtArm();
        HideDottedLine();
        isReadyToSpawn = false;
    }

    private void Update()
    {
        if (GameInput.Instance.IsMove)
        {
            HandleMoveAction();
        }
    }
    
    
    private void ShowDottedLine()
    {
        dottedLine.SetActive(true);
    }

    private void HideDottedLine()
    {
        dottedLine.SetActive(false);
    }
    
    private void HandleMoveAction()
    {
        int dirX = GameInput.Instance.IsMoveLeft ? -1 : 1;
        
        // 计算移动距离
        float moveDistance = dirX * keyboardMoveSpeed * Time.deltaTime;
        
        // 创建新的位置向量
        Vector3 targetPos = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);
        
        // 移动到新位置
        MoveToPosition(targetPos);
    }
    
    /// <summary>
    /// 移动机械臂到指定位置，限制在X轴范围内
    /// </summary>
    private void MoveToPosition(Vector3 position)
    {
        // 限制X轴在允许范围内
        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        
        // 设置目标位置，只改变X轴，保持Y和Z不变
        Vector3 newPosition = new Vector3(
            clampedX,
            transform.position.y,
            transform.position.z
        );
        
        // 直接移动到目标位置
        transform.position = newPosition;
    }
    
    
    // 在编辑器中绘制移动范围
    private void OnDrawGizmos()
    {
        // 绘制移动范围
        Gizmos.color = Color.yellow;
        Vector3 minPos = new Vector3(minX, transform.position.y, transform.position.z);
        Vector3 maxPos = new Vector3(maxX, transform.position.y, transform.position.z);
        
        Gizmos.DrawSphere(minPos, 0.2f);
        Gizmos.DrawSphere(maxPos, 0.2f);
        Gizmos.DrawLine(minPos, maxPos);
        
        // 绘制点击平面
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(new Vector3(0, clickPlaneHeight, 0), new Vector3(20, 0.01f, 20));
    }
} 