using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;

    // Component references
    private GameSettings gameSettings;
    private Rigidbody2D rb;
    private float highestHorizontalSpeed;

    // getters and setters
    public MovementData MovementData => movementData;
    public Rigidbody2D Rigidbody => rb;
    public float HighestHorizontalSpeed
    { 
        get => highestHorizontalSpeed;
        set => highestHorizontalSpeed = value; 
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        highestHorizontalSpeed = movementData.startAirSpeed;
        gameSettings = GameSettingsManager.Instance.Settings;
    }

    public void Move(Vector2 velocity)
    {
        rb.velocity = velocity;
    }

    public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        rb.AddForce(force, forceMode);
    }

    public void setRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
    public void ResetSpeed()
    {
        highestHorizontalSpeed = movementData.startAirSpeed;
    }
}
