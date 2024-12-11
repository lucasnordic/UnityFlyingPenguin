[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public PlayerSettings playerSettings;
    public WorldSettings worldSettings;
    public UISettings uiSettings;
    public CameraSettings cameraSettings;
} 