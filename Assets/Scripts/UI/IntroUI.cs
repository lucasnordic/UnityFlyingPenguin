using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private TMP_Text startPromptText;
    [SerializeField] private TMP_Text escPromptText;
    private StateMachine playerStateMachine;

    private void Start()
    {
        playerStateMachine = FindObjectOfType<StateMachine>();

        if (startPromptText == null)
        {
            Debug.LogError("Start prompt text not assigned in IntroUI");
        }
    }

    private void Update()
    {
        // Only show text during intro state
        if (startPromptText != null)
        {
            startPromptText.gameObject.SetActive(playerStateMachine.IsInState<IntroState>());
        }

        if (escPromptText != null)
        {
            escPromptText.gameObject.SetActive(playerStateMachine.IsInState<IntroState>());
        }
    }
}