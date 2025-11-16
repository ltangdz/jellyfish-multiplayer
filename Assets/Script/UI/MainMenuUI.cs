using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class MainMenuUI : MonoBehaviour
{
    [FormerlySerializedAs("startGameButton")]
    [Header("UI引用")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private GameObject instructionsUI;
    
    [Header("动画设置")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float buttonPunchScale = 1.2f;
    
    private void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 显示最高分数
        UpdateHighScoreText();
    }
    
    private void InitializeUI()
    {
        startButton.onClick.AddListener(OnStartGameClicked);
        
        // 添加按钮动画
        startButton.transform.DOScale(1.05f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
            
        instructionsButton.onClick.AddListener(() => instructionsUI.SetActive(true));
        
        instructionsUI.SetActive(false);
    }
    
    private void OnStartGameClicked()
    {
        // 按钮点击动画
        startButton.transform.DOKill();
        startButton.transform.DOPunchScale(Vector3.one * buttonPunchScale, animationDuration, 5, 1f)
            .OnComplete(() => {
                // 动画完成后加载游戏场景
                SceneManager.LoadScene(Settings.GameScene);
            });
    }
    
    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"最高得分：{highScore}分";
            
            // 添加文本动画效果
            highScoreText.transform.DOScale(1.2f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    highScoreText.transform.DOScale(1f, 0.2f)
                        .SetEase(Ease.InBack);
                });
        }
    }
    
}
