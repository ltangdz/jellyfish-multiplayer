using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("水母预览")]
    public Image nextJellyfishPreview; // 下一个水母的预览图像
    
    [Header("控制按钮")]
    public Button leftButton;          // 左移按钮
    public Button rightButton;         // 右移按钮
    public Button dropButton;          // 放下水母按钮
    
    [Header("分数显示")]
    public Text scoreText;             // 当前分数文本
    public Text highScoreText;         // 最高分数文本
    
    [Header("设置面板")]
    public Button settingsButton;      // 设置按钮
    public GameObject settingsPanel;   // 设置面板
    public Button closeSettingsButton; // 关闭设置面板按钮
    public Slider musicVolumeSlider;   // 音乐音量滑动条
    public Text musicVolumeText;       // 音乐音量百分比文本
    public Slider sfxVolumeSlider;     // 音效音量滑动条
    public Text sfxVolumeText;         // 音效音量百分比文本
    public Button restartButton;       // 重新开始按钮
    public Button quitButton;          // 退出游戏按钮
    
    [Header("音频设置")]
    public AudioSource musicAudioSource; // 音乐音频源
    [Range(0f, 1f)]
    [SerializeField] private float defaultMusicVolume = 0.5f; // 默认音乐音量
    [Range(0f, 1f)]
    [SerializeField] private float defaultSfxVolume = 0.5f;   // 默认音效音量
    
    private ScnController sceneController;         // 场景控制器引用
    private MechanicalArmController armController; // 机械臂控制器引用
    private GameManager gameManager;               // 游戏管理器引用
    private bool isGamePaused = false;             // 游戏是否暂停
    
    // 音量键
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取场景控制器引用
        sceneController = FindObjectOfType<ScnController>();
        if (sceneController == null)
        {
            Debug.LogWarning("未找到ScnController！水母预览功能可能无法正常工作。");
        }
        
        // 获取机械臂控制器引用
        armController = FindObjectOfType<MechanicalArmController>();
        if (armController == null)
        {
            Debug.LogWarning("未找到MechanicalArmController！按钮控制功能可能无法正常工作。");
        }
        
        // 获取游戏管理器引用
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogWarning("未找到GameManager！分数显示功能可能无法正常工作。");
        }
        else
        {
            // 订阅分数变化事件
            gameManager.OnScoreChanged += UpdateScoreDisplay;
            gameManager.OnHighScoreChanged += UpdateHighScoreDisplay;
            
            // 初始化分数显示
            UpdateScoreDisplay(gameManager.GetCurrentScore());
            UpdateHighScoreDisplay(gameManager.GetHighScore());
        }
        
        // 检查预览图像组件
        if (nextJellyfishPreview == null)
        {
            Debug.LogWarning("未设置水母预览图像组件！请在Inspector中设置。");
        }
        
        // 检查分数文本组件
        if (scoreText == null)
        {
            Debug.LogWarning("未设置分数文本组件！请在Inspector中设置。");
        }
        
        if (highScoreText == null)
        {
            Debug.LogWarning("未设置最高分文本组件！请在Inspector中设置。");
        }
        
        // 初始化设置面板
        InitializeSettingsPanel();
        
        // 设置按钮事件
        SetupButtonEvents();
    }
    
    // 初始化设置面板
    private void InitializeSettingsPanel()
    {
        // 确保设置面板初始隐藏
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("未设置设置面板！请在Inspector中设置。");
        }
        
        // 设置打开按钮事件
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettingsPanel);
        }
        else
        {
            Debug.LogWarning("未设置设置按钮！请在Inspector中设置。");
        }
        
        // 设置关闭按钮事件
        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(CloseSettingsPanel);
        }
        else
        {
            Debug.LogWarning("未设置关闭设置面板按钮！请在Inspector中设置。");
        }
        
        // 初始化音乐音量滑动条
        if (musicVolumeSlider != null)
        {
            // 从PlayerPrefs加载音乐音量
            float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
            musicVolumeSlider.value = musicVolume;
            
            // 设置音乐音量
            SetMusicVolume(musicVolume);
            
            // 添加滑动条事件
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogWarning("未设置音乐音量滑动条！请在Inspector中设置。");
        }
        
        // 初始化音效音量滑动条
        if (sfxVolumeSlider != null)
        {
            // 从PlayerPrefs加载音效音量
            float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSfxVolume);
            sfxVolumeSlider.value = sfxVolume;
            
            // 设置音效音量
            SetSfxVolume(sfxVolume);
            
            // 添加滑动条事件
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
        else
        {
            Debug.LogWarning("未设置音效音量滑动条！请在Inspector中设置。");
        }
        
        // 设置重新开始按钮事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("未设置重新开始按钮！请在Inspector中设置。");
        }
        
        // 设置退出按钮事件
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("未设置退出游戏按钮！请在Inspector中设置。");
        }
    }
    
    // 打开设置面板
    private void OpenSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            PauseGame(true);
        }
    }
    
    // 关闭设置面板
    private void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            PauseGame(false);
        }
    }
    
    // 切换设置面板显示/隐藏
    private void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
            
            // 切换游戏暂停状态
            PauseGame(isActive);
        }
    }
    
    // 暂停/恢复游戏
    private void PauseGame(bool pause)
    {
        isGamePaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
    
    // 设置音乐音量
    private void SetMusicVolume(float volume)
    {
        // 保存音乐音量
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        
        // 更新音乐音量文本
        if (musicVolumeText != null)
        {
            musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        }
        
        // 设置音乐音频源音量
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("未找到音乐音频源！");
        }
    }
    
    // 设置音效音量
    private void SetSfxVolume(float volume)
    {
        // 保存音效音量
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        
        // 更新音效音量文本
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        }
        
        // 通知所有水母更新音效音量
        JellyfishController[] jellyfishControllers = FindObjectsOfType<JellyfishController>();
        foreach (JellyfishController jellyfish in jellyfishControllers)
        {
            jellyfish.UpdateSoundVolume(volume);
        }
    }
    
    // 重新开始游戏
    private void RestartGame()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        
        // 重新加载当前场景
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    
    // 退出游戏
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // 当对象被销毁时
    void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        if (gameManager != null)
        {
            gameManager.OnScoreChanged -= UpdateScoreDisplay;
            gameManager.OnHighScoreChanged -= UpdateHighScoreDisplay;
        }
        
        // 恢复时间缩放
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // 检测按钮持续按下状态
        CheckButtonHold();
        
        // 检测ESC键，打开/关闭设置面板
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
    }
    
    /// <summary>
    /// 更新分数显示
    /// </summary>
    /// <param name="score">当前分数</param>
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
    
    /// <summary>
    /// 更新最高分显示
    /// </summary>
    /// <param name="highScore">最高分数</param>
    private void UpdateHighScoreDisplay(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }
    }
    
    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtonEvents()
    {
        // 设置左按钮事件
        if (leftButton != null)
        {
            // 添加按下事件监听
            EventTrigger leftTrigger = leftButton.gameObject.GetComponent<EventTrigger>();
            if (leftTrigger == null)
            {
                leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // 清除现有事件
            if (leftTrigger.triggers != null)
            {
                leftTrigger.triggers.Clear();
            }
            else
            {
                leftTrigger.triggers = new List<EventTrigger.Entry>();
            }
            
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
        }
        
        // 设置右按钮事件
        if (rightButton != null)
        {
            // 添加按下事件监听
            EventTrigger rightTrigger = rightButton.gameObject.GetComponent<EventTrigger>();
            if (rightTrigger == null)
            {
                rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // 清除现有事件
            if (rightTrigger.triggers != null)
            {
                rightTrigger.triggers.Clear();
            }
            else
            {
                rightTrigger.triggers = new List<EventTrigger.Entry>();
            }
            
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
        }
        
        // 设置放下按钮事件
        if (dropButton != null)
        {
            dropButton.onClick.AddListener(OnDropButtonClicked);
        }
    }
    
    // 左右按钮按下状态
    private bool isLeftButtonPressed = false;
    private bool isRightButtonPressed = false;
    
    /// <summary>
    /// 检测按钮持续按下状态
    /// </summary>
    private void CheckButtonHold()
    {
        // 如果游戏暂停，不处理按钮输入
        if (isGamePaused) return;
        
        if (armController == null) return;
        
        // 如果左按钮被按下，持续向左移动
        if (isLeftButtonPressed)
        {
            armController.MoveLeft();
        }
        
        // 如果右按钮被按下，持续向右移动
        if (isRightButtonPressed)
        {
            armController.MoveRight();
        }
    }
    
    /// <summary>
    /// 放下按钮点击事件
    /// </summary>
    public void OnDropButtonClicked()
    {
        // 如果游戏暂停，不处理按钮输入
        if (isGamePaused) return;
        
        if (armController != null)
        {
            armController.DropJellyfish();
        }
    }
    
    /// <summary>
    /// 更新下一个水母的预览图像
    /// </summary>
    /// <param name="sprite">水母图像</param>
    /// <param name="jellyfishName">水母名称</param>
    public void UpdateNextJellyfishPreview(Sprite sprite, string jellyfishName = "")
    {
        if (nextJellyfishPreview != null && sprite != null)
        {
            // 设置精灵
            nextJellyfishPreview.sprite = sprite;
            nextJellyfishPreview.enabled = true;
            
            // 设置为原生大小
            nextJellyfishPreview.preserveAspect = true; // 保持宽高比
            
            // 如果Image组件的设置为SetNativeSize，则应用原生大小
            nextJellyfishPreview.SetNativeSize();
            
            // 如果需要显示水母名称，可以在这里添加代码
        }
    }
    
    /// <summary>
    /// 获取当前音效音量
    /// </summary>
    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSfxVolume);
    }
}
