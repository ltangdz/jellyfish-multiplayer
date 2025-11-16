using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("UI")]
    [Tooltip("主UI (操作按钮 设置按钮等等)")]
    [SerializeField] private MainUI mainUI;
    public MainUI MainUI => mainUI;
    
    [Tooltip("设置界面UI")]
    [SerializeField] private SettingUI settingUI;
    
    [Tooltip("警告UI")]
    [SerializeField] private WarningUI warningUI;
    public WarningUI WarningUI => warningUI;
    
    [Tooltip("倒计时UI")]
    [SerializeField] private CountDownUI countDownUI;
    public CountDownUI CountDownUI => countDownUI;
    
    [Tooltip("游戏结束UI")]
    [SerializeField] private GameOverUI gameOverUI;
    
    public void HideCountDownUI()
    {
        countDownUI.Hide();
    }

    public void ShowCountDownUI()
    {
        countDownUI.Show();
    }

    public void HideMainUI()
    {
        mainUI.gameObject.SetActive(false);
    }

    public void ShowMainUI()
    {
        mainUI.gameObject.SetActive(true);
    }

    public void ShowWarningUI()
    {
        warningUI.Show();
    }

    public void HideWarningUI()
    {
        warningUI.Hide();
    }
}