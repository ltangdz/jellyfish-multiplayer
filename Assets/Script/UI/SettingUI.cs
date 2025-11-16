using System;
using System.Collections;
using System.Collections.Generic;
using Script.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button restartButton;      // 重新开始
    [SerializeField] private Button mainMenuButton;     // 主菜单
    [SerializeField] private Button exitButton;         // 退出游戏
    [SerializeField] private Button closeButton;        // 关闭UI
    
    [SerializeField] private Slider musicVolumeSlider;   // 音乐音量滑动条
    [SerializeField] private TextMeshProUGUI musicVolumeText;       // 音乐音量百分比文本
    [SerializeField] private Slider sfxVolumeSlider;     // 音效音量滑动条
    [SerializeField] private TextMeshProUGUI sfxVolumeText;         // 音效音量百分比文本
    
    private void Awake()
    {
        SetButtonEvents();

        // 监听暂停和取消暂停事件
        EventManager.Instance.AddListener(EventName.gamePaused, (sender, args) =>
        {
            Show();
        });
        EventManager.Instance.AddListener(EventName.gameUnpaused, (sender, args) =>
        {
            Hide();
        });
    }
    

    private void SetButtonEvents()
    {
        restartButton.onClick.AddListener(() =>
        {
            Loader.Load(Settings.GameScene);
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Settings.MainMenuScene);
        });
        
        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        
        closeButton.onClick.AddListener(() =>
        {
            this.TriggerEvent(EventName.gameUnpaused);
        });
    }

    private void Start()
    {
        SetAudioSfxVolumeEvents();
        
        Hide();
    }

    /// <summary>
    /// 设置音量和音效 滑动条和文本的事件
    /// </summary>
    private void SetAudioSfxVolumeEvents()
    {
        float musicVolume = AudioManager.Instance.Volume;
        musicVolumeSlider.value = musicVolume;
        musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100) + "%";
        musicVolumeSlider.onValueChanged.AddListener((newVolume) =>
        {
            AudioManager.Instance.ChangeVolume(newVolume);
            musicVolumeText.text = Mathf.RoundToInt(newVolume * 100) + "%";
        });
        
        float sfxVolume = SoundManager.Instance.Volume;
        sfxVolumeSlider.value = sfxVolume;
        sfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100) + "%";
        sfxVolumeSlider.onValueChanged.AddListener((newVolume) =>
        {
            SoundManager.Instance.ChangeVolume(newVolume);
            sfxVolumeText.text = Mathf.RoundToInt(newVolume * 100) + "%";
        });
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.gamePaused, (sender, args) =>
        {
            Show();
        });
        EventManager.Instance.RemoveListener(EventName.gameUnpaused, (sender, args) =>
        {
            Hide();
        });
        
        Time.timeScale = 1;
    }

    private void Hide()
    {
        Time.timeScale = 1;

        gameObject.SetActive(false);
    }

    private void Show()
    {
        Time.timeScale = 0;
        
        gameObject.SetActive(true);
    }
}
