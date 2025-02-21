using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 startPosition = Vector3.zero;
    Hashtable blockContainer = new Hashtable();

    private int xPlayerMove => (int)(player.position.x - startPosition.x);

    private int zPlayerMove => (int)(player.position.z - startPosition.z);

    private int xPlayerLocation => (int)Mathf.Floor(player.position.x);

    private int zPlayerLocation => (int)Mathf.Floor(player.position.z);


    [Header("Terrain")]
    [SerializeField] GameObject voxel;
    [SerializeField] int chunkSize = 20;
    [SerializeField] int noiseHeight = 5;
    [SerializeField] float detailScale = 8;

    [Header("Trees")]
    [SerializeField] bool scatterTrees = true;
    [SerializeField] GameObject tree;
    [SerializeField] int amountPerChunk = 20;

    List<Vector3> blockPositions = new List<Vector3>();
    
    private void Start()
    {
        GenerateTerrain();

        if (scatterTrees)
        {
            SpawnTrees();
        }
    }

    private void Update()
    {
        if (Mathf.Abs(xPlayerMove) >= 1 || Mathf.Abs(zPlayerMove) >= 1)
        {
            GenerateTerrain();
        }
    }

    private void GenerateTerrain() 
    {
        for (int x = -chunkSize; x < chunkSize; x++)
        {
            for (int z = -chunkSize; z < chunkSize; z++)
            {
                // Voxel block position
                Vector3 position = new Vector3(x + xPlayerLocation,
                    GenerateNoise(x + xPlayerLocation, z + zPlayerLocation, detailScale) * noiseHeight,
                    z + zPlayerLocation);

                if (!blockContainer.ContainsKey(position))
                {
                    // Instantiate voxel block
                    GameObject block = Instantiate(voxel, position, Quaternion.identity, transform);

                    // Add block to hashtable
                    blockContainer.Add(position, block);

                    // Add block position to positions list
                    blockPositions.Add(block.transform.position);
                }
            }
        }
    }

    private void SpawnTrees() 
    {
        for (int c = 0; c < amountPerChunk; c++)
        {
            GameObject spawnedTree = Instantiate(tree, GenerateTreeSpawnLocation(), Quaternion.identity, transform);
        }
    }

    private Vector3 GenerateTreeSpawnLocation()
    {
        int randomIndex = Random.Range(0, blockPositions.Count);

        Vector3 newPos = new Vector3(
            blockPositions[randomIndex].x,
            blockPositions[randomIndex].y + 0.5f,
            blockPositions[randomIndex].z
        );
        blockPositions.RemoveAt(randomIndex);

        return newPos;
    }

    private float GenerateNoise(int x, int z, float detailScale) 
    {
        float noiseX = (x + transform.position.x) / detailScale;
        float noiseZ = (z + transform.position.z) / detailScale;

        return Mathf.PerlinNoise(noiseX, noiseZ);        
    }
}