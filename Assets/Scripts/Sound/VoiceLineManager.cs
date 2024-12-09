using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class VoiceLineManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource narratorAudioSource;

    [Header("Voice Line Groups")]
    [SerializeField] private AudioClip[] gameStartLines;
    [SerializeField] private AudioClip[] crashLines;
    [SerializeField] private AudioClip[] gameOverLines;

    private StateMachine playerStateMachine;
    private PlayerStats playerStats;
    private bool hasPlayedGameOver = false;

    private void Start()
    {
        SetupReferences();
        // Start monitoring game start
        if (playerStateMachine != null && narratorAudioSource != null) StartCoroutine(MonitorGameStart());
    }
    private void SetupReferences()
    {
        playerStateMachine = FindObjectOfType<StateMachine>();
        if (playerStateMachine == null)
        {
            Debug.LogError("StateMachine not found in scene!");
            return;
        }

        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats not found in scene!");
        }
        else
        {
            playerStats.OnGameOver += PlayGameOverLine;
            playerStats.OnScoreChanged += (score) => { if (score == 0) hasPlayedGameOver = false; };
        }

        if (!narratorAudioSource)
        {
            Debug.LogError("Narrator AudioSource not assigned to VoiceLineManager!");
        }
    }

    private IEnumerator MonitorGameStart()
    {
        // Wait until we're in intro state
        while (!playerStateMachine.IsInState<IntroState>())
        {
            yield return null;
        }

        // Wait for space key
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        PlayGameStartLine();
    }

    private void PlayFromArray(AudioClip[] clips)
    {
        if (narratorAudioSource.isPlaying) return;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        narratorAudioSource.PlayOneShot(clip);
    }

    public void PlayGameStartLine() => PlayFromArray(gameStartLines);
    public void PlayCrashLine() => PlayFromArray(crashLines);
    private void PlayGameOverLine()
    {
        if (hasPlayedGameOver) return;
        PlayFromArray(gameOverLines);
        hasPlayedGameOver = true;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnGameOver -= PlayGameOverLine;
            playerStats.OnScoreChanged -= (score) => { if (score == 0) hasPlayedGameOver = false; };
        }
    }
}
