using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 处理死亡区域逻辑
/// </summary>
public class DeadArea : MonoBehaviour
{
    [Header("死亡判定设置")] 
    [SerializeField] private float warningTimerMax = 1f; // 物体进入区域后 到开始死亡倒计时的秒数
    [SerializeField] private float deathTimerMax = 3.0f; // 物体进入区域后的死亡倒计时
    
    [Header("警告设置")]
    [SerializeField] private Color normalColor = Color.clear; // 正常颜色（透明）
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.5f); // 警告颜色（半透明红色）
    [SerializeField] private float warningPulseTime = 0.5f; // 警告闪烁时间
    
    private Image warningBorderImage; // 警告边框图像
    
    private bool isAnyInDeadArea = false;       // 是否有物体是否在死亡区域
    private bool isWarning = false;
    private float warningTimer = 0f;                // 当前警告计时器
    private float deathTimer = 0f;                  // 当前死亡计时器
    private Sequence warningSequence;               // 警告动画序列
    private int jellyfishCount = 0;                  // 当前区域内水母数量
    public int JellyfishCount => jellyfishCount;
    
    private void Start()
    {
        // 初始化UI
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        warningBorderImage = UIManager.Instance.WarningUI.WarningImage;
        
        // 初始化警告边框
        warningBorderImage.color = normalColor;
    }
    
    private void Update()
    {
        CheckAnyInDeadArea();
    }

    private void CheckAnyInDeadArea()
    {
        // 如果已经死亡，不再检查
        if (GameManager.Instance.IsGameOver) return;
            
        // 如果有物体在死亡区域，增加警告计时器
        if (isAnyInDeadArea)
        {
            warningTimer += Time.deltaTime;
            if (warningTimer >= warningTimerMax)
            {
                if (!isWarning)
                {
                    // 播放警告动画
                    StartWarningAnimation();
                    isWarning = true;
                }
                
                deathTimer += Time.deltaTime;
                
                // 如果计时器超过死亡时间，触发死亡
                if (deathTimer >= deathTimerMax)
                {
                    TriggerGameOver();
                }
            }
        }
        else
        {
            StopWarningAnimation();
            
            // 如果没有物体在死亡区域，重置计时器
            warningTimer = 0f;
            deathTimer = 0f;
        }
    }
    
    private void TriggerGameOver()
    {
        GameManager.Instance.TriggerGameOver();
        
        // 停止警告动画
        StopWarningAnimation();
        
        // 可以在这里添加其他死亡逻辑，比如停止游戏等
        Debug.Log("玩家死亡：物体超出区域时间过长！");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTriggerEnter(collision.gameObject.name);
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTriggerExit(collision.gameObject.name);
    }
    
    private void HandleTriggerEnter(string objectName)
    {
        jellyfishCount++;
        
        isAnyInDeadArea = true;
        
        Debug.Log($"物体 {objectName} 进入死亡区域");
    }
    
    private void HandleTriggerExit(string objectName)
    {
        jellyfishCount--;
        
        // 如果没有碰撞体在区域内，停止警告
        if (jellyfishCount <= 0)
        {
            jellyfishCount = 0; // 确保不会变成负数
            
            isAnyInDeadArea = false;
            
            Debug.Log($"物体 {objectName} 离开死亡区域");
        }
    }
    
    private void StartWarningAnimation()
    {
        // 如果已经死亡，不再显示警告
        if (GameManager.Instance.IsGameOver) return;
            
        UIManager.Instance.ShowWarningUI();
            
        // 停止之前的动画序列
        warningSequence?.Kill();

        // 创建新的动画序列
        warningSequence = DOTween.Sequence();
        
        // 添加颜色渐变动画
        warningSequence.Append(warningBorderImage.DOColor(warningColor, warningPulseTime / 2))
                      .Append(warningBorderImage.DOColor(new Color(warningColor.r, warningColor.g, warningColor.b, 0.2f), warningPulseTime / 2))
                      .SetLoops(-1, LoopType.Restart); // 无限循环
    }
    
    private void StopWarningAnimation()
    {
        isWarning = false;
        UIManager.Instance.HideWarningUI();
        
        // 停止动画序列
        warningSequence.Kill();
        
        // 恢复正常颜色
        warningBorderImage.DOColor(normalColor, warningPulseTime / 2);
    }
}
