using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IPlayerInput input;
    private IPlayerState currentState;
    private IPlayerState previousState;
    public IntroState IntroState { get; private set; }
    public GroundState GroundState { get; private set; }
    public FlyingState FlyingState { get; private set; }
    public BallState BallState { get; private set; }

    void Awake()
    {
        input = GetComponent<KeyboardInput>();

        if (input == null)
        {
            Debug.LogError("No KeyboardInput component found on " + gameObject.name);
            return;
        }

        // Pass input to states
        IntroState = new IntroState(this, input);
        FlyingState = new FlyingState(this, input);
        GroundState = new GroundState(this, input);
        BallState = new BallState(this);
    }

    void Start()
    {
        ChangeState(IntroState);
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    public void ResetState()
    {
        ChangeState(IntroState); // Reset to IntroState
    }

    public void ChangeState(IPlayerState newState)
    {
        previousState = currentState;
        currentState?.ExitState();
        currentState = newState;
        currentState?.EnterState();
    }

    public bool IsInState<T>() where T : IPlayerState
    {
        return currentState is T;
    }
}