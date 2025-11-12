using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // 单例实例
    public static GameManager Instance { get; private set; }
    
    [Header("游戏数据")]
    [SerializeField] private int currentScore = 0;   // 当前分数
    [SerializeField] private int highScore = 0;      // 最高分数
    
    // 事件 - 分数变化时触发
    public event Action<int> OnScoreChanged;
    // 事件 - 最高分数变化时触发
    public event Action<int> OnHighScoreChanged;
    
    // 初始化
    void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 从PlayerPrefs加载最高分
        LoadHighScore();
    }
    
    void Start()
    {
        // 初始化分数
        ResetScore();
    }
    
    /// <summary>
    /// 增加分数
    /// </summary>
    /// <param name="points">要增加的分数</param>
    public void AddScore(int points)
    {
        if (points <= 0) return;
        
        // 增加分数
        currentScore += points;
        
        // 触发分数变化事件
        OnScoreChanged?.Invoke(currentScore);
        
        // 检查是否为最高分
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            
            // 触发最高分变化事件
            OnHighScoreChanged?.Invoke(highScore);
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
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
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
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 从PlayerPrefs加载最高分数
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
