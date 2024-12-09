using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: Refactor this script. It is not following the Single Responsibility Principle.
public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text lifeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private GameObject escPanel;
    [SerializeField] private GameObject parchmentImage;

    private StateMachine playerStateMachine;
    private IGameStats playerStats;
    private ScoreboardUI scoreboardUI;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerStateMachine = FindObjectOfType<StateMachine>();
        scoreboardUI = FindObjectOfType<ScoreboardUI>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStatsUI: PlayerStats not found in scene.");
            return;
        }

        // Subscribe to events
        playerStats.OnScoreChanged += UpdateScoreUI;
        playerStats.OnLifeChanged += UpdateLifeUI;
        playerStats.OnGameOver += ShowGameOver;
        playerStats.OnGameWon += PauseGame;

        // Hide panels & UI elements
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (escPanel != null) escPanel.SetActive(false);
        if (parchmentImage != null) parchmentImage.SetActive(true);

        // Update UI
        UpdateScoreUI(playerStats.Score);
        UpdateLifeUI(playerStats.Life);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // if flight state is active, hide parchment image
        if (playerStateMachine != null && playerStateMachine.IsInState<FlyingState>())
        {
            if (parchmentImage != null) parchmentImage.SetActive(false);
        }

        // Check for restart input when game over panel is shown
        if (gameOverPanel != null && gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }

        // check for escape input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playerStats.IsGameWon && scoreboardUI != null && scoreboardUI.YouWonPanel.activeSelf)
            {
                RestartGame(); // Restart game if player won and game won panel is active
            }
            else if (escPanel != null) // Show/hide escape panel
            {
                escPanel.SetActive(!escPanel.activeSelf);
                Time.timeScale = escPanel.activeSelf ? 0f : 1f;
            }
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null) gameOverScoreText.text = $"Final Score: {playerStats.Score}";
        }

        Time.timeScale = 0f; // Pause the game
    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            Debug.LogError("GameManager not found in scene.");
        }
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText == null) return;
        scoreText.text = $"Score: {score}";
    }

    private void UpdateLifeUI(int life)
    {
        if (scoreText == null) return;
        lifeText.text = $"Life: {life}";
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnScoreChanged -= UpdateScoreUI;
            playerStats.OnLifeChanged -= UpdateLifeUI;
            playerStats.OnGameOver -= ShowGameOver;
            playerStats.OnGameWon -= PauseGame;
        }
    }
}
