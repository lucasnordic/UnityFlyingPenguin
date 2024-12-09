using UnityEngine;
using UnityEngine.UI;

public class IntroState : IPlayerState
{
    private readonly StateMachine stateMachine;
    private readonly PlayerController player;
    private readonly VisualController visuals;
    private readonly MovementData data;
    private readonly IPlayerInput input;
    private readonly Image image;

    public IntroState(StateMachine stateMachine, IPlayerInput input)
    {
        this.stateMachine = stateMachine;
        this.input = input;
        player = stateMachine.GetComponent<PlayerController>();
        visuals = stateMachine.GetComponent<VisualController>();
        data = player.MovementData;
        image = GameObject.FindGameObjectWithTag("Binoculars_UI").GetComponent<Image>();
    }

    public void EnterState()
    {
        // Disable physics/gravity
        player.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        player.Rigidbody.velocity = Vector2.zero;
        player.Rigidbody.gravityScale = 0f;

        // Set initial position
        player.SetPosition(data.introStartPosition);

        // Start flying animation
        visuals.PlayAnimation("penguin_fly");

        // set binoculars UI to full alpha
        image.color = new Color(1, 1, 1, 1);
    }

    public void UpdateState()
    {
        // Move horizontally at constant speed
        player.SetPosition(player.transform.position +
            new Vector3(data.introHorizontalSpeed * Time.deltaTime, 0, 0));

        // Check for start game input
        if (input.StartGamePressed)
        {
            // Enable physics and transition to flying state
            player.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            player.Rigidbody.velocity = new Vector2(data.introHorizontalSpeed, 0);
            player.Rigidbody.gravityScale = data.gravity;
            stateMachine.ChangeState(stateMachine.FlyingState);
        }
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        // Ensure physics is enabled when leaving intro state
        player.Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        // reduce binoculars UI alpha
        image.color = new Color(1, 1, 1, 0);
    }
}