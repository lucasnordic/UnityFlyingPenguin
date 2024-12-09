using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private TMP_Text speedText;

    private PlayerController playerController;
    private float currentSpeed;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (!ReferenceCheck())
            return;
    }

    void Update()
    {
        if (!ReferenceCheck())
            return;

        // current speed on the x axis
        currentSpeed = playerController.Rigidbody.velocity.x;

        if (speedText == null)
            return;

        // Update UI text and fill amount
        speedText.text = $"Speed: {currentSpeed:F2}";
    }

    private bool ReferenceCheck()
    {
        if (playerController == null)
        {
            Debug.LogError("SpeedMeter: PlayerController not found in scene.");
            return false;
        }

        if (speedText == null)
        {
            Debug.LogError("SpeedMeter: Speed Text not assigned.");
            return false;
        }

        return true;
    }
}
