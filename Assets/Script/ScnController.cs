using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JellyfishItem
{
    public Sprite jellyfishImage;            // 水母图像
    public GameObject jellyfishPrefab;       // 水母预制体
    public float mechanicalArmAngle = 0f;    // 机械臂角度
    public string jellyfishName = "水母";     // 水母名称
}

public class ScnController : MonoBehaviour
{
    [Header("机械臂控制")]
    [SerializeField] private Transform mechanicalClawTransform;  // 机械臂的Transform
    [SerializeField] private HeaderController armController;     // 机械臂角度控制器
    [SerializeField] private Transform imageTransform;           // 水母图像显示位置
    [SerializeField] private SpriteRenderer jellyfishRenderer;   // 水母图像渲染器
    
    [Header("水母设置")]
    [SerializeField] private JellyfishItem[] jellyfishItems;     // 水母数据数组
    [SerializeField] private Transform jellyfishContainer;       // 水母容器（生成的水母的父对象）
    [SerializeField] private float spawnDelay = 1f;              // 生成延迟时间
    
    [Header("UI引用")]
    [SerializeField] private UIManager uiManager;                // UI管理器引用
    
    private Camera mainCamera;
    private int currentJellyfishIndex = -1;   // 当前水母索引
    private int nextJellyfishIndex = -1;      // 下一个水母索引
    private bool canClick = true;            // 是否可以点击
    
    // 初始化
    void Start()
    {
        // 获取主相机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("找不到主相机！请确保场景中有标记为MainCamera的相机。");
        }
        
        // 检查机械臂Transform
        if (mechanicalClawTransform == null)
        {
            Debug.LogError("未设置机械臂Transform！请在Inspector中设置。");
        }
        
        // 检查机械臂角度控制器
        if (armController == null)
        {
            // 尝试从场景中查找机械臂控制器
            armController = GameObject.FindObjectOfType<HeaderController>();
            if (armController == null)
            {
                Debug.LogWarning("未找到机械臂角度控制器！请确保场景中有HeaderController组件。");
            }
        }
        
        // 检查水母渲染器
        if (jellyfishRenderer == null)
        {
            Debug.LogWarning("未设置水母图像渲染器！请在Inspector中设置。");
        }
        
        // 检查水母容器
        if (jellyfishContainer == null)
        {
            Debug.LogWarning("未设置水母容器！请在Inspector中设置。");
        }
        
        // 检查UI管理器
        if (uiManager == null)
        {
            // 尝试从场景中查找UI管理器
            uiManager = GameObject.FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("未找到UIManager！水母预览功能可能无法正常工作。");
            }
        }
        
        // 检查水母数据
        if (jellyfishItems == null || jellyfishItems.Length == 0)
        {
            Debug.LogError("未设置水母数据！请在Inspector中设置。");
        }
        else
        {
            // 初始化当前水母和下一个水母
            InitializeJellyfish();
        }
    }
    
    // 初始化水母
    private void InitializeJellyfish()
    {
        if (jellyfishItems == null || jellyfishItems.Length == 0)
            return;
            
        // 随机选择当前水母
        currentJellyfishIndex = Random.Range(0, jellyfishItems.Length);
        
        // 选择下一个水母（确保与当前不同，除非只有一种水母）
        SelectNextJellyfish();
        
        // 更新当前水母图像和机械臂角度
        UpdateJellyfishImage();
        UpdateMechanicalArmAngle();
        ShowJellyfishImage(true);
        
        // 更新UI预览（显示下一个水母）
        UpdateUIPreview();
    }
    
    // 选择下一个水母
    private void SelectNextJellyfish()
    {
        if (jellyfishItems == null || jellyfishItems.Length == 0)
            return;
            
        // 随机选择一个水母索引，确保与当前索引不同（如果可能）
        if (jellyfishItems.Length == 1)
        {
            nextJellyfishIndex = 0;
        }
        else
        {
            do
            {
                nextJellyfishIndex = Random.Range(0, jellyfishItems.Length);
            }
            while (nextJellyfishIndex == currentJellyfishIndex);
        }
        
        // 更新UI预览
        UpdateUIPreview();
    }
    
    // 更新水母图像
    private void UpdateJellyfishImage()
    {
        if (jellyfishRenderer != null && currentJellyfishIndex >= 0 && currentJellyfishIndex < jellyfishItems.Length)
        {
            jellyfishRenderer.sprite = jellyfishItems[currentJellyfishIndex].jellyfishImage;
        }
    }
    
    // 显示或隐藏水母图像
    private void ShowJellyfishImage(bool show)
    {
        if (jellyfishRenderer != null)
        {
            jellyfishRenderer.enabled = show;
        }
    }
    
    // 更新机械臂角度
    private void UpdateMechanicalArmAngle()
    {
        if (armController != null && currentJellyfishIndex >= 0 && currentJellyfishIndex < jellyfishItems.Length)
        {
            // 使用HeaderController的方法设置机械臂角度
            armController.SetArmRotation(jellyfishItems[currentJellyfishIndex].mechanicalArmAngle);
        }
    }
    
    // 更新UI预览
    private void UpdateUIPreview()
    {
        if (uiManager != null && nextJellyfishIndex >= 0 && nextJellyfishIndex < jellyfishItems.Length)
        {
            // 获取下一个水母数据
            JellyfishItem nextItem = jellyfishItems[nextJellyfishIndex];
            
            // 更新UI预览，显示下一个水母
            uiManager.UpdateNextJellyfishPreview(nextItem.jellyfishImage, nextItem.jellyfishName);
        }
    }
    
    /// <summary>
    /// 在机械臂位置生成水母 - 供机械臂控制器调用
    /// </summary>
    public void SpawnJellyfishAtArm()
    {
        // 如果正在处理点击或无法点击，则忽略
        if (!canClick)
            return;
            
        // 启动生成水母的协程
        StartCoroutine(SpawnJellyfishCoroutine());
    }
    
    // 生成水母协程
    private IEnumerator SpawnJellyfishCoroutine()
    {
        // 禁止点击
        canClick = false;
        
        // 如果当前没有有效的水母索引，直接返回
        if (currentJellyfishIndex < 0 || currentJellyfishIndex >= jellyfishItems.Length || mechanicalClawTransform == null)
        {
            yield return new WaitForSeconds(spawnDelay);
            canClick = true;
            yield break;
        }
        
        // 获取当前水母预制体
        GameObject prefab = jellyfishItems[currentJellyfishIndex].jellyfishPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("当前水母预制体为空！");
            yield return new WaitForSeconds(spawnDelay);
            canClick = true;
            yield break;
        }
        
        // 在生成水母之前隐藏机械臂上的水母图像
        ShowJellyfishImage(false);
        
        // 计算生成位置（与图像变换位置相同的X坐标）
        Vector3 spawnPosition;
        if (imageTransform != null)
        {
            // 使用图像变换的位置，但与机械臂的X坐标保持一致
            spawnPosition = new Vector3(
                mechanicalClawTransform.position.x,
                imageTransform.position.y,
                imageTransform.position.z
            );
        }
        else
        {
            // 直接在机械臂下方生成
            spawnPosition = new Vector3(
                mechanicalClawTransform.position.x,
                mechanicalClawTransform.position.y - 1f,
                mechanicalClawTransform.position.z
            );
        }
        
        // 生成水母预制体
        GameObject jellyfish = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        // 如果指定了容器，将水母设为容器的子对象
        if (jellyfishContainer != null)
        {
            jellyfish.transform.SetParent(jellyfishContainer);
        }
        
        // 等待指定时间
        yield return new WaitForSeconds(spawnDelay);
        
        // 将下一个水母设为当前水母
        currentJellyfishIndex = nextJellyfishIndex;
        
        // 更新当前水母图像和机械臂角度
        UpdateJellyfishImage();
        UpdateMechanicalArmAngle();
        ShowJellyfishImage(true);
        
        // 选择新的下一个水母
        SelectNextJellyfish();
        
        // 允许点击
        canClick = true;
    }
    
    // 在编辑器中绘制生成位置
    private void OnDrawGizmos()
    {
        // 如果设置了图像变换，绘制生成位置
        if (imageTransform != null && mechanicalClawTransform != null)
        {
            Gizmos.color = Color.blue;
            Vector3 spawnPos = new Vector3(
                mechanicalClawTransform.position.x,
                imageTransform.position.y,
                imageTransform.position.z
            );
            Gizmos.DrawSphere(spawnPos, 0.3f);
        }
    }
}
