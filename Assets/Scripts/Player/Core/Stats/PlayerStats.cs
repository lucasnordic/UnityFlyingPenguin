using UnityEngine;

public class PlayerStats : MonoBehaviour, IGameStats
{
    [Header("Initial Values")]
    [SerializeField] private int maxLife = 3;
    [SerializeField] private int startingScore = 0;

    private int score;
    private int life;
    private bool isGameWon;

    // getters and setters
    public int Score => score;
    public int Life => life;
    public int MaxLife => maxLife;
    public bool IsGameWon => isGameWon;

    // events
    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnLifeChanged;
    public event System.Action OnGameOver;
    public event System.Action OnGameWon;

    private void Start()
    {
        ResetStats();
    }

    public void SetGameWon(bool value)
    {
        isGameWon = value;
        OnGameWon?.Invoke();
    }

    public void ResetStats()
    {
        score = startingScore;
        life = maxLife;
        isGameWon = false;
        OnScoreChanged?.Invoke(score);
        OnLifeChanged?.Invoke(life);
    }

    public void AddScore(int amount = 1)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public void LoseLife(int amount = 1)
    {
        life -= amount;
        OnLifeChanged?.Invoke(life);

        if (life <= 0)
        {
            OnGameOver?.Invoke();
        }
    }

    public void GainLife (int amount = 1)
    {
        life += amount;
        OnLifeChanged?.Invoke(Life);
    }
}