using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// [System.Serializable]
// public class JellyfishItem
// {
//     public Sprite jellyfishImage;            // 水母图像
//     public GameObject jellyfishPrefab;       // 水母预制体
//     public float mechanicalArmAngle = 0f;    // 机械臂角度
//     public string jellyfishName = "水母";     // 水母名称
// }

/// <summary>
/// 管理水母生成
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{
    [Header("机械臂控制")]
    [SerializeField] private Transform mechanicalClawTransform;  // 机械臂的Transform
    [SerializeField] private HeaderController armController;     // 机械臂角度控制器
    [SerializeField] private Transform imageTransform;           // 水母图像显示位置
    [SerializeField] private SpriteRenderer jellyfishRenderer;   // 水母图像渲染器
    
    [FormerlySerializedAs("spawnJellyfisSOList")]
    [Header("水母设置")]
    // [SerializeField] private JellyfishItem[] jellyfishItems;     // 水母数据数组
    [SerializeField] private JellyfishSOList spawnJellyfishSOList;
    [SerializeField] private JellyfishSOList mergeJellyfishSOList;
    [SerializeField] private float spawnDelay = 1f;              // 生成延迟时间
    

    private int currentJellyfishIndex = -1;   // 当前水母索引
    private int nextJellyfishIndex = -1;      // 下一个水母索引
    private bool canClick = true;            // 是否可以点击
    public bool CanClick => canClick;

    private List<JellyfishSO> spawnJellyfishList;
    private int spawnJellyfishCount;
    private void Start()
    {
        InitializeJellyfish();
    }
    
    /// <summary>
    /// 初始化水母
    /// </summary>
    private void InitializeJellyfish()
    {
        spawnJellyfishList = spawnJellyfishSOList.JellyfishList;
        spawnJellyfishCount = spawnJellyfishList.Count;
        
        // 随机选择当前水母
        currentJellyfishIndex = Random.Range(0, spawnJellyfishCount);
        
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
        // 随机选择一个水母索引
        nextJellyfishIndex = Random.Range(0, spawnJellyfishCount);
        
        // 更新UI预览
        UpdateUIPreview();
    }
    
    // 更新水母图像
    private void UpdateJellyfishImage()
    {
        jellyfishRenderer.sprite = spawnJellyfishList[currentJellyfishIndex].jellyfishImage;
    }
    
    // 显示或隐藏水母图像
    private void ShowJellyfishImage(bool show)
    {
        jellyfishRenderer.enabled = show;
    }
    
    // 更新机械臂角度
    private void UpdateMechanicalArmAngle()
    {
        // 使用HeaderController的方法设置机械臂角度
        armController.SetArmRotation(spawnJellyfishList[currentJellyfishIndex].mechanicalArmAngle);
    }
    
    // 更新UI预览
    private void UpdateUIPreview()
    {
        // 获取下一个水母数据
        JellyfishSO jellyfishSO = spawnJellyfishList[nextJellyfishIndex];
        
        // 更新UI预览，显示下一个水母
        UIManager.Instance.MainUI.UpdateNextJellyfishPreview(jellyfishSO.jellyfishImage, jellyfishSO.jellyfishName);
    }
    
    /// <summary>
    /// 在机械臂位置生成水母 - 供机械臂控制器调用
    /// </summary>
    public void SpawnJellyfishAtArm()
    {
        // 如果正在处理点击或无法点击，则忽略
        if (!canClick) return;
            
        // 启动生成水母的协程
        StartCoroutine(SpawnJellyfishCoroutine());
    }
    
    // 生成水母协程
    private IEnumerator SpawnJellyfishCoroutine()
    {
        // 禁止点击
        canClick = false;
        
        // yield return new WaitForSeconds(spawnDelay);
        // canClick = true;
        
        // 获取当前水母预制体
        GameObject jellyfishPrefab = spawnJellyfishList[currentJellyfishIndex].jellyfishPrefab;
        
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
        GameObject jellyfish = Instantiate(jellyfishPrefab, spawnPosition, Quaternion.identity);
        
        // 将水母设为容器的子对象
        jellyfish.transform.SetParent(GameManager.Instance.JellyfishContainer);
        
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

    public GameObject GetNextLevelPrefab(int currLevel)
    {
        if(currLevel - 1 >= mergeJellyfishSOList.JellyfishList.Count) return null;
        
        JellyfishSO jellyfishSO = mergeJellyfishSOList.JellyfishList[currLevel - 1];
        return jellyfishSO.jellyfishPrefab;
    }
}
