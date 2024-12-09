using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : IPlayerState
{
    // Component references
    private readonly StateMachine stateMachine;
    private readonly PlayerController player;
    private readonly VisualController visuals;
    private readonly MovementData data;
    private readonly IPlayerInput input;

    public FlyingState(StateMachine stateMachine, IPlayerInput input)
    {
        this.stateMachine = stateMachine;
        player = stateMachine.GetComponent<PlayerController>();
        visuals = stateMachine.GetComponent<VisualController>();
        this.input = input;
        data = player.MovementData;
    }

    public void EnterState()
    {
        float startSpeed;

        // Set start speed based on whether game has started or is already running
        if (GameManager.Instance != null && !GameManager.Instance.IsGameStarted)
        {
            startSpeed = data.startAirSpeed;
            GameManager.Instance.IsGameStarted = true;
        }
        else if (player.HighestHorizontalSpeed <= 0) // temporary fix since brick wall collision sets speed to 0
        {
            startSpeed = data.startAirSpeed;
        }
        else
        {
            startSpeed = player.HighestHorizontalSpeed;
        }

        player.Rigidbody.velocity = new Vector2(startSpeed, player.Rigidbody.velocity.y);
        player.Rigidbody.gravityScale = data.gravity;
        Jump();

        visuals.PlayAnimation("penguin_fly");
    }

    public void UpdateState()
    {
        if (input.JumpPressed) Jump();
    }

    private void Jump()
    {
        float heightRatio = player.transform.position.y / data.jumpCutHeight;
        // Using power function for sharper falloff
        float jumpForce = data.airJumpForce * Mathf.Pow(1 - heightRatio, 2f); // 2f makes it quadratic

        // use velocity instead of AddForce for more control
        // player.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        player.Rigidbody.velocity = new Vector2(player.Rigidbody.velocity.x, jumpForce);


    }

    public void FixedUpdateState()
    {
        Vector2 currentVelocity = player.Rigidbody.velocity;
        IncreaseSpeed(currentVelocity);
        AddRoll(currentVelocity);
    }

    private void IncreaseSpeed(Vector2 currentVelocity)
    {
        // Increase speed if below max speed
        if (Mathf.Abs(currentVelocity.x) < data.maxAirSpeed)
        {
            float speedDifference = data.startAirSpeed - Mathf.Abs(currentVelocity.x);
            player.AddForce(Vector2.right * data.airMoveForce);
        }
    }

    private void AddRoll(Vector2 currentVelocity)
    {
        // Add roll based on vertical velocity
        float roll = currentVelocity.y * data.rollMultiplier;
        player.setRotation(Quaternion.Euler(0, 0, roll));
    }

    public void ExitState() {
    }
}