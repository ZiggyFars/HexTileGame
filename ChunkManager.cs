using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;
    public float activationDistance = 100f; // Distance within which chunks are active

    public static ChunkManager instance; // Singleton instance

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddChunk(GameObject chunk)
    {
        if (!chunks.Contains(chunk))
        {
            chunks.Add(chunk);
        }
    }

    private List<GameObject> chunks = new List<GameObject>(); // Initialization at declaration

    void Start()
    {
        PopulateChunkList();
    }

    void PopulateChunkList()
    {
        GameObject[] chunkObjects = GameObject.FindGameObjectsWithTag("ChunkTag"); // Make sure your chunks have "ChunkTag" tag
        chunks.AddRange(chunkObjects);
    }

    void Update()
    {
        foreach (GameObject chunk in chunks)
        {
            float distance = Vector3.Distance(player.position, chunk.transform.position);
            bool currentlyActive = chunk.activeSelf;

            if (distance < activationDistance && IsInView(playerCamera, chunk.transform))
            {
                if (!currentlyActive)
                {
                    chunk.SetActive(true);
                }
            }
            else if (distance >= activationDistance * 1.1f) // Use a slightly larger distance for deactivation to add hysteresis
            {
                if (currentlyActive)
                {
                    chunk.SetActive(false);
                }
            }
        }
    }

    bool IsInView(Camera camera, Transform target)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(target.position);
        // Expand the viewport check to include a buffer around the view
        float buffer = 0.1f; // Adjust this value as needed. A value of 0.1 means 10% buffer around the camera view.

        // Check if the target is within this expanded viewport area
        return viewportPoint.z > 0 && viewportPoint.x > -buffer && viewportPoint.x < 1 + buffer && viewportPoint.y > -buffer && viewportPoint.y < 1 + buffer;
    }
}