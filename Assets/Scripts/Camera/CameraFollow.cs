using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 2f; 
    [SerializeField] private float xlookAheadDistance = 4f;
    [SerializeField] private float yLookAheadDistance = 4f;
    [SerializeField] private float lookAheadTime = 0.5f;  // Time to reach look-ahead position
    [SerializeField] private float targetHorizontalOffset = 12f;

    private float currentLookAheadX = 0f;
    private float currentLookAheadY = 0f;
    private float lookAheadVelocityX;
    private float lookAheadVelocityY;
    private float currentHorizontalOffset;
    private float offsetSmoothVelocity;
    private Vector2 currentVelocity;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnOffset = 4f;
    [SerializeField] private GameObject respawnPreviewPrefab;
    private GameObject currentPreview;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomedInZ = -1f; // Closer to scene
    [SerializeField] private float zoomDuration = 2f;

    private Transform playerPosition;
    private float verticalOffset;
    private PlayerEvents playerEvents;
    private PlayerController player;
    private StateMachine playerStateMachine;
    private bool isFollowingPlayer = true;

    void Start()
    {
        SetupPlayerReferences();
        SubscribeToEvents();
    }

    void LateUpdate()
    {
        FollowPlayer();
    }

    void OnDestroy()
    {
        if (playerEvents == null) return;
        playerEvents.OnPlayerRespawning -= HandlePlayerRespawning;
        playerEvents.OnPlayerRespawnComplete -= HandlePlayerRespawnComplete;
    }

    private void SetupPlayerReferences()
    {
        playerPosition = GameObject.FindWithTag("Player").transform;
        currentHorizontalOffset = 0f;
        verticalOffset = transform.position.y - playerPosition.position.y;
        player = playerPosition.GetComponent<PlayerController>();
        playerStateMachine = player.GetComponent<StateMachine>();
    }

    private void SubscribeToEvents()
    {
        playerEvents = Object.FindObjectOfType<PlayerEvents>();

        if (playerEvents == null) return; 

        playerEvents.OnPlayerRespawning += HandlePlayerRespawning;
        playerEvents.OnPlayerRespawnComplete += HandlePlayerRespawnComplete;
    }

    private void HandlePlayerRespawning(Vector2 respawnPosition)
    {
        StartCoroutine(ZoomForRespawn(respawnPosition));
    }


    private void HandlePlayerRespawnComplete()
    {
        isFollowingPlayer = true;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }

    // Zoom in towards the player and return to respawn position
    private IEnumerator ZoomForRespawn(Vector2 respawnPosition)
    {
        float originalZ = transform.position.z;
        float currentZVelocity = 0f;
        float elapsedTime = 0f;

        // Zoom in sequence
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float newZ = Mathf.SmoothDamp(transform.position.z, zoomedInZ, ref currentZVelocity, zoomDuration / 2);

            transform.position = new Vector3(
                playerPosition.position.x,
                playerPosition.position.y,
                newZ
            );
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        // Show preview sprite
        if (respawnPreviewPrefab != null)
        {
            currentPreview = Instantiate(respawnPreviewPrefab, respawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Respawn preview prefab is not assigned!");
        }

        // Move camera
        isFollowingPlayer = false;
        SetCameraPositionX(respawnPosition.x - respawnOffset);
        SetCameraPositionY(respawnPosition.y + respawnOffset);

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            originalZ
        );
    }

    // Follow the player with look-ahead
    private void FollowPlayer()
    {
        if (!isFollowingPlayer) return;

        Vector3 playerPos = playerPosition.position;

        // Adjust horizontal offset based on player state
        float desiredOffset = 8f; // Default centered offset
        if (playerStateMachine.IsInState<IntroState>())
        {
            desiredOffset = targetHorizontalOffset; // default when flying
        }

        // Smoothly adjust the horizontal offset
        currentHorizontalOffset = Mathf.SmoothDamp(
            currentHorizontalOffset,
            desiredOffset,
            ref offsetSmoothVelocity,
            1f / smoothSpeed
        );

        // Look-ahead based on input direction on X axis
        float xInput = Input.GetAxis("Horizontal");
        float targetLookAheadX = xlookAheadDistance * xInput;
        currentLookAheadX = Mathf.SmoothDamp(
            currentLookAheadX,
            targetLookAheadX,
            ref lookAheadVelocityX,
            lookAheadTime
        );

        // Look-ahead based on input direction on Y axis
        float yInput = Input.GetAxis("Vertical");
        float targetLookAheadY = yLookAheadDistance * yInput;
        currentLookAheadY = Mathf.SmoothDamp(
            currentLookAheadY,
            targetLookAheadY,
            ref lookAheadVelocityY,
            lookAheadTime
        );

        // Calculate target X and Y positions
        float targetX = playerPos.x + currentHorizontalOffset + currentLookAheadX;
        float targetY = playerPos.y + verticalOffset + currentLookAheadY;

        // Smooth movement to target position
        Vector2 smoothPosition = Vector2.SmoothDamp(
            transform.position,
            new Vector2(targetX, targetY),
            ref currentVelocity,
            1f / smoothSpeed
        );

        transform.position = new Vector3(smoothPosition.x, smoothPosition.y, transform.position.z);
    }

    public void SetCameraPositionX(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        currentLookAheadX = 0f;
        currentVelocity = Vector2.zero;
    }

    public void SetCameraPositionY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        currentLookAheadX = 0f;
        currentVelocity = Vector2.zero;
    }

    public void SetCameraPosition(Vector3 position)
    {
        transform.position = position;
        currentLookAheadX = 0f;
        currentVelocity = Vector2.zero;
    }

    public void ResetCamera(Vector3 playerStartPos)
    {
        // Reset all the camera's internal state
        currentLookAheadX = 0f;
        currentVelocity = Vector2.zero;
        isFollowingPlayer = true;

        float cameraX = playerStartPos.x + currentHorizontalOffset;
        float cameraY = playerStartPos.y + verticalOffset;

        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
}