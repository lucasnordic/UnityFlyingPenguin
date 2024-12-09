using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Refactor this bad boy
public class RepeaterController : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject lifePowerup;
    [SerializeField] private GameObject largeCoinPowerup;

    // References
    private StateMachine playerStateMachine;
    private PlayerController player;
    private CollisionHandler playerCollisionHandler;
    private GameTimer gameTimer;

    // Variables
    private float lastSpawnX = 0f;  // Track where we last spawned a wall
    private float wallSpawnInterval = 25f;  // Distance between walls
    private bool hasSpawnedFirstWall = false;
    private float wallMaxHeight = 11f;
    private float wallMinHeight = 1.5f;

    private readonly int largeCoinSpawnInterval = 4;
    private int largeCoinCounter = 0;
    private readonly int heartSpawnInterval = 6;
    private int heartCounter = 0;

    private Vector3[] initialGroundPositions;
    private GameObject[] Grounds = null;
    private float GroundWidth = 0;
    private int GroundCount = 0;

    private Vector3[] initialSkyPositions;
    private GameObject[] Skys = null;
    private float SkyWidth = 0;
    private int SkyCount = 0;

    private Vector3[] initialCloudPositions;
    private GameObject[] Clouds = null;
    private float CloudWidth = 15f; // hardcoded check because of scale
    private int CloudCount = 0;

    // Getters and Setters
    public float LastSpawnX => lastSpawnX;

    private void Start() { GetReferences(); }

    void Update() { SpawnObjects(); }

    private void OnTriggerEnter2D(Collider2D collision) { MoveObjects(collision); }

    private void GetReferences()
    {
        playerStateMachine = GameObject.FindGameObjectWithTag("Player").GetComponent<StateMachine>();
        player = playerStateMachine.GetComponent<PlayerController>();
        playerCollisionHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CollisionHandler>();
        gameTimer = FindObjectOfType<GameTimer>();

        Grounds = GameObject.FindGameObjectsWithTag("Ground");
        GroundWidth = Grounds[0].transform.localScale.x;
        GroundCount = Grounds.Length;

        Skys = GameObject.FindGameObjectsWithTag("Sky");
        SkyWidth = Skys[0].transform.localScale.x;
        SkyCount = Skys.Length;

        Clouds = GameObject.FindGameObjectsWithTag("Cloud");
        CloudCount = Clouds.Length;
    }

    private void SpawnObjects()
    {
        if (playerStateMachine.IsInState<FlyingState>() &&
            (!hasSpawnedFirstWall || player.transform.position.x > lastSpawnX - wallSpawnInterval))
        {
            SpawnBrickWall();
            heartCounter++;
            largeCoinCounter++;

            if (heartCounter >= heartSpawnInterval)
            {
                GenerateHeart(9f); // generate heart powerup between walls
                heartCounter = 0;
            }

            if (largeCoinCounter >= largeCoinSpawnInterval)
            {
                GenerateLargeCoin(11f); // generate large coin powerup between walls
                largeCoinCounter = 0;
            }
        }
    }

    private void MoveObjects(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            HandleGround(collision);
        if (collision.CompareTag("Sky"))
            HandleSky(collision);
        if (collision.CompareTag("Cloud"))
            HandleCloud(collision);
        if (collision.CompareTag("Brick"))
            Destroy(collision.gameObject);  // Remove old bricks
    }

    private void HandleGround(Collider2D collision)
    {
        Vector3 newPosition = collision.transform.position;
        newPosition.x += GroundWidth * GroundCount;
        collision.transform.position = newPosition;
    }

    private void HandleSky(Collider2D collision)
    {
        Vector3 newPosition = collision.transform.position;
        newPosition.x += SkyWidth * (SkyCount - 1);
        collision.transform.position = newPosition;
    }

    private void HandleCloud(Collider2D collision)
    {
        Vector3 newPosition = collision.transform.position;
        newPosition.x += CloudWidth * (CloudCount - 1);
        collision.transform.position = newPosition;
    }

    private void SpawnBrickWall()
    {
        Vector3 spawnPosition;
        float timeRemaining = gameTimer.TimeRemaining;
        float extraSeparation = (timeRemaining / gameTimer.GameDuration) * 2f; // decrease separation as time goes on

        if (!hasSpawnedFirstWall)
        {
            spawnPosition = new Vector3(player.transform.position.x + 10f, Random.Range(wallMinHeight, wallMaxHeight), 0);
            hasSpawnedFirstWall = true;
        }
        else
        {
            spawnPosition = new Vector3(lastSpawnX + wallSpawnInterval, Random.Range(wallMinHeight, wallMaxHeight), 0);
        }

        // instantiate the brickPrefab and find wall top and bottom
        GameObject brick = Instantiate(brickPrefab, spawnPosition, Quaternion.identity);
        Transform wallTop = brick.transform.Find("TopWall");
        Transform wallBottom = brick.transform.Find("BottomWall");

        if (wallTop == null || wallBottom == null)
        {
            Debug.LogError("WallTop or WallBottom not found in Brick prefab!");
            return;
        }

        // set the wall top and bottom positions
        wallTop.position += Vector3.up * extraSeparation;
        wallBottom.position -= Vector3.up * extraSeparation;

        brick.tag = "Brick";  // Make sure Brick is caught by the repeater
        lastSpawnX = spawnPosition.x;
    }

    private void GenerateHeart(float spawnDistance)
    {
        float spawnX = lastSpawnX + spawnDistance;
        Vector3 position = new Vector3(spawnX, Random.Range(3f, 7f), 0);

        Instantiate(lifePowerup, position, Quaternion.identity);
    }

    private void GenerateLargeCoin(float spawnDistance)
    {
        float spawnX = lastSpawnX + spawnDistance;
        Vector3 position = new Vector3(spawnX, Random.Range(2f, 9f), 0);

        Instantiate(largeCoinPowerup, position, Quaternion.identity);
    }

    public void ResetSpawnPosition()
    {
        lastSpawnX = 0f;
        hasSpawnedFirstWall = false;
    }
}
