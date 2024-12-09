using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerStats))]
public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private AudioSource coinAudioSource;
    [SerializeField] private AudioSource lifeAudioSource;
    [SerializeField] private AudioSource crashAudioSource; // TODO: Add crash sound

    // Component references
    private PlayerStats playerStats;
    private StateMachine stateMachine;
    private VoiceLineManager voiceLineManager;
    private RespawnManager respawnManager;
    private PlayerController player;

    // Variables
    private bool hasReachedFirstCheckpoint = false;
    private Vector2 lastCheckpointPosition;
    private Vector2 startPosition;
    private float currentVelocityX;

    // Getters
    public Vector2 LastCheckpointPosition => lastCheckpointPosition;
    public bool HasReachedFirstCheckpoint => hasReachedFirstCheckpoint;

    private void Start()
    {
        startPosition = transform.position; // Store initial position as first spawn point
        lastCheckpointPosition = startPosition;
    }

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        stateMachine = GetComponentInChildren<StateMachine>();
        voiceLineManager = FindObjectOfType<VoiceLineManager>();
        respawnManager = FindObjectOfType<RespawnManager>();
        player = GetComponent<PlayerController>();

        // Error handling
        if (respawnManager == null) Debug.LogError("CollisionHandler: Missing RespawnManager reference");
        if (stateMachine == null) Debug.LogError("StateMachine not found in children of " + gameObject.name);
        if (coinAudioSource == null || lifeAudioSource == null)  Debug.LogWarning("AudioSources not set in " + gameObject.name); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Coin":
                HandleCoinCollision(collision);
                break;

            case "CoinLarge":
                HandleCoinCollision(collision);
                break;

            case "Fire":
                HandleFireCollision(collision);
                break;

            case "Door":
                HandleDoorCollision();
                break;

            case "Life":
                HandleLifeCollision(collision);
                break;

            case "Checkpoint":
                hasReachedFirstCheckpoint = true;
                lastCheckpointPosition = collision.transform.position;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // only handle following collisions if player is in flying state
        if (!stateMachine.IsInState<FlyingState>()) return;

        // store new speed if its higher than before
        currentVelocityX = player.Rigidbody.velocity.x;
        if (currentVelocityX > player.HighestHorizontalSpeed)
        {
            player.HighestHorizontalSpeed = currentVelocityX;
        }

        switch (collision.gameObject.tag)
        {
            case "Ground":
                HandleObstacleCollision();
                break;

            case "Brick": // AKA BrickWall
                HandleObstacleCollision();
                break;
        }
    }

    private void HandleObstacleCollision()
    {
        if (playerStats.Life <= 1)
        {
            playerStats.LoseLife();
            return;
        }

        playerStats.LoseLife();

        Vector2 spawnPosition = hasReachedFirstCheckpoint ? lastCheckpointPosition : startPosition;
        spawnPosition.x += 1f;

        stateMachine.ChangeState(stateMachine.BallState);
        respawnManager.InitiateRespawn(spawnPosition);

        if (crashAudioSource) crashAudioSource.Play();
        if (voiceLineManager) voiceLineManager.PlayCrashLine();
    }

    private void HandleCoinCollision(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
        if (collision.CompareTag("CoinLarge")) playerStats.AddScore(5);
        else playerStats.AddScore(1);
        coinAudioSource?.Play();
    }

    private void HandleLifeCollision(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
        playerStats.GainLife();
        lifeAudioSource?.Play();
    }
    private void HandleFireCollision(Collider2D collision) // Todo, update
    {
        playerStats.LoseLife();

        if (stateMachine != null) stateMachine.ResetState();
    }

    private void HandleDoorCollision()
    {
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next level available!");
        }
    }

    public void Reset()
    {
        hasReachedFirstCheckpoint = false;
        lastCheckpointPosition = startPosition;
    }
}