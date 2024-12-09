using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationSpawner : MonoBehaviour
{
    [System.Serializable] public class VegetationLayer
    {
        public GameObject[] prefabs;
        public float zPosition;
        public float minSpacing = 2f;
        public float spawnChance = 0.5f;
        [Range(0, 1f)] public float densityFactor = 1f;

        // TODO: Add min and max scale
        //public float minScale;
        //public float maxScale;
    }

    [Header("Vegetation Layers")]
    [SerializeField]
    private VegetationLayer grassLayer = new()
    {
        zPosition = -1.5f, // Slightly in front
        minSpacing = 1.0f,
        spawnChance = 0.5f,
        densityFactor = 1f
    };

    [SerializeField] private VegetationLayer bushLayer = new()
    {
        zPosition = -1.45f, // Middle
        minSpacing = 3.0f,
        spawnChance = 0.5f,
        densityFactor = 0.7f
    };

    [SerializeField] private VegetationLayer treeLayer = new()
    {
        zPosition = -1.4f, // Back
        minSpacing = 5.0f,
        spawnChance = 0.3f,
        densityFactor = 0.5f
    };

    // get groundwidth from an object with the tag "Ground"
    [Header("Spawn Settings")]
    [SerializeField] private float groundWidth = 0f;
    [SerializeField] private float spawnAheadDistance = 40f;

    private float lastSpawnX;
    private List<GameObject> spawnedVegetation;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        groundWidth = GameObject.FindGameObjectWithTag("Ground").GetComponent<BoxCollider2D>().bounds.size.x;
        spawnedVegetation = new List<GameObject>();
        lastSpawnX = player.position.x - spawnAheadDistance; // Start spawning behind player
    }

    private void Update()
    {
        if (player.position.x + spawnAheadDistance > lastSpawnX) SpawnVegetationSection();
        CleanupOldVegetation();
    }

    private void SpawnVegetationSection()
    {
        float spawnX = lastSpawnX + groundWidth;

        SpawnVegetationLayer(grassLayer, spawnX);
        SpawnVegetationLayer(bushLayer, spawnX);
        SpawnVegetationLayer(treeLayer, spawnX);

        lastSpawnX = spawnX;
    }

    // Prefab is the vegetation to spawn
    private void SpawnVegetationLayer(VegetationLayer layer, float spawnX)
    {
        float baseSpacing = layer.minSpacing / layer.densityFactor; // Higher density = smaller spacing
        float x = spawnX;
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float groundHeight = ground.transform.position.y + ground.GetComponent<BoxCollider2D>().bounds.size.y / 2f;
        float lastWidth = 0f; // track last width to avoid overlapping

        while (x < spawnX + groundWidth)
        {
            if (Random.value < layer.spawnChance)
            {
                GameObject prefab = layer.prefabs[Random.Range(0, layer.prefabs.Length)];
                GameObject vegetation = Instantiate(prefab);
                SpriteRenderer spriteRenderer = vegetation.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null) { Destroy(vegetation); return; }

                // get sprite width and ensure minimum spacing
                float spriteWidth = spriteRenderer.bounds.size.x;
                x += lastWidth / 2f + spriteWidth / 2f; // Add half of last width and half of sprite width. 

                Vector3 position = new Vector3(
                    x, 
                    groundHeight, // Use prefab's y position
                    layer.zPosition
                );

                // Set position, scale and flip
                vegetation.transform.position = position;
                vegetation.transform.localScale *= Random.Range(0.75f, 1.0f); // Random scale

                if (Random.value > 0.5f)
                {
                    Vector3 scale = vegetation.transform.localScale;
                    scale.x *= -1;
                    vegetation.transform.localScale = scale;
                }

                lastWidth = spriteWidth * Mathf.Abs(vegetation.transform.localScale.x); // store width for next iteration
                spawnedVegetation.Add(vegetation);
            }

            x += baseSpacing + Random.Range(0.8f, 1.2f); // Randomize spacing a bit
        }
    }

    private void CleanupOldVegetation()
    {
        if (spawnedVegetation.Count == 0 || spawnedVegetation == null) return;

        float cleanupX = player.position.x - spawnAheadDistance - 10f; // Cleanup 10 units extra further behind player
        spawnedVegetation.RemoveAll(item => item == null); // Remove null items

        for (int i = spawnedVegetation.Count - 1; i >= 0; i--)
        {
            GameObject vegetation = spawnedVegetation[i];

            if (vegetation.transform.position.x < cleanupX)
            {
                Destroy(vegetation);
                spawnedVegetation.RemoveAt(i);
            }
        }
    }

    public void CleanupAndReset()
    {
        foreach (GameObject vegetation in spawnedVegetation)
        {
            if (vegetation != null) Destroy(vegetation);
        }

        spawnedVegetation.Clear();
        lastSpawnX = 0f;
    }
}
