using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 机械臂移动控制器
/// 负责机械臂的移动控制和位置约束
/// </summary>
public class MechanicalArmController : MonoBehaviour
{
    [Header("移动范围设置")]
    [SerializeField] public float minX = -5f;  // X轴最小值
    [SerializeField] public float maxX = 5f;   // X轴最大值
    
    [Header("相机设置")]
    [SerializeField] private Camera targetCamera; // 用于转换坐标的相机
    
    [Header("点击设置")]
    [SerializeField] private float clickPlaneHeight = 0f;  // 点击平面的高度
    [SerializeField] private bool useFixedZ = true;        // 是否使用固定的Z值
    [SerializeField] private float fixedZPosition = 0f;    // 固定的Z位置值
    
    [Header("键盘控制设置")]
    [SerializeField] private float keyboardMoveSpeed = 5f;  // 键盘移动速度
    [SerializeField] private bool enableKeyboardControl = true;  // 启用键盘控制
    [SerializeField] private KeyCode dropKey = KeyCode.J;  // 放下水母的按键
    
    [Header("UI按钮控制设置")]
    [SerializeField] private float buttonMoveSpeed = 1f;  // 按钮移动速度
    
    [Header("虚线设置")]
    [SerializeField] private GameObject dottedLine;       // 虚线游戏对象
    [SerializeField] private float hideDelay = 0.5f;      // 隐藏延迟时间（秒）
    
    // 场景控制器引用
    private ScnController sceneController;
    
    // 移动状态跟踪
    private bool isMoving = false;
    private Vector3 lastPosition;
    private float movementTimer = 0f;
    
    // 初始化
    void Start()
    {
        // 如果没有指定相机，使用主相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("找不到主相机！请确保场景中有标记为MainCamera的相机。");
            }
        }
        
        // 设置点击平面高度为机械臂的Y位置
        clickPlaneHeight = transform.position.y;
        
        // 获取场景控制器
        sceneController = FindObjectOfType<ScnController>();
        if (sceneController == null)
        {
            Debug.LogWarning("未找到场景控制器！请确保场景中有ScnController组件。");
        }
        
        // 初始化位置
        lastPosition = transform.position;
        
        // 初始时隐藏虚线
        if (dottedLine != null)
        {
            dottedLine.SetActive(false);
        }
    }
    
    // 每帧更新
    void Update()
    {
        // 处理鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        
        // 如果启用了键盘控制，检测键盘输入
        if (enableKeyboardControl)
        {
            HandleKeyboardInput();
            
            // 检测J键按下，放下水母
            if (Input.GetKeyDown(dropKey))
            {
                if (sceneController != null)
                {
                    sceneController.SpawnJellyfishAtArm();
                }
            }
        }
        
        // 检测移动状态
        CheckMovementState();
    }
    
    /// <summary>
    /// 检测移动状态，控制虚线显示/隐藏
    /// </summary>
    private void CheckMovementState()
    {
        // 检测是否移动
        if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {
            // 正在移动
            isMoving = true;
            movementTimer = 0f;
            
            // 显示虚线
            ShowDottedLine(true);
            
            // 更新上一个位置
            lastPosition = transform.position;
        }
        else if (isMoving)
        {
            // 已停止移动，但仍处于"移动状态"
            movementTimer += Time.deltaTime;
            
            // 如果停止时间超过延迟，则隐藏虚线
            if (movementTimer >= hideDelay)
            {
                isMoving = false;
                ShowDottedLine(false);
            }
        }
    }
    
    /// <summary>
    /// 显示或隐藏虚线
    /// </summary>
    private void ShowDottedLine(bool show)
    {
        if (dottedLine != null)
        {
            dottedLine.SetActive(show);
        }
    }
    
    /// <summary>
    /// 处理键盘输入
    /// </summary>
    private void HandleKeyboardInput()
    {
        // 检查是否有水平方向输入（AD键或左右箭头键）
        float horizontalInput = 0f;
        
        // 检查A键或左箭头键（向左移动）
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput -= 1f;
        }
        
        // 检查D键或右箭头键（向右移动）
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput += 1f;
        }
        
        // 如果有输入，移动机械臂
        if (horizontalInput != 0f)
        {
            // 计算移动距离
            float moveDistance = horizontalInput * keyboardMoveSpeed * Time.deltaTime;
            
            // 计算新的位置
            float newX = transform.position.x + moveDistance;
            
            // 创建新的位置向量
            Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
            
            // 移动到新位置
            MoveToPosition(newPosition);
        }
    }
    
    /// <summary>
    /// 处理鼠标点击，移动机械臂到点击位置并放下水母
    /// </summary>
    public void HandleMouseClick()
    {
        // 检查是否点击了UI元素
        if (IsPointerOverUIElement())
        {
            // 如果点击了UI元素，不处理机械臂移动
            return;
        }
        
        if (targetCamera == null)
        {
            Debug.LogError("相机未找到，无法处理点击");
            return;
        }
        
        // 将鼠标点击位置转换为世界坐标
        Vector3 worldPos = GetMouseWorldPosition();
        
        // 移动机械臂到点击位置
        if (worldPos != Vector3.zero)
        {
            MoveToPosition(worldPos);
            
            // 鼠标点击后放下水母
            if (sceneController != null)
            {
                sceneController.SpawnJellyfishAtArm();
            }
        }
    }
    
    /// <summary>
    /// 检查鼠标是否在UI元素上
    /// </summary>
    private bool IsPointerOverUIElement()
    {
        // 检查是否有EventSystem
        if (EventSystem.current == null)
            return false;
            
        // 检查鼠标是否在UI元素上
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        return results.Count > 0;
    }
    
    /// <summary>
    /// 获取鼠标点击的世界坐标位置
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;
        
        // 设置Z坐标为相机到平面的距离
        mousePos.z = targetCamera.transform.position.y - clickPlaneHeight;
        if (mousePos.z < 0.1f) mousePos.z = 10f; // 确保距离为正值
        
        // 将屏幕坐标转换为世界坐标
        Vector3 worldPos = targetCamera.ScreenToWorldPoint(mousePos);
        
        // 使用机械臂的Y坐标和固定的Z坐标
        worldPos.y = transform.position.y;
        if (useFixedZ)
        {
            worldPos.z = fixedZPosition;
        }
        
        return worldPos;
    }
    
    /// <summary>
    /// 移动机械臂到指定位置，限制在X轴范围内
    /// </summary>
    public void MoveToPosition(Vector3 position)
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
    
    /// <summary>
    /// 设置移动范围
    /// </summary>
    public void SetMovementRange(float min, float max)
    {
        minX = min;
        maxX = max;
    }
    
    /// <summary>
    /// 获取当前X位置
    /// </summary>
    public float GetCurrentXPosition()
    {
        return transform.position.x;
    }
    
    /// <summary>
    /// 启用或禁用键盘控制
    /// </summary>
    public void SetKeyboardControlEnabled(bool enabled)
    {
        enableKeyboardControl = enabled;
    }
    
    /// <summary>
    /// 设置键盘移动速度
    /// </summary>
    public void SetKeyboardMoveSpeed(float speed)
    {
        keyboardMoveSpeed = Mathf.Max(0.1f, speed);
    }
    
    /// <summary>
    /// UI按钮控制 - 向左移动（供UI按钮调用）
    /// </summary>
    public void MoveLeft()
    {
        // 计算新的位置
        float newX = transform.position.x - buttonMoveSpeed * Time.deltaTime * 10f;
        
        // 创建新的位置向量
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
        
        // 移动到新位置
        MoveToPosition(newPosition);
    }
    
    /// <summary>
    /// UI按钮控制 - 向右移动（供UI按钮调用）
    /// </summary>
    public void MoveRight()
    {
        // 计算新的位置
        float newX = transform.position.x + buttonMoveSpeed * Time.deltaTime * 10f;
        
        // 创建新的位置向量
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
        
        // 移动到新位置
        MoveToPosition(newPosition);
    }
    
    /// <summary>
    /// UI按钮控制 - 放下水母（供UI按钮调用）
    /// </summary>
    public void DropJellyfish()
    {
        if (sceneController != null)
        {
            sceneController.SpawnJellyfishAtArm();
        }
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