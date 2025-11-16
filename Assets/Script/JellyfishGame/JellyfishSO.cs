using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JellyfishSO : ScriptableObject
{
    public Sprite jellyfishImage;            // 水母图像
    public GameObject jellyfishPrefab;       // 水母预制体
    public float mechanicalArmAngle;    // 机械臂角度
    public string jellyfishName;     // 水母名称    
}