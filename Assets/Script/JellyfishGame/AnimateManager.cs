using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnimateManager : Singleton<AnimateManager>
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
    [SerializeField] private float soundDelayTime = 0.5f;
    
    private TextMeshProUGUI countdownText; // 倒计时文本
    [Header("UI设置")]
    [SerializeField] private SpriteRenderer fadeSprite; // 需要渐隐的2D精灵
    [SerializeField] private float countdownDuration = 1.0f; // 每个数字显示时长
    [SerializeField] private float uiFadeInDuration = 0.5f; // UI淡入时长
    [SerializeField] private float imageFadeOutDuration = 1.0f; // 图像淡出时长
    
    private void Start()
    {
        // 初始化位置
        mainCamera.transform.position = cameraStartPosition;
        
        targetGameObject.position = startTransform.position;
        targetGameObject.rotation = startTransform.rotation;
        
        // 初始化UI
        InitializeUI();
        
        // 开始动画
        PlayEntranceAnimation();
    }
    
    private void InitializeUI()
    {
        // 隐藏主UI
        UIManager.Instance.HideMainUI();
        
        countdownText = UIManager.Instance.CountDownUI.countDownText; 
        // 隐藏倒计时UI
        UIManager.Instance.HideCountDownUI();
        
        // 设置渐隐精灵为完全不透明
        Color color = fadeSprite.color;
        color.a = 1f;
        fadeSprite.color = color;
        fadeSprite.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 播放倒计时动画
    /// </summary>
    private void PlayEntranceAnimation()
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
        mainCamera.transform.DOMove(cameraEndPosition, cameraMoveTime)
            .SetEase(cameraEaseType)
            .OnComplete(() => {
                Debug.Log("摄像机动画完成");
            });
    }
    
    private void PlayObjectAnimation()
    {
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
        // 延迟播放音效
        DOVirtual.DelayedCall(soundDelayTime, () => {
            // audioSource.PlayOneShot(entranceSound);
            SoundManager.Instance.PlaySound(AudioManager.Instance.AudioClipRefsSO.entranceSound);
        });
    }
    
    private void StartCountdownAnimation()
    {
        // 显示倒计时UI
        // countdownText.gameObject.SetActive(true);
        UIManager.Instance.ShowCountDownUI();
        
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
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 1
        countdownSequence.AppendCallback(() => {
            countdownText.text = "1";
            countdownText.transform.localScale = Vector3.zero;
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 开始
        countdownSequence.AppendCallback(() => {
            countdownText.text = "开始!";
            countdownText.transform.localScale = Vector3.zero;
        });
        countdownSequence.Append(countdownText.transform.DOScale(1.5f, countdownDuration * 0.5f)
            .SetEase(Ease.OutBack));
        countdownSequence.Append(countdownText.transform.DOScale(1f, countdownDuration * 0.5f)
            .SetEase(Ease.InBack));
        
        // 倒计时结束后显示游戏UI
        // 切换游戏状态
        countdownSequence.OnComplete(() => {
            // 销毁倒计时文本
            // countdownText.gameObject.SetActive(false);
            UIManager.Instance.HideCountDownUI();
            
            // 显示主UI
            ShowMainUI();
        });
    }
    
    private void ShowMainUI()
    {
        // 渐显游戏UI
        UIManager.Instance.ShowMainUI();
        
        // 获取所有CanvasGroup组件
        CanvasGroup canvasGroup = UIManager.Instance.MainUI.GetComponent<CanvasGroup>();
        
        // 初始设置为透明
        canvasGroup.alpha = 0f;
        
        // 延迟渐显，每个UI元素有一点延迟
        const float delay = 0.1f;
        canvasGroup.DOFade(1f, uiFadeInDuration)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad);
        
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
}