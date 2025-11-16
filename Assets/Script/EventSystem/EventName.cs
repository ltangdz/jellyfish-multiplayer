/// <summary>
/// 静态事件名称类
/// </summary>
public static class EventName
{
    // public static string MyEvent = nameof(MyEvent);

    #region GameManager
    
    public static string scoreChanged = nameof(scoreChanged);
    public static string highScoreChanged = nameof(highScoreChanged);
    public static string gamePaused = nameof(gamePaused);
    public static string gameUnpaused = nameof(gameUnpaused);
    public static string gameStart = nameof(gameStart);
    public static string gameOver = nameof(gameOver);
    
    
    #endregion

    #region GameInput

    public static string trailPreviewed = nameof(trailPreviewed);
    public static string jellyfishDropped = nameof(jellyfishDropped);

    #endregion
    
}
