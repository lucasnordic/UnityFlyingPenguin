using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    // Component references
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float bounciness = 0.5f;

    private int groundContactCount = 0;  // Track number of ground contacts
    private Vector2 startPosition;
    private bool isGrounded;

    void Start()
    {
        startPosition = transform.position;
        HandlePhysics();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        HandleJump();

        // Reset position if ball falls too far
        if (transform.position.y < -10f)
            ResetPosition();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContactCount++;
            isGrounded = groundContactCount > 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContactCount--;
            isGrounded = groundContactCount > 0;
        }
    }

    private void HandlePhysics()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.gravityScale = 2f;

        // Make sure we're using a circle collider
        if (GetComponent<CircleCollider2D>() == null)
            gameObject.AddComponent<CircleCollider2D>();

        // Set up physics material
        PhysicsMaterial2D ballMaterial = new PhysicsMaterial2D("BallMaterial");
        ballMaterial.bounciness = bounciness;
        ballMaterial.friction = 0.4f;
        GetComponent<CircleCollider2D>().sharedMaterial = ballMaterial;
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Only apply force if we're under max speed
        if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            rb.AddForce(Vector2.right * moveInput * moveForce);

        // Apply more force when changing direction
        if (moveInput != 0 && Mathf.Sign(moveInput) != Mathf.Sign(rb.velocity.x))
            rb.AddForce(Vector2.right * moveInput * moveForce * 2);

        // Add rotation based on movement
        rb.AddTorque(-moveInput * 2f);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        groundContactCount = 0;
        isGrounded = false;
    }
}