using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexChunk : MonoBehaviour
{
    public GameObject hexPrefab; // Your hexagon prefab
    public int chunkSizeX = 10; // Width of the chunk
    public int chunkSizeZ = 10; // Height of the chunk
    public float hexWidth = 1.154701f; // Adjust to the width of your hex prefab
    public float hexHeight = 0.8659766f; // Adjust to the height of your hex prefab
    public Material topLayerMaterial; // Assign in Inspector
    public Material middleLayerMaterial; // Assign in Inspector
    public Material bottomLayerMaterial; // Assign in Inspector
    public int layers = 3; // Number of layers of hex tiles

    private float noiseOffsetX = 0f;
    private float noiseOffsetZ = 0f;

    private Dictionary<string, GameObject> occupiedPositions = new Dictionary<string, GameObject>();

    void Start()
    {
        GenerateChunk();
    }

    void GenerateChunk()
    {
        // Reset for new chunk generation
        occupiedPositions.Clear();

        float noiseScale = 0.025f; // Controls the variation in elevation
        float maxElevation = 20f; // Controls the maximum height difference

        // Calculate the offset based on the chunk's world position
        // Assuming that chunkSizeX and chunkSizeZ represent the number of tiles in one chunk
        Vector2 chunkOffset = new Vector2(transform.position.x / (hexWidth * 0.75f),
                                          transform.position.z / (hexHeight * Mathf.Sqrt(3f) / 2f));

        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int z = 0; z < chunkSizeZ; z++)
            {

                // Adjust x and z to use global coordinates
                float globalX = (x + chunkOffset.x) * noiseScale;
                float globalZ = (z + chunkOffset.y) * noiseScale;

                // Use Perlin noise to determine the height
                float noiseValue = Mathf.PerlinNoise(globalX + noiseOffsetX,
                                                     globalZ + noiseOffsetZ);

                // Map noiseValue to a height value
                float elevation = noiseValue * maxElevation;
                float highestElevation = Mathf.Floor(elevation);

                for (int y = 0; y <= elevation; y++)
                {
                    Vector3 position = CalculateHexPosition(x, z) + transform.position;
                    position.y += y * 0.5f; // Adjust based on your hex tile model's height

                    GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);

                    // Determine if this tile is at the highest elevation for its (x, z) coordinate.
                    bool isSurfaceTile = (y >= highestElevation);

                    string posKey = CreatePositionKey(position);

                    // Check for overlaps and destroy if necessary
                    if (occupiedPositions.ContainsKey(posKey))
                    {
                        GameObject existingHex = occupiedPositions[posKey];
                        if (existingHex != null) // Check if the GameObject exists
                        {
                            Destroy(existingHex);
                        }

                    }

                    occupiedPositions[posKey] = hex; // This will overwrite any existing entry with the same key

                    // Assign materials based on layer
                    MeshRenderer renderer = hex.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        if (isSurfaceTile)
                        {
                            renderer.material = topLayerMaterial; // Grass for the topmost hex
                        }
                        else if (y > elevation * 0.5f)
                        {
                            renderer.material = middleLayerMaterial; // Dirt
                        }
                        else
                        {
                            renderer.material = bottomLayerMaterial; // Stone
                        }
                    }
                }

            }
        }
    }



    string CreatePositionKey(Vector3 position)
    {
        // Assuming your hexHeight is less than 1 and you want to differentiate increments of 0.5
        int precisionXZ = 100; // Good for x and z axis precision
        int precisionY = 1000; // Must be larger to account for small y increments

        string key = $"{Mathf.RoundToInt(position.x * precisionXZ)}_" +
                     $"{Mathf.RoundToInt(position.y * precisionY)}_" + // Increased precision for y axis
                     $"{Mathf.RoundToInt(position.z * precisionXZ)}";

        return key;
    }

    Vector3 CalculateHexPosition(int x, int z)
    {
        float horizontalSpacing = hexWidth * 0.75f;
        float verticalSpacing = hexHeight * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (z % 2) * (horizontalSpacing / 2);
        float zPos = z * verticalSpacing;

        return new Vector3(xPos, 0, zPos);
    }

}
