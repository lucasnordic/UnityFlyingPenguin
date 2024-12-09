using UnityEngine;

public class SkyGradient : MonoBehaviour
{
    [SerializeField] private Color topColor = Color.black;
    [SerializeField] private Color bottomColor = Color.blue;

    void Start()
    {
        // Create a new material
        Material gradientMaterial = new Material(Shader.Find("Standard"));

        // Create a gradient texture
        Texture2D gradientTexture = new Texture2D(1, 256);
        Color[] colors = new Color[256];

        // Fill the texture with a gradient
        for (int i = 0; i < 256; i++)
        {
            float t = i / 255f;
            colors[i] = Color.Lerp(bottomColor, topColor, t);
        }

        gradientTexture.SetPixels(colors);
        gradientTexture.Apply();

        // Apply the texture to the material
        gradientMaterial.mainTexture = gradientTexture;

        // Apply the material to your background cube
        GetComponent<MeshRenderer>().material = gradientMaterial;
    }
}