using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinBehaviour : MonoBehaviour
{
    // Component references
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] private GameObject visualsObject;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float slideSpeed = 10f; // Configurable slide speed
    [SerializeField] private float groundSlideTime = 0.5f; // How long to slide on ground

    private PenguinState state;

    private struct PenguinState
    {
        public bool isGrounded;
        public bool isSliding;
        public bool isRunning;
        public bool hasDoubleJumped;
        public float slideStartTime;
        public Vector2 startPosition;
        public Quaternion startRotation;

        public void Reset()
        {
            isGrounded = false;
            isSliding = false;
            isRunning = false;
            slideStartTime = -1f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        anim = visualsObject.GetComponent<Animator>();
        sr = visualsObject.GetComponent<SpriteRenderer>();

        // store initial state
        state = new PenguinState
        {
            startPosition = transform.position,
            startRotation = transform.rotation
        };

        // Initialize state
        ResetState();
    }

    public void ResetState()
    {
        // reset physics
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // reset pos, rot, state vars and animations
        transform.position = state.startPosition;
        transform.rotation = state.startRotation;
        state.Reset();
        anim.Play("penguin_idle");
        CancelInvoke(); // Cancel any scheduled invokes
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Handle movement based on state
        if (state.isSliding)
        {
            HandleSlideMovement(moveInput);
        }
        else
        {
            HandleNormalMovement(moveInput);
        }

        // Flip sprite
        if (moveInput != 0 && !state.isSliding)
        {
            sr.flipX = moveInput < 0;
        }

        // Handle inputs
        HandleJump();
        HandleSlideInput();
        HandleAttack();

        // Update animator parameters
        UpdateAnimations(moveInput);
    }

    private void HandleJump()
    {
        if (!Input.GetButtonDown("Jump")) return;
        
        if (state.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.Play("penguin_jump");
            state.hasDoubleJumped = false; // Reset double jump

            // reset slide timer if we jump while sliding
            if (state.isSliding)
            {
                state.slideStartTime = -1f;
            }
        }
        else if (!state.hasDoubleJumped) // Double jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.75f);
            state.hasDoubleJumped = true;
            anim.Play("penguin_jump");
        }
        
    }   

    private void HandleSlideInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (state.isSliding) { EndSlide(); }
            else { StartSlide(); }
        }

        // Only auto-stop slide when on ground and no input
        if (state.isSliding && state.isGrounded && Input.GetAxis("Horizontal") == 0)
        {
            EndSlide();
        }
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // only play attack animation once
            anim.Play("penguin_attack");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // return to idle animation
            anim.Play("penguin_idle");
        }
    }

    private void HandleNormalMovement(float moveInput)
    {
        if (state.isGrounded)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Air control when not sliding
            float currentXVelocity = rb.velocity.x;
            float targetVelocity = moveInput * moveSpeed;
            float newXVelocity = Mathf.Lerp(currentXVelocity, targetVelocity, 0.1f); // Adjust 0.1f for more/less air control
            rb.velocity = new Vector2(newXVelocity, rb.velocity.y);
        }
    }

    private void HandleSlideMovement(float moveInput)
    {
        // Get slide direction
        float slideDirection = sr.flipX ? -1 : 1;

        HandleSlideVelocity(moveInput, slideDirection);
        CheckEndSlide();
    }   

    private void HandleSlideVelocity (float moveInput, float slideDirection)
    {
        float currentXVelocity = rb.velocity.x;

        // Set initial slide velocity when new slide starts
        if (state.slideStartTime < 0)
        {
            rb.velocity = new Vector2(slideDirection * slideSpeed, rb.velocity.y);
        }

        // Air control while sliding
        float oppositeInput = -Mathf.Sign(currentXVelocity) * moveInput;

        if (oppositeInput > 0)
        {
            float airControlFactor = 0.03f; // Adjust for more/less control
            float targetVelocity = currentXVelocity + (moveInput * moveSpeed * airControlFactor);
            rb.velocity = new Vector2(targetVelocity, rb.velocity.y);
        }
    }

    private void CheckEndSlide()
    {
        // Only check for slide end when we're on ground and have a valid start time
        if (state.isGrounded && state.slideStartTime > 0)
        {
            float slideDuration = Time.time - state.slideStartTime;
            if (slideDuration > groundSlideTime)
            {
                EndSlide();
            }
        }
    }

    void UpdateAnimations(float moveInput)
    {
        // Get current animation state
        var currentState = anim.GetCurrentAnimatorStateInfo(0);

        // Don't interrupt attack, jump or slide animations
        if ( currentState.IsName("penguin_attack") || currentState.IsName("penguin_jump") || currentState.IsName("penguin_slide"))
        {
            return;
        }

        if (currentState.IsName("penguin_preslide") && !state.isGrounded)
        {
            return;
        }

        // If moving
        if (Mathf.Abs(moveInput) > 0)
        {
            anim.Play("penguin_walk");
        }
        else
        {
            anim.Play("penguin_idle");
        }
    }

    void StartSlide()
        {
        state.isSliding = true;
        anim.Play("penguin_preslide");

        // set slide direction 
        float slideDirection = sr.flipX ? -1 : 1;
        rb.velocity = new Vector2(slideDirection * slideSpeed, rb.velocity.y);

        // start slide timer
        if (state.isGrounded)
        {
            state.slideStartTime = Time.time;
        } else
        {
            state.slideStartTime = -1f; // reset slide timer, if we are in the air
        }

        Invoke("TransitionToSlide", 0.1f);
    }

    void TransitionToSlide()
    {
        anim.Play("penguin_slide");
    }

    private void EndSlide()
    {
        state.isSliding = false;
        state.slideStartTime = -1f;
        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y); // reduce velocity

        anim.Play("penguin_idle");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Ground"))
        {
            return;
        }

        state.isGrounded = true;

        // if we are sliding and we just hit the ground, start the slide timer
        if (state.isSliding && state.slideStartTime < 0)
        {
            state.slideStartTime = Time.time;
        }
        // check if we are in a jump animation
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("penguin_jump"))
        {
            StartCoroutine(PlayLandingAnimation());
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            state.isGrounded = false;
        }
    }

    IEnumerator PlayLandingAnimation()
    {
        // this won't work
        anim.Play("penguin_land");
        yield return new WaitForSeconds(2.1f);
        anim.Play("penguin_idle");
    }
}
