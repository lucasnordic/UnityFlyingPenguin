using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;
    public static GameSettingsManager Instance => instance;

    [SerializeField] private GameSettings gameSettings;
    
    public GameSettings Settings => gameSettings;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
} 