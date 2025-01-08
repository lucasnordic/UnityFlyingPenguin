using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
    [Header("Stats")]
    public int maxLife = 3;
    public int maxScore = 9999;

    [Header("Physics")] // Also used for ground movement
    public float gravity = 2f;
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float slideSpeed = 10f;

    [Header("Ball State")]
    public float ballBounceForce = 5f;
    public float ballSpeedMultiplier = 0.8f;
    public float ballStateDuration = 2f;

    [Header("Flying")]
    public float startAirSpeed = 5f;
    public float airMoveForce = 0.1f;
    public float maxAirSpeed = 15f;
    public float airJumpForce = 12f;
    public float maxJumpHeight = 12f;
    public float jumpCutHeight = 50f;
    public float bounciness = 0.5f;
    public float rollMultiplier = 1f;

    [Header("Intro")]
    public float introJumpHeight = 6f;
    public float introHorizontalSpeed = 8f;
    public float introJumpTriggerHeight = 3f;
    public Vector2 introStartPosition = new Vector2(-7f, 0f);
} 