using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // Component references
    private IGameStats playerStats;
    private StateMachine playerStateMachine;
    private VegetationSpawner vegetationSpawner;
    private RepeaterController repeaterController;
    private CameraFollow cameraFollow;
    private GameTimer gameTimer;

    // Variables
    private bool isGameStarted = false;

    // Getters and Setters
    public static GameManager Instance => instance;
    public bool IsGameStarted
    {
        get => isGameStarted;
        set => isGameStarted = value;
    }


    // dont destroy on load
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadMainMenu();

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();
    }

    void FindReferences()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerStateMachine = FindObjectOfType<StateMachine>();
        vegetationSpawner = FindObjectOfType<VegetationSpawner>();
        repeaterController = FindObjectOfType<RepeaterController>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        gameTimer = FindObjectOfType<GameTimer>();

        if (playerStats != null)
        {
            playerStats.OnGameOver += HandleGameOver;
        }
    }

    // Load First level
    private void LoadMainMenu()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "_preload")
        {
            SceneManager.LoadScene("BouncyPenguin-Level1");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Restart time
        this.IsGameStarted = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

private void HandleGameOver()
    {
        Time.timeScale = 0f; // Pause the game, letting the UI handle the game over
    }
}
