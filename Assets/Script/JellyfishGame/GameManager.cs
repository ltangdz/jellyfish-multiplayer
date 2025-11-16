using UnityEngine;
using Script.EventSystem;

public enum GameState
{
    ReadyToStart,
    Playing,
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
    [Header("游戏数据")]
    [SerializeField] private int currentScore = 0;   // 当前分数
    [SerializeField] private int highScore = 0;      // 最高分数
    [Tooltip("水母生成的父物体")]
    [SerializeField] private Transform jellyfishContainer;
    public Transform JellyfishContainer => jellyfishContainer;

    
    private bool isGamePaused = false;
    public bool IsGamePaused => isGamePaused;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    private GameState currentState;
    
    private float readyToStartTimer = 5f;
    private float readyToStartTimerMax = 5f;
    
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        
        if (isGamePaused)
        {
        }
        else
        {
        }
    }


    private void Update()
    {
        switch (currentState)
        {
            case GameState.ReadyToStart:
                readyToStartTimer -= Time.deltaTime;
                if (readyToStartTimer <= 0f)
                {
                    currentState = GameState.Playing;
                    readyToStartTimer = readyToStartTimerMax;
                    Debug.Log("游戏开始");
                    this.TriggerEvent(EventName.gameStart);
                }
                break;
            case GameState.Playing:
                break;
            case GameState.GameOver:
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        // 从PlayerPrefs加载最高分
        LoadHighScore();
    }
    
    private void Start()
    {
        // 初始化分数
        ResetScore();
        
        currentState = GameState.ReadyToStart;
    }

    private void OnDestroy()
    {
        
    }

    /// <summary>
    /// 增加分数
    /// </summary>
    /// <param name="points">要增加的分数</param>
    public void UpdateScore(int points)
    {
        if (points <= 0) return;
        
        // 增加分数
        currentScore += points;
        
        // 触发分数变化事件
        // OnScoreChanged?.Invoke(currentScore);
        this.TriggerEvent(EventName.scoreChanged, new ScoreEventArgs{score = currentScore});
        
        // 检查是否为最高分
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            
            // 触发最高分变化事件
            // OnHighScoreChanged?.Invoke(highScore);
            this.TriggerEvent(EventName.highScoreChanged, new ScoreEventArgs { score = highScore });
        }
    }
    
    /// <summary>
    /// 根据水母等级计算得分
    /// </summary>
    /// <param name="level">合并后的水母等级</param>
    /// <returns>得分</returns>
    public int CalculateScoreForLevel(int level)
    {
        if (level <= 1) return 0;
        
        // 按照指数增长计算分数: 2^(level-1)
        return (int)Mathf.Pow(2, level - 1);
    }
    
    /// <summary>
    /// 重置分数
    /// </summary>
    private void ResetScore()
    {
        currentScore = 0;
        this.TriggerEvent(EventName.scoreChanged, new ScoreEventArgs{score = currentScore});

    }
    
    /// <summary>
    /// 获取当前分数
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// 获取最高分数
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }
    
    /// <summary>
    /// 保存最高分数到PlayerPrefs
    /// </summary>
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(Settings.PLAYER_PREFS_HIGH_SCORE, highScore);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 从PlayerPrefs加载最高分数
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(Settings.PLAYER_PREFS_HIGH_SCORE, 0);
    }

    /// <summary>
    /// 触发游戏结束事件
    /// </summary>
    public void TriggerGameOver()
    {
        isGameOver = true;
        currentState = GameState.GameOver;
        this.TriggerEvent(EventName.gameOver);
    }
}
