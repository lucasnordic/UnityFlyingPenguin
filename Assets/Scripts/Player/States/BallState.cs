using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallState : IPlayerState
{
    private readonly StateMachine stateMachine;
    private readonly PlayerController player;
    private readonly VisualController visuals;

    public BallState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        player = stateMachine.GetComponent<PlayerController>();
        visuals = stateMachine.GetComponent<VisualController>();
    }

    public void EnterState()
    {
        visuals.PlayAnimation("penguin_idle");
        SlowDownPlayer();
    }

    private void SlowDownPlayer()
    {
        float speedReduce = 0.8f;
        Vector2 currentVelocity = player.Rigidbody.velocity;
        player.Rigidbody.velocity = new Vector2(
            currentVelocity.x * speedReduce,
            -currentVelocity.y * speedReduce
        );
    }
    public void UpdateState()
    {
        // slow down the player gradually
        Vector2 currentVelocity = player.Rigidbody.velocity;
        player.Rigidbody.velocity = new Vector2(
            currentVelocity.x * 0.995f,
            currentVelocity.y * 0.995f
        );
    }

    public void FixedUpdateState() { }
    public void ExitState() { }
}
