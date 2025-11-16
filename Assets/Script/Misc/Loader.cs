using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载静态类
/// </summary>
public static class Loader
{
    /// <summary>
    /// 客户端本地 场景加载
    /// </summary>
    /// <param name="targetScene"></param>
    public static void Load(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    /// <summary>
    /// 联机游戏 场景加载 只有Server可以调用
    /// </summary>
    /// <param name="targetScene"></param>
    public static void LoadNetwork(string targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
