using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject youWonPanel;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Button submitScoreButton;

    private ScoreManager scoreManager;
    private IGameStats playerStats;

    // getters
    public GameObject YouWonPanel => youWonPanel;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        playerStats = FindObjectOfType<PlayerStats>();

        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.AddListener(SubmitScore);
        }

        if (playerStats != null)
        {
            playerStats.OnGameWon += ShowWinPanel;
        }
    }

    private void ShowWinPanel()
    {
        if (youWonPanel != null)
        {
            youWonPanel.SetActive(true);
            ShowHighScores();
        }
    }

    private void Update()
    {
        if (youWonPanel != null && playerStats != null)
        {
            youWonPanel.SetActive(playerStats.IsGameWon);
        }
    }

    private void SubmitScore()
    {
        if (playerNameInput == null || scoreManager == null)
        {
            Debug.LogError("ScoreboardUI: Missing references.");
            return;
        }

        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogError("ScoreboardUI: Player name is empty.");
            return;
        }

        scoreManager.AddScore(playerNameInput.text, playerStats.Score);
        playerNameInput.interactable = false;
        submitScoreButton.interactable = false;

        youWonPanel.SetActive(false);
        ShowHighScores();
    }

    public void ShowHighScores()
    {
        if (scoreManager == null || highScoreText == null)
        {
            Debug.LogError("ScoreboardUI: Missing references.");
            return;
        }

        var scores = scoreManager.GetHighScores();
        string highScoreString = "High Scores\n";

        foreach (var score in scores)
        {
            highScoreString += $"{score.playerName}: {score.score}\n";
        }

        highScoreText.text = highScoreString;
    }

    public void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnGameWon -= ShowWinPanel;
        }
    }
}
