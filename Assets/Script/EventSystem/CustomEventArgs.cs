using System;
using UnityEngine;

/// <summary>
/// 自定义事件参数类
/// </summary>
public class CustomEventArgs : MonoBehaviour
{

}

// /// <summary>
// /// 模板
// /// </summary>
// public class MyEventArgs : EventArgs
// {
//     
// }

/// <summary>
/// 自定义分数事件参数
/// </summary>
public class ScoreEventArgs : EventArgs
{
    public int score;
}
