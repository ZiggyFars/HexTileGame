using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public GameObject hexChunkPrefab; // Your hex chunk prefab
    public int numChunksX = 10; // Number of chunks in the X direction
    public int numChunksZ = 10; // Number of chunks in the Z direction

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Assuming chunkSizeX and chunkSizeZ refer to the number of hexes in each chunk
        HexChunk chunkScript = hexChunkPrefab.GetComponent<HexChunk>();

        // The width of a chunk is the width of a hexagon multiplied by the number of hexes,
        // minus some overlap because each hexagon is slightly offset horizontally from the next.
        float chunkWidth = chunkScript.chunkSizeX * chunkScript.hexWidth * 0.75f;

        // The height of a chunk is the distance from the center of a hexagon to the center of a hexagon
        // in the next row up, multiplied by the number of rows.
        float chunkHeight = (chunkScript.chunkSizeZ - 1) * (chunkScript.hexHeight * Mathf.Sqrt(3) / 2);

        for (int x = 0; x < numChunksX; x++)
        {
            for (int z = 0; z < numChunksZ; z++)
            {
                // Calculate the horizontal offset for odd rows
                float xOffset = (z % 2 == 0) ? 0 : chunkScript.hexWidth * 0.75f / 2;

                // Apply the offset when calculating the chunk position
                Vector3 chunkPosition = new Vector3(
                    x * chunkWidth + xOffset, // Apply horizontal offset here
                    0,
                    z * chunkHeight // No additional vertical offset necessary
                );

                GameObject newChunk = Instantiate(hexChunkPrefab, chunkPosition, Quaternion.identity);
                // The ChunkManager should handle registering the chunk
                ChunkManager.instance.AddChunk(newChunk);
            }
        }
    }
}