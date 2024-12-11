[System.Serializable]
public class WorldSettings
{
    [Header("World Bounds")]
    public float worldHeight = 10f;
    public float worldWidth = 16f;
    public float worldDepth = 10f;
    
    [Header("Spawning")]
    public float wallSpawnInterval = 25f;
    public float wallMaxHeight = 11f;
    public float wallMinHeight = 1.5f;
    public int largeCoinSpawnInterval = 4;
    public int heartSpawnInterval = 6;
} 