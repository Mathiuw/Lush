using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 startPosition = Vector3.zero;
    Hashtable voxelPositions = new Hashtable();

    private int xPlayerMove => (int)(player.position.x - startPosition.x);

    private int zPlayerMove => (int)(player.position.z - startPosition.z);

    private int xPlayerLocation => (int)Mathf.Floor(player.position.x);

    private int zPlayerLocation => (int)Mathf.Floor(player.position.z);

    [Header("Terrain")]
    [SerializeField] GameObject voxel;
    [SerializeField] int worldSize = 20;
    [SerializeField] int noiseHeight = 5;
    [SerializeField] float detailScale = 8;

    [Header("Chunk")]
    [SerializeField] int chunkSize = 20;

    [Header("Trees")]
    [SerializeField] bool scatterTrees = true;
    [SerializeField] GameObject tree;
    [SerializeField] int amountPerChunk = 20;

    List<Vector3> treeVoxelPositions = new List<Vector3>();
    
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
        if (Mathf.Abs(xPlayerMove) >= chunkSize || Mathf.Abs(zPlayerMove) >= chunkSize)
        {
            GenerateTerrain();
        }
    }

    private void GenerateTerrain() 
    {
        Hashtable newTiles = new Hashtable();
        float cTime = Time.realtimeSinceStartup;

        for (int x = -worldSize; x < worldSize; x++)
        {
            for (int z = -worldSize; z < worldSize; z++)
            {
                // Voxel block position
                Vector3 position = new Vector3((x * chunkSize) + xPlayerLocation,
                   /* GenerateNoise(x + xPlayerLocation, z + zPlayerLocation, detailScale) * noiseHeight*/0,
                    (z * chunkSize) + zPlayerLocation);

                if (!voxelPositions.ContainsKey(position))
                {
                    // Instantiate voxel block
                    GameObject block = Instantiate(voxel, position, Quaternion.identity, transform);

                    Tile tile = new Tile(cTime, block);

                    // Add block to hashtable
                    voxelPositions.Add(position, tile);

                    // Add block position to positions list
                    treeVoxelPositions.Add(block.transform.position);
                }
                else
                {
                    ((Tile)voxelPositions[position]).cTimeStamp = cTime;
                }
            }
        }

        foreach (Tile tile in voxelPositions.Values)
        {
            if (!tile.cTimeStamp.Equals(cTime))
            {
                Destroy(tile.tileObject);
            }
            else
            {
                newTiles.Add(tile.tileObject, tile);
            }
        }

        voxelPositions = newTiles;
        startPosition = player.transform.position;
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
        int randomIndex = Random.Range(0, treeVoxelPositions.Count);

        Vector3 newPos = new Vector3(
            treeVoxelPositions[randomIndex].x,
            treeVoxelPositions[randomIndex].y + 0.5f,
            treeVoxelPositions[randomIndex].z
        );
        treeVoxelPositions.RemoveAt(randomIndex);

        return newPos;
    }

    private float GenerateNoise(int x, int z, float detailScale) 
    {
        float noiseX = (x + transform.position.x) / detailScale;
        float noiseZ = (z + transform.position.z) / detailScale;

        return Mathf.PerlinNoise(noiseX, noiseZ);        
    }

    private class Tile 
    {
        public float cTimeStamp;
        public GameObject tileObject;

        public Tile(float cTimeStamp, GameObject tileObject)
        {
            this.cTimeStamp = cTimeStamp;
            this.tileObject = tileObject;
        }
    }
}