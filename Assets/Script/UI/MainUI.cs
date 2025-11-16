using System;
using Script.EventSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("控制")]
    [Tooltip("左移按钮")] public Button leftButton;
    [Tooltip("右移按钮")] public Button rightButton;
    [Tooltip("放下按钮")] public Button dropButton;
    
    [FormerlySerializedAs("scoreText")]
    [Header("分数显示")] 
    [Tooltip("当前分数")] public TextMeshProUGUI currentScoreText;
    [Tooltip("最高分数")] public TextMeshProUGUI highScoreText;
    
    [Header("设置")]
    [Tooltip("设置按钮")] public Button settingsButton;
    
    [FormerlySerializedAs("nextJellyfishPreview")]
    [Header("水母预览")]
    [Tooltip("下一个水母的预览图像")] public Image nextJellyfishImage;

    private void Start()
    {
        settingsButton.onClick.AddListener(() =>
        {
            // UIManager.Instance.ShowSettingUI();
            this.TriggerEvent(EventName.gamePaused);
        });
        
        EventManager.Instance.AddListener(EventName.scoreChanged, UpdateScoreDisplay);
        EventManager.Instance.AddListener(EventName.highScoreChanged, UpdateHighScoreDisplay);
        
        SetupButtonEvents();
        
        // 初始化分数显示
        UpdateScoreDisplay(this, new ScoreEventArgs { score = GameManager.Instance.GetCurrentScore() });
        UpdateHighScoreDisplay(this, new ScoreEventArgs { score = GameManager.Instance.GetHighScore() });
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.scoreChanged, UpdateScoreDisplay);
        EventManager.Instance.RemoveListener(EventName.highScoreChanged, UpdateHighScoreDisplay);
    }
    
    /// <summary>
    /// 更新最高分
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateHighScoreDisplay(object sender, EventArgs e)
    {
        var scoreEventArgs = e as ScoreEventArgs;
        int highScore = scoreEventArgs.score;
        
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }
    }

    /// <summary>
    /// 更新分数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateScoreDisplay(object sender, EventArgs e)
    {
        ScoreEventArgs scoreEventArgs = e as ScoreEventArgs;
        int score = scoreEventArgs.score;
        if (currentScoreText != null)
        {
            currentScoreText.text = score.ToString();
        }
    }
    
    /// <summary>
    /// 更新下一个水母的预览图像
    /// </summary>
    /// <param name="sprite">水母图像</param>
    /// <param name="jellyfishName">水母名称</param>
    public void UpdateNextJellyfishPreview(Sprite sprite, string jellyfishName = "")
    {
        if (nextJellyfishImage != null && sprite != null)
        {
            // 设置精灵
            nextJellyfishImage.sprite = sprite;
            nextJellyfishImage.enabled = true;
            
            // 设置为原生大小
            nextJellyfishImage.preserveAspect = true; // 保持宽高比
            
            // 如果Image组件的设置为SetNativeSize，则应用原生大小
            nextJellyfishImage.SetNativeSize();
            
            // 如果需要显示水母名称，可以在这里添加代码
        }
    }
    
    // 左右按钮按下状态
    private bool isLeftButtonPressed = false;
    private bool isPreviousLeftButtonPressed = false;
    private bool isRightButtonPressed = false;
    private bool isPreviousRightButtonPressed = false;
    
    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtonEvents()
    {
        // 添加按下事件监听
        EventTrigger leftTrigger = leftButton.GetComponent<EventTrigger>() ?? leftButton.AddComponent<EventTrigger>();
        
        // 清除现有事件
        leftTrigger.triggers.Clear(); 
        
        // 设置左按钮事件
        // 添加按下事件
        EventTrigger.Entry leftEntry = new EventTrigger.Entry();
        leftEntry.eventID = EventTriggerType.PointerDown;
        leftEntry.callback.AddListener((data) => { isLeftButtonPressed = true; });
        leftTrigger.triggers.Add(leftEntry);
        
        // 添加抬起事件
        EventTrigger.Entry leftUpEntry = new EventTrigger.Entry();
        leftUpEntry.eventID = EventTriggerType.PointerUp;
        leftUpEntry.callback.AddListener((data) => { isLeftButtonPressed = false; });
        leftTrigger.triggers.Add(leftUpEntry);
        
        
        // 设置右按钮事件
        // 添加按下事件监听
        EventTrigger rightTrigger = rightButton.GetComponent<EventTrigger>() ?? rightButton.AddComponent<EventTrigger>();
        
        // 清除现有事件
        rightTrigger.triggers.Clear();
        
        // 添加按下事件
        EventTrigger.Entry rightEntry = new EventTrigger.Entry();
        rightEntry.eventID = EventTriggerType.PointerDown;
        rightEntry.callback.AddListener((data) => { isRightButtonPressed = true; });
        rightTrigger.triggers.Add(rightEntry);
        
        // 添加抬起事件
        EventTrigger.Entry rightUpEntry = new EventTrigger.Entry();
        rightUpEntry.eventID = EventTriggerType.PointerUp;
        rightUpEntry.callback.AddListener((data) => { isRightButtonPressed = false; });
        rightTrigger.triggers.Add(rightUpEntry);
        

        // 设置放下按钮事件
        // 添加按下事件监听
        EventTrigger dropTrigger = dropButton.GetComponent<EventTrigger>() ?? dropButton.AddComponent<EventTrigger>();
        
        // 清除现有事件
        dropTrigger.triggers.Clear();
        
        // 添加按下事件
        EventTrigger.Entry dropEntry = new EventTrigger.Entry();
        dropEntry.eventID = EventTriggerType.PointerDown;
        dropEntry.callback.AddListener((data) => { OnDropButtonPerformed(); });
        dropTrigger.triggers.Add(dropEntry);
        
        // 添加抬起事件
        EventTrigger.Entry dropUpEntry = new EventTrigger.Entry();
        dropUpEntry.eventID = EventTriggerType.PointerUp;
        dropUpEntry.callback.AddListener((data) => { OnDropButtonCanceled(); });
        dropTrigger.triggers.Add(dropUpEntry);
    }

    /// <summary>
    /// 放下按钮点击事件
    /// </summary>
    private void OnDropButtonPerformed()
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        this.TriggerEvent(EventName.trailPreviewed);
    }

    /// <summary>
    /// 放下按钮松开事件
    /// </summary>
    private void OnDropButtonCanceled()
    {
        // 如果游戏暂停，不处理按钮输入
        if (GameManager.Instance.IsGamePaused) return;
        
        this.TriggerEvent(EventName.jellyfishDropped);
    }

    private void Update()
    {
        HandleMoveInput();
    }

    /// <summary>
    /// 处理左右按钮
    /// </summary>
    private void HandleMoveInput()
    {
        // 如果游戏暂停，不处理按钮输入
        if (GameManager.Instance.IsGamePaused) return;
        
        // 如果左按钮被按下，且和上一帧不同，持续向左移动
        if (isLeftButtonPressed != isPreviousLeftButtonPressed)
        {
            // MechanicalArmController.Instance.MoveLeft();
            isPreviousLeftButtonPressed = isLeftButtonPressed;
            if (isLeftButtonPressed)
            {
                GameInput.Instance.IsMoveLeft = true;
            }
            else
            {
                GameInput.Instance.IsMoveLeft = false;
            }
        }
        
        // 如果右按钮被按下，且和上一帧不同，持续向右移动
        if (isRightButtonPressed != isPreviousRightButtonPressed)
        {
            isPreviousRightButtonPressed = isRightButtonPressed;
            if (isRightButtonPressed)
            {
                GameInput.Instance.IsMoveRight = true;
            }
            else
            {
                GameInput.Instance.IsMoveRight = false;
            }
        }
    }
}
