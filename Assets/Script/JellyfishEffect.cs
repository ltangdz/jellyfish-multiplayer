using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class JellyfishEffect : MonoBehaviour
{
    [SerializeField]
    private Material jellyfishMaterial; // 在Inspector中指定材质
    private SpriteRenderer spriteRenderer;

    [Header("涟漪设置")]
    [Range(0.1f, 10f)]
    public float rippleSpeed = 2.0f; // 控制涟漪速度
    [Range(0.001f, 0.1f)]
    public float rippleAmount = 0.01f; // 控制涟漪强度
    [Range(0f, 0.1f)]
    public float waveIntensity = 0.02f; // 控制波动强度

    [Header("触须设置")]
    [Range(0.1f, 10f)]
    public float tentacleSwaySpeed = 3.0f; // 控制触须摆动速度
    [Range(0.01f, 0.5f)]
    public float tentacleSwayAmount = 0.1f;  // 控制触须摆动幅度

    [Header("颜色设置")]
    public Color jellyfishColor = Color.white; // 控制颜色

    void OnEnable()
    {
        // 确保材质在启用时被创建（编辑模式和运行模式）
        InitializeMaterial();
    }
    
    void Start()
    {
        InitializeMaterial();
    }

    void Update()
    {
        // 实时更新材质属性（编辑模式和运行模式）
        UpdateMaterialProperties();
    }
    
    // 初始化材质
    void InitializeMaterial()
    {
        // 检查SpriteRenderer是否已存在
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null && spriteRenderer.sprite != null && jellyfishMaterial != null)
        {
            // 将原始贴图应用到材质
            jellyfishMaterial.SetTexture("_MainTex", spriteRenderer.sprite.texture);
            
            // 应用材质到精灵渲染器
            spriteRenderer.material = jellyfishMaterial;
            
            // 初始化设置
            UpdateMaterialProperties();
        }
    }
    
    // 当Inspector中的值发生变化时调用
    void OnValidate()
    {
        // 确保在编辑器中修改值时更新效果
        if (spriteRenderer != null && jellyfishMaterial != null)
            UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (jellyfishMaterial != null)
        {
            jellyfishMaterial.SetFloat("_RippleSpeed", rippleSpeed);
            jellyfishMaterial.SetFloat("_RippleAmount", rippleAmount);
            jellyfishMaterial.SetFloat("_WaveIntensity", waveIntensity);
            jellyfishMaterial.SetFloat("_TentacleSwaySpeed", tentacleSwaySpeed);
            jellyfishMaterial.SetFloat("_TentacleSwayAmount", tentacleSwayAmount);
            jellyfishMaterial.SetColor("_Color", jellyfishColor);
        }
    }
} 