using UnityEngine;
public class KeyboardInput : MonoBehaviour, IPlayerInput  // Make sure MonoBehaviour is here
{
    // Cache the values in Update to avoid getting input outside of Update
    private bool jumpPressed;
    private bool startGamePressed;
    private float moveDirection;
    private bool slidePressed;

    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        startGamePressed = Input.GetKeyDown(KeyCode.Space);
        moveDirection = Input.GetAxis("Horizontal");
    }

    public bool JumpPressed => jumpPressed;
    public bool StartGamePressed => startGamePressed;
    public float MoveDirection => moveDirection;
    public bool SlidePressed => slidePressed;
}