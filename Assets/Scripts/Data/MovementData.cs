using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Penguin/Movement Data")]
public class MovementData : ScriptableObject
{
    [Header("Player Stats")]
    public int maxLife = 3;
    public int maxScore = 9999;

    [Header("Physics")]
    public float gravity = 2f;

    [Header("Ground Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float slideSpeed = 10f;

    [Header("Ball State")]
    public float ballBounceForce = 5f;
    public float ballSpeedMultiplier = 0.8f;
    public float ballStateDuration = 2f; // how long the player will be in ball state

    [Header("Flying Movement")]
    public float startAirSpeed = 5f;
    public float airMoveForce = 0.1f;
    public float maxAirSpeed = 15f;
    public float airJumpForce = 12f;
    public float maxJumpHeight = 12f; // the maximum height the player is allowed to use the full jump force
    public float jumpCutHeight = 50f; // the height at which the player will no longer have any upward force applied. Like air is too thin to push against
    public float bounciness = 0.5f;
    public float rollMultiplier = 1f;

    [Header("Intro Settings")]
    public float introJumpHeight = 6f;
    public float introHorizontalSpeed = 8f;
    public float introJumpTriggerHeight = 3f;
    public Vector2 introStartPosition = new Vector2(-7f, 0f);
}