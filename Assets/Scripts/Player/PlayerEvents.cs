using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public event System.Action<Vector2> OnPlayerRespawning;
    public event System.Action OnPlayerRespawnComplete;

    public void TriggerRespawning(Vector2 position)
    {
        OnPlayerRespawning?.Invoke(position);
    }

    public void TriggerRespawnComplete()
    {
        OnPlayerRespawnComplete?.Invoke();
    }
}