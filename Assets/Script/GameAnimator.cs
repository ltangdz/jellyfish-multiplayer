using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameAnimator : MonoBehaviour
{
    [Header("摄像机设置")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraStartPosition = new Vector3(0, 10, -10);
    [SerializeField] private Vector3 cameraEndPosition = new Vector3(0, 0, -10);
    [SerializeField] private float cameraMoveTime = 2.0f;
    [SerializeField] private Ease cameraEaseType = Ease.OutBack;
    
    [Header("游戏物体设置")]
    [SerializeField] private Transform targetGameObject;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private float objectMoveTime = 2.5f;
    [SerializeField] private Ease objectEaseType = Ease.OutBounce;
    
    [Header("音效设置")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip entranceSound;
    [SerializeField] private float soundDelay = 0.5f;
    
    [Header("UI设置")]
    [SerializeField] private GameObject[] gameUIObjects; // 游戏UI对象数组
    [SerializeField] private Text countdownText; // 倒计时文本
    [SerializeField] private SpriteRenderer fadeSprite; // 需要渐隐的2D精灵
    [SerializeField] private float countdownDuration = 1.0f; // 每个数字显示时长
    [SerializeField] private float uiFadeInDuration = 0.5f; // UI淡入时长
    [SerializeField] private float imageFadeOutDuration = 1.0f; // 图像淡出时长
    
    private void Start()
    {
        // 初始化位置
        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraStartPosition;
        }
        
        if (targetGameObject != null && startTransform != null)
        {
            targetGameObject.position = startTransform.position;
            targetGameObject.rotation = startTransform.rotation;
        }
        
        // 初始化UI
        InitializeUI();
        
        // 开始动画
        PlayEntranceAnimation();
    }
    
    private void InitializeUI()
    {
        // 隐藏所有游戏UI
        if (gameUIObjects != null)
        {
            foreach (GameObject uiObject in gameUIObjects)
            {
                if (uiObject != null)
                {
                    uiObject.SetActive(false);
                }
            }
        }
        
        // 隐藏倒计时文本
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        // 设置渐隐精灵为完全不透明
        if (fadeSprite != null)
        {
            Color color = fadeSprite.color;
            color.a = 1f;
            fadeSprite.color = color;
            fadeSprite.gameObject.SetActive(true);
        }
    }
    
    public void PlayEntranceAnimation()
    {
        // 播放摄像机动画
        PlayCameraAnimation();
        
        // 播放物体移动动画
        PlayObjectAnimation();
        
        // 播放音效
        PlayEntranceSound();
    }
    
    private void PlayCameraAnimation()
    {
        if (mainCamera == null)
        {
            Debug.LogError("未设置主摄像机！请在Inspector中分配引用。");
            return;
        }
        
        mainCamera.transform.DOMove(cameraEndPosition, cameraMoveTime)
            .SetEase(cameraEaseType)
            .OnComplete(() => {
                Debug.Log("摄像机动画完成");
            });
    }
    
    private void PlayObjectAnimation()
    {
        if (targetGameObject == null || endTransform == null)
        {
            Debug.LogError("未设置目标物体或终点变换！请在Inspector中分配引用。");
            return;
        }
        
        // 移动到目标位置
        targetGameObject.DOMove(endTransform.position, objectMoveTime)
            .SetEase(objectEaseType);
            
        // 旋转到目标朝向
        targetGameObject.DORotateQuaternion(endTransform.rotation, objectMoveTime)
            .SetEase(objectEaseType)
            .OnComplete(() => {
                Debug.Log("物体动画完成");
                
                // 开始倒计时动画
                StartCountdownAnimation();
            });
    }
    
    private void PlayEntranceSound()
    {
        if (audioSource == null || entranceSound == null)
        {
            Debug.LogWarning("未设置音频源或音效！请在Inspector中分配引用。");
            return;
        }
        
        // 延迟播放音效
        DOVirtual.DelayedCall(soundDelay, () => {
            audioSource.PlayOneShot(entranceSound);
        });
    }
    
    private void StartCountdownAnimation()
    {
        if (countdownText == null)
        {
            Debug.LogWarning("未设置倒计时文本！请在Inspector中分配引用。");
            ShowGameUI();
            return;
        }
        
        // 显示倒计时文本
        countdownText.gameObject.SetActive(true);
        
        // 开始3,2,1倒计时动画
        countdownText.text = "3";
        countdownText.transform.localScale = Vector3.zero;
        
        Sequence countdownSequence = DOTween.Sequence();
        
        // 3
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 2
        countdownSequence.AppendCallback(() => {
            countdownText.text = "2";
            countdownText.transform.localScale = Vector3.zero;
            
            // 播放音效
            if (audioSource != null)
            {
                audioSource.Play();
            }
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 1
        countdownSequence.AppendCallback(() => {
            countdownText.text = "1";
            countdownText.transform.localScale = Vector3.zero;
            
            // 播放音效
            if (audioSource != null)
            {
                audioSource.Play();
            }
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 开始
        countdownSequence.AppendCallback(() => {
            countdownText.text = "开始!";
            countdownText.transform.localScale = Vector3.zero;
            
            // 播放音效
            if (audioSource != null)
            {
                audioSource.Play();
            }
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 倒计时结束后显示游戏UI
        countdownSequence.OnComplete(() => {
            // 销毁倒计时文本
            countdownText.gameObject.SetActive(false);
            
            // 显示游戏UI
            ShowGameUI();
        });
    }
    
    private void ShowGameUI()
    {
        // 渐显游戏UI
        if (gameUIObjects != null)
        {
            for (int i = 0; i < gameUIObjects.Length; i++)
            {
                GameObject uiObject = gameUIObjects[i];
                if (uiObject != null)
                {
                    uiObject.SetActive(true);
                    
                    // 获取所有CanvasGroup组件
                    CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = uiObject.AddComponent<CanvasGroup>();
                    }
                    
                    // 初始设置为透明
                    canvasGroup.alpha = 0f;
                    
                    // 延迟渐显，每个UI元素有一点延迟
                    float delay = i * 0.1f;
                    canvasGroup.DOFade(1f, uiFadeInDuration)
                        .SetDelay(delay)
                        .SetEase(Ease.OutQuad);
                }
            }
        }
        
        // 渐隐2D精灵
        if (fadeSprite != null)
        {
            fadeSprite.DOFade(0f, imageFadeOutDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    fadeSprite.gameObject.SetActive(false);
                });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
