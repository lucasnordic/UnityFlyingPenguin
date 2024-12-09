using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    // Component references
    private StateMachine playerStateMachine;
    private PlayerController playerController;
    private PlayerStats playerStats;
    private CountdownManager countdownManager;
    private PlayerEvents playerEvents;

    // Variables
    private bool isRespawning = false;

    private void Awake()
    {
        SetupReferences();
        ValidateReferences();
    }

    private void SetupReferences()
    {
        playerStateMachine = FindObjectOfType<StateMachine>();
        playerController = FindObjectOfType<PlayerController>();
        playerStats = FindObjectOfType<PlayerStats>();
        countdownManager = FindObjectOfType<CountdownManager>();
        playerEvents = FindObjectOfType<PlayerEvents>();
    }

    private void ValidateReferences()
    {
        if (playerStateMachine == null) Debug.LogError("RespawnManager: Missing StateMachine reference");
        if (playerController == null) Debug.LogError("RespawnManager: Missing PlayerController reference");
        if (playerStats == null) Debug.LogError("RespawnManager: Missing PlayerStats reference");
        if (countdownManager == null) Debug.LogError("RespawnManager: Missing CountdownManager reference");
        if (playerEvents == null) Debug.LogError("RespawnManager: Missing PlayerEvents reference");
    }

    private IEnumerator RespawnSequence(Vector2 respawnPosition)
    {
        isRespawning = true;

        // Notify that respawn is starting
        playerEvents?.TriggerRespawning(respawnPosition);

        // Wait for any ongoing processes to complete
        yield return new WaitForSeconds(0.1f);

        // Start countdown
        bool countdownComplete = false;
        StartCoroutine(countdownManager.StartCountdown(() => countdownComplete = true));

        // Wait for countdown
        yield return new WaitUntil(() => countdownComplete);

        // Double check game state before respawning
        if (playerStats.Life > 0 && Time.timeScale > 0)
        {
            // Set position and state
            playerController.SetPosition(respawnPosition);
            playerStateMachine.ChangeState(playerStateMachine.FlyingState);
            playerEvents?.TriggerRespawnComplete();
        }

        isRespawning = false;
    }

    public void InitiateRespawn(Vector2 respawnPosition)
    {
        // Guard clauses for safety
        if (isRespawning) return;
        if (playerStats.Life <= 0) return;
        if (Time.timeScale == 0) return;

        StartCoroutine(RespawnSequence(respawnPosition));
    }
}