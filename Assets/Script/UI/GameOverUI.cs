using System;
using System.Collections;
using System.Collections.Generic;
using Script.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        BindButtonActions();
    }

    private void BindButtonActions()
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
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.gameOver, (sender, args) =>
        {
            Show();
        });
        
        Hide();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.gameOver, (sender, args) =>
        {
            Show();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
