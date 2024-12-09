using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Penguin/Movement Data")]
public class WorldData : ScriptableObject
{
    [Header("World Stats")]
    //public float worldWidth = 16f;
    /*public float worldDepth = 10f;*/ // the depth of the background
    public float worldHeight = 10f; // the height of the background skybox until it fades to black
}