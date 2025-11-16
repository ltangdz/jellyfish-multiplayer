/// <summary>
/// 设置静态类
/// </summary>
public static class Settings
{
    #region Scene Name

    public static string MainMenuScene = nameof(MainMenuScene);
    public static string GameScene = nameof(GameScene);

    #endregion

    #region PlayerPrefs

    // 分数
    public const string PLAYER_PREFS_HIGH_SCORE = "HighScore";
    
    // 音量键
    public const string PLAYER_PREFS_MUSIC_VOLUME_KEY = "MusicVolume";
    public const string PLAYER_PREFS_SFX_VOLUME_KEY = "SfxVolume";

    #endregion

    #region InputActions

    public const string INPUTACTION_MOVELEFT = "Player/MoveLeft";
    public const string INPUTACTION_MOVERIGHT = "Player/MoveRight";
    public const string INPUTACTION_PREVIEW = "Player/Preview";
    public const string INPUTACTION_DROP = "Player/Drop";

    #endregion
}
