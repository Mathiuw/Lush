using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] GameObject voxel;
    [SerializeField] int worldSizeX = 20;
    [SerializeField] int worldSizeY = 20;
    [SerializeField] int noiseHeight = 5;
    [SerializeField] float detailScale = 8;
    [SerializeField] float gridOffset = 1.0f;

    [Header("Trees")]
    [SerializeField] bool scatterTrees = true;
    [SerializeField] GameObject tree;
    [SerializeField] int amountPerChunk = 20;

    List<Vector3> blockPositions = new List<Vector3>();

    private void Start()
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int z = 0; z < worldSizeY; z++)
            {
                // Voxel block position
                Vector3 pos = new Vector3(x * gridOffset, GenerateNoise(x, z, detailScale) * noiseHeight, z * gridOffset);

                // Instantiate voxel block
                GameObject block = Instantiate(voxel, pos, Quaternion.identity, transform);

                // Add block position to positions list
                blockPositions.Add(block.transform.position);
            }
        }

        SpawnTrees();
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
