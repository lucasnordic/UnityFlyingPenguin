public interface IGameStats
{
    int Score { get; }
    int Life { get; }
    int MaxLife { get; }
    bool IsGameWon { get; }
    event System.Action<int> OnScoreChanged;
    event System.Action<int> OnLifeChanged;
    event System.Action OnGameOver;
    event System.Action OnGameWon;
}