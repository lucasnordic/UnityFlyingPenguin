using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class MusicLineManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource birdAudioSource;

    [Header("Voice Line Groups")]
    [SerializeField] private AudioClip[] flapWingLines;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayFromArray();
        }
    }

    private void PlayFromArray()
    {
        if (birdAudioSource.isPlaying) return;
        if (flapWingLines == null || flapWingLines.Length == 0) return;

        AudioClip clip = flapWingLines[Random.Range(0, flapWingLines.Length)];
        birdAudioSource.PlayOneShot(clip);
    }
}
