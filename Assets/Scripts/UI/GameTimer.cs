using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameDuration = 180f;

    // References
    private PlayerStats playerStats;
    private StateMachine playerStateMachine;

    // Timer state
    private float timeRemaining;
    private bool isTimerRunning = false;

    // Getters
    public float TimeRemaining => timeRemaining;
    public bool IsTimerRunning => isTimerRunning;
    public float GameDuration => gameDuration;


    private void Start()
    {
        timeRemaining = gameDuration;
        playerStats = FindObjectOfType<PlayerStats>();
        playerStateMachine = FindObjectOfType<StateMachine>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in scene!");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("Timer Text not assigned to GameTimer!");
            return;
        }

        // Start timer when player starts moving
        if (playerStateMachine != null)
        {
            StartCoroutine(WaitForGameStart());
        }

        UpdateTimerDisplay();
    }

    private System.Collections.IEnumerator WaitForGameStart()
    {
        // wait until we enter flying state
        while (!playerStateMachine.IsInState<FlyingState>())
        {
            yield return null;
        }

        isTimerRunning = true;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerDisplay();

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isTimerRunning = false;
            TriggerGameWon();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void TriggerGameWon()
    {
        if (playerStats != null)
        {
            playerStats.SetGameWon(true);
        }
    }

    // Call this when restarting the game
    public void ResetTimer()
    {
        timeRemaining = gameDuration;
        isTimerRunning = false;
        UpdateTimerDisplay();
    }
}