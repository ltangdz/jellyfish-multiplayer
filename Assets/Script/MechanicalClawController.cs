using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class HeaderController : MonoBehaviour
{
    [Header("机械臂旋转控制")]
    [SerializeField] private Transform leftArm;  // 左臂
    [SerializeField] private Transform rightArm; // 右臂
    
    [Range(-180f, 180f)]
    [SerializeField] private float armRotationAngle = 0f; // 旋转角度
    
    private float lastArmRotationAngle;

    // 初始化
    void Start()
    {
        lastArmRotationAngle = armRotationAngle;
        UpdateArmRotation(); // 初始化时应用旋转
    }

    // 每帧更新
    void Update()
    {
        // 如果角度发生变化，则更新旋转
        if (!Mathf.Approximately(lastArmRotationAngle, armRotationAngle))
        {
            UpdateArmRotation();
            lastArmRotationAngle = armRotationAngle;
        }
    }
    
    // 当Inspector中的值发生变化时调用
    void OnValidate()
    {
        // 立即更新机械臂旋转
        UpdateArmRotation();
        lastArmRotationAngle = armRotationAngle;
    }
    
    // 更新机械臂旋转角度
    private void UpdateArmRotation()
    {
        if (leftArm != null)
        {
            Vector3 rotation = leftArm.localEulerAngles;
            leftArm.localEulerAngles = new Vector3(rotation.x, rotation.y, armRotationAngle);
        }
        
        if (rightArm != null)
        {
            Vector3 rotation = rightArm.localEulerAngles;
            rightArm.localEulerAngles = new Vector3(rotation.x, rotation.y, -armRotationAngle);
        }
    }
    
    /// <summary>
    /// 设置机械臂的旋转角度
    /// </summary>
    /// <param name="angle">旋转角度（-180到180之间）</param>
    public void SetArmRotation(float angle)
    {
        // 限制角度在有效范围内
        armRotationAngle = Mathf.Clamp(angle, -180f, 180f);
        UpdateArmRotation();
        lastArmRotationAngle = armRotationAngle;
    }
}
