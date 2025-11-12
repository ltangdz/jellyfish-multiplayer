using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Die : MonoBehaviour
{
    [Header("死亡判定设置")]
    [SerializeField] private float deathTimer = 3.0f; // 物体进入区域后的死亡倒计时
    
    [Header("UI引用")]
    [SerializeField] private Image warningBorder; // 警告边框图像
    [SerializeField] private Text gameOverText; // 游戏结束文本
    [SerializeField] private Image blackPanel; // 黑色面板
    [SerializeField] private Button restartButton; // 重新开始按钮
    
    [Header("警告设置")]
    [SerializeField] private Color normalColor = Color.clear; // 正常颜色（透明）
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.5f); // 警告颜色（半透明红色）
    [SerializeField] private float warningPulseTime = 0.5f; // 警告闪烁时间
    
    [Header("游戏结束设置")]
    [SerializeField] private float fadeInTime = 1.0f; // 黑色面板淡入时间
    [SerializeField] private float textScaleTime = 0.5f; // 文本缩放时间
    
    private bool isObjectInDeathZone = false; // 物体是否在死亡区域
    private float currentDeathTimer = 0f; // 当前死亡计时器
    private bool isDead = false; // 是否已经死亡
    private Sequence warningSequence; // 警告动画序列
    private int colliderCount = 0; // 当前碰撞体数量
    
    private void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 设置重新开始按钮监听
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }
    
    private void InitializeUI()
    {
        // 初始化警告边框
        if (warningBorder != null)
        {
            warningBorder.color = normalColor;
        }
        
        // 初始化游戏结束文本和黑色面板
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        
        if (blackPanel != null)
        {
            Color panelColor = blackPanel.color;
            panelColor.a = 0f;
            blackPanel.color = panelColor;
            blackPanel.gameObject.SetActive(false);
        }
        
        // 初始化重新开始按钮
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        // 如果已经死亡，不再检查
        if (isDead)
            return;
            
        // 如果有物体在死亡区域，增加计时器
        if (isObjectInDeathZone)
        {
            currentDeathTimer += Time.deltaTime;
            
            // 如果计时器超过死亡时间，触发死亡
            if (currentDeathTimer >= deathTimer)
            {
                TriggerDeath();
            }
        }
        else
        {
            // 如果没有物体在死亡区域，重置计时器
            currentDeathTimer = 0f;
        }
    }
    
    // 3D碰撞检测
    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other.gameObject.name);
    }
    
    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other.gameObject.name);
    }
    
    // 2D碰撞检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other.gameObject.name);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        HandleTriggerExit(other.gameObject.name);
    }
    
    // 统一处理进入触发器
    private void HandleTriggerEnter(string objectName)
    {
        // 增加碰撞体计数
        colliderCount++;
        
        // 有物体进入，开始警告
        isObjectInDeathZone = true;
        StartWarningAnimation();
        
        Debug.Log($"物体 {objectName} 进入死亡区域");
    }
    
    // 统一处理离开触发器
    private void HandleTriggerExit(string objectName)
    {
        // 减少碰撞体计数
        colliderCount--;
        
        // 如果没有碰撞体在区域内，停止警告
        if (colliderCount <= 0)
        {
            colliderCount = 0; // 确保不会变成负数
            isObjectInDeathZone = false;
            StopWarningAnimation();
            
            Debug.Log($"物体 {objectName} 离开死亡区域");
        }
    }
    
    private void StartWarningAnimation()
    {
        // 如果已经死亡，不再显示警告
        if (isDead)
            return;
            
        // 如果警告边框不存在，直接返回
        if (warningBorder == null)
            return;
            
        // 停止之前的动画序列
        if (warningSequence != null)
        {
            warningSequence.Kill();
        }
        
        // 创建新的动画序列
        warningSequence = DOTween.Sequence();
        
        // 添加颜色渐变动画
        warningSequence.Append(warningBorder.DOColor(warningColor, warningPulseTime / 2))
                      .Append(warningBorder.DOColor(new Color(warningColor.r, warningColor.g, warningColor.b, 0.2f), warningPulseTime / 2))
                      .SetLoops(-1, LoopType.Restart); // 无限循环
    }
    
    private void StopWarningAnimation()
    {
        // 如果警告边框不存在，直接返回
        if (warningBorder == null)
            return;
            
        // 停止动画序列
        if (warningSequence != null)
        {
            warningSequence.Kill();
        }
        
        // 恢复正常颜色
        warningBorder.DOColor(normalColor, warningPulseTime / 2);
    }
    
    private void TriggerDeath()
    {
        // 标记为已死亡
        isDead = true;
        
        // 停止警告动画
        StopWarningAnimation();
        
        // 显示游戏结束UI
        ShowGameOverUI();
        
        // 可以在这里添加其他死亡逻辑，比如停止游戏等
        Debug.Log("玩家死亡：物体超出区域时间过长！");
    }
    
    private void ShowGameOverUI()
    {
        // 显示黑色面板
        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            blackPanel.DOFade(0.8f, fadeInTime).SetEase(Ease.InOutQuad);
        }
        
        // 显示游戏结束文本
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.transform.localScale = Vector3.zero;
            
            // 添加缩放动画
            gameOverText.transform.DOScale(1f, textScaleTime)
                .SetEase(Ease.OutBack)
                .SetDelay(fadeInTime * 0.5f);
        }
        
        // 显示重新开始按钮
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.transform.localScale = Vector3.zero;
            
            // 添加缩放动画
            restartButton.transform.DOScale(1f, textScaleTime)
                .SetEase(Ease.OutBack)
                .SetDelay(fadeInTime * 0.8f);
        }
    }
    
    private void RestartGame()
    {
        // 重新加载当前场景
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
