using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : IPlayerState
{
    // Component references
    private readonly StateMachine stateMachine;
    private readonly PlayerController player;
    private readonly VisualController visuals;
    private readonly MovementData data;
    private readonly PhysicsMaterialManager materials;
    private readonly IPlayerInput input;

    // State tracking
    private bool isSliding;
    private float slideStartTime = -1f;

    public GroundState(StateMachine stateMachine, IPlayerInput input)
    {
        this.stateMachine = stateMachine;
        this.input = input;
        player = stateMachine.GetComponent<PlayerController>();
        visuals = stateMachine.GetComponent<VisualController>();
        materials = stateMachine.GetComponent<PhysicsMaterialManager>();
        data = player.MovementData;
    }

    public void EnterState()
    {
        materials.SetGroundMaterial();
        if (isSliding)
        {
            EndSlide();
        }
        player.transform.rotation = Quaternion.identity;
        visuals.PlaySlideSequence();
    }

    public void UpdateState()
    {
        float moveInput = input.MoveDirection;

        if (isSliding)
        {
            HandleSlideMovement(moveInput);
        }
        else
        {
            HandleNormalMovement(moveInput);
        }

        // Handle inputs
        if (input.JumpPressed) HandleJump();
        if (input.SlidePressed) HandleSlideInput();

        // Update animations
        UpdateAnimations(moveInput);
    }

    private void HandleNormalMovement(float moveInput)
    {
        Vector2 currentVelocity = player.Rigidbody.velocity;
        player.Move(new Vector2(moveInput * data.moveSpeed, currentVelocity.y));

        if (moveInput != 0)
        {
            visuals.FlipSprite(moveInput < 0);
        }
    }

    private void HandleSlideMovement(float moveInput)
    {
        float slideDirection = visuals.IsFacingLeft ? -1 : 1;

        // Set initial slide velocity when new slide starts
        if (slideStartTime < 0)
        {
            player.Move(new Vector2(slideDirection * data.slideSpeed, player.Rigidbody.velocity.y));
            slideStartTime = Time.time;
        }

        // Check if we should end slide
        if (Time.time - slideStartTime > data.ballStateDuration)
        {
            EndSlide();
        }
    }

    private void HandleJump()
    {
        Vector2 currentVelocity = player.Rigidbody.velocity;
        player.Move(new Vector2(currentVelocity.x, data.jumpForce));
        visuals.PlayAnimation("penguin_jump");

        // End slide if we're sliding
        if (isSliding)
        {
            EndSlide();
        }

        stateMachine.ChangeState(stateMachine.FlyingState);
    }

    private void HandleSlideInput()
    {
        if (isSliding)
        {
            EndSlide();
        }
        else
        {
            StartSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        visuals.PlaySlideSequence();

        float slideDirection = visuals.IsFacingLeft ? -1 : 1;
        player.Move(new Vector2(slideDirection * data.slideSpeed, player.Rigidbody.velocity.y));
        slideStartTime = Time.time;
    }

    private void EndSlide()
    {
        isSliding = false;
        slideStartTime = -1f;

        // Reduce velocity when ending slide
        Vector2 currentVelocity = player.Rigidbody.velocity;
        player.Move(new Vector2(currentVelocity.x * 0.5f, currentVelocity.y));

        visuals.PlayAnimation("penguin_idle");
    }

    private void UpdateAnimations(float moveInput)
    {
        // Don't interrupt certain animations
        string currentAnim = visuals.GetCurrentAnimation();
        if (currentAnim == "penguin_attack" ||
            currentAnim == "penguin_jump" ||
            currentAnim == "penguin_slide" ||
            (currentAnim == "penguin_preslide"))
        {
            return;
        }

        if (isSliding) return;

        visuals.PlayAnimation(Mathf.Abs(moveInput) > 0 ? "penguin_walk" : "penguin_idle");
    }

    public void FixedUpdateState() { }
    public void ExitState() { }
}