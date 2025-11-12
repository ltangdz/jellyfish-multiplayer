using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 使用DOTween插件

public class JellyfishController : MonoBehaviour
{
    [Header("水母属性")]
    [SerializeField] private int level = 1; // 水母等级
    [SerializeField] private GameObject nextLevelPrefab; // 下一级水母预制体
    
    [Header("合成设置")]
    [SerializeField] private float mergeAnimationDuration = 0.1f; // 合成动画持续时间
    [SerializeField] private float shrinkScale = 0.3f; // 缩小至原来的比例
    [SerializeField] private float expandScale = 1.2f; // 新水母初始放大比例
    
    [Header("音频设置")]
    public AudioClip mergeSoundEffect; // 合成音效
    [Range(0.1f, 10f)]
    [SerializeField] private float soundVolume = 5f; // 音效音量
    
    [Header("分数设置")]
    [SerializeField] private bool addScoreOnMerge = true; // 是否在合成时增加分数
    
    private bool isBeingMerged = false; // 是否正在被合成
    private Rigidbody2D rb; // 刚体组件
    private Collider2D jellyfishCollider; // 碰撞体组件
    private SpriteRenderer spriteRenderer; // 精灵渲染器
    private AudioSource audioSource; // 音频源组件

    // 初始化
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jellyfishCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 确保有AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D音效
        }
        
        // 初始动画 - 从小变大
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        
        // 从UIManager获取音效音量
        UpdateSoundVolumeFromSettings();
    }

    // 从设置中更新音效音量
    private void UpdateSoundVolumeFromSettings()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            // 获取当前音效音量设置
            float sfxVolume = uiManager.GetSfxVolume();
            UpdateSoundVolume(sfxVolume);
        }
    }
    
    // 更新音效音量
    public void UpdateSoundVolume(float volume)
    {
        // 更新音效音量
        soundVolume = volume * 10f; // 转换为0-10范围
    }

    // 碰撞检测
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果正在被合成，则忽略碰撞
        if (isBeingMerged)
            return;
            
        // 获取碰撞对象的水母控制器
        JellyfishController otherJellyfish = collision.gameObject.GetComponent<JellyfishController>();
        
        // 如果碰撞对象是水母且等级相同
        if (otherJellyfish != null && otherJellyfish.GetLevel() == level && !otherJellyfish.IsBeingMerged())
        {
            // 设置两个水母都为"正在被合成"状态
            isBeingMerged = true;
            otherJellyfish.SetBeingMerged(true);
            
            // 关闭物理效果，防止进一步碰撞
            DisablePhysics();
            otherJellyfish.DisablePhysics();
            
            // 执行合成动画
            StartMergeAnimation(otherJellyfish);
        }
    }

    // 开始合成动画
    private void StartMergeAnimation(JellyfishController otherJellyfish)
    {
        // 计算两个水母的中点作为新水母的生成位置
        Vector3 mergePosition = (transform.position + otherJellyfish.transform.position) / 2f;
        
        // 播放合成音效 - 在动画开始时就播放
        PlayMergeSound();
        
        // 两个水母向中心点靠拢并缩小
        transform.DOMove(mergePosition, mergeAnimationDuration * 0.7f).SetEase(Ease.InQuad);
        transform.DOScale(Vector3.one * shrinkScale, mergeAnimationDuration * 0.7f).SetEase(Ease.InQuad);
        
        otherJellyfish.transform.DOMove(mergePosition, mergeAnimationDuration * 0.7f).SetEase(Ease.InQuad);
        otherJellyfish.transform.DOScale(Vector3.one * shrinkScale, mergeAnimationDuration * 0.7f).SetEase(Ease.InQuad);
        
        // 动画完成后生成新水母
        DOVirtual.DelayedCall(mergeAnimationDuration * 0.7f, () => {
            // 如果有下一级水母预制体，则生成
            if (nextLevelPrefab != null)
            {
                // 生成新水母
                GameObject newJellyfish = Instantiate(nextLevelPrefab, mergePosition, Quaternion.identity);
                
                // 设置新水母的初始缩放并播放放大动画
                newJellyfish.transform.localScale = Vector3.one * expandScale;
                newJellyfish.transform.DOScale(1f, mergeAnimationDuration * 0.5f).SetEase(Ease.OutBack);
                
                // 获取新水母的控制器
                JellyfishController newController = newJellyfish.GetComponent<JellyfishController>();
                if (newController != null)
                {
                    // 增加分数
                    AddScoreForMerge(newController.GetLevel());
                }
            }
            
            // 销毁原来的两个水母
            Destroy(otherJellyfish.gameObject);
            Destroy(gameObject);
        });
    }
    
    // 增加合成分数
    private void AddScoreForMerge(int newLevel)
    {
        if (!addScoreOnMerge) return;
        
        // 获取GameManager实例
        if (GameManager.Instance != null)
        {
            // 计算并添加分数
            int scoreToAdd = GameManager.Instance.CalculateScoreForLevel(newLevel);
            GameManager.Instance.AddScore(scoreToAdd);
            
            // 在合并位置显示得分文本（可选）
            ShowScoreText(scoreToAdd);
        }
    }
    
    // 显示得分文本（可选功能）
    private void ShowScoreText(int score)
    {
        // 这里可以实现一个浮动的分数文本效果
        // 例如：生成一个临时UI文本，显示"+分数"，然后向上飘动并淡出
    }
    
    // 播放合成音效
    private void PlayMergeSound()
    {
        if (mergeSoundEffect != null)
        {
            // 创建一个独立的游戏对象来播放音效
            GameObject audioObj = new GameObject("MergeSound");
            audioObj.transform.position = transform.position;
            
            // 添加AudioSource组件并设置参数
            AudioSource audioSrc = audioObj.AddComponent<AudioSource>();
            audioSrc.clip = mergeSoundEffect;
            audioSrc.volume = soundVolume;
            audioSrc.spatialBlend = 0f; // 2D音效
            audioSrc.Play();
            
            // 播放完成后销毁游戏对象
            Destroy(audioObj, mergeSoundEffect.length + 0.1f);
            
            Debug.Log("播放合成音效: " + mergeSoundEffect.name + "，音量: " + soundVolume);
        }
        else
        {
            Debug.LogWarning("未设置合成音效");
        }
    }
    
    // 禁用物理组件
    public void DisablePhysics()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true;
        }
        
        if (jellyfishCollider != null)
        {
            jellyfishCollider.enabled = false;
        }
    }
    
    // 启用物理组件
    public void EnablePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        if (jellyfishCollider != null)
        {
            jellyfishCollider.enabled = true;
        }
    }
    
    // 获取水母等级
    public int GetLevel()
    {
        return level;
    }
    
    // 检查水母是否正在被合成
    public bool IsBeingMerged()
    {
        return isBeingMerged;
    }
    
    // 设置水母正在被合成状态
    public void SetBeingMerged(bool state)
    {
        isBeingMerged = state;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
