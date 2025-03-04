using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 startPosition = Vector3.zero;
    Hashtable chunkPositions = new Hashtable();

    private int xPlayerMoved => (int)(player.position.x - startPosition.x);

    private int zPlayerMoved => (int)(player.position.z - startPosition.z);

    public int xPlayerLocation => (int)Mathf.Floor(player.position.x);

    public int zPlayerLocation => (int)Mathf.Floor(player.position.z);

    [SerializeField] bool showGizmos = false;

    [Header("Terrain")]
    [SerializeField] bool oneTimeGeneration = false;
    [SerializeField] GameObject chunk;
    [SerializeField] int worldSize = 3;
    [SerializeField] int noiseHeight = 5;
    [SerializeField] float detailScale = 8;
    Grid grid;

    [Header("Chunk")]
    [SerializeField] int chunkSize = 20;

    [Header("Trees")]
    [SerializeField] bool scatterTrees = true;
    [SerializeField] GameObject tree;
    [SerializeField] int amountPerChunk = 10;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        grid.cellSize = new Vector3(chunkSize, chunkSize, chunkSize);

        GenerateTerrain();
    }

    private void Update()
    {
        if (oneTimeGeneration) return;

        if (Mathf.Abs(xPlayerMoved) >= chunkSize || Mathf.Abs(zPlayerMoved) >= chunkSize)
        {
            GenerateTerrain();
        }
    }

    private void GenerateTerrain() 
    {
        Hashtable newChunks = new Hashtable();
        float cTime = Time.realtimeSinceStartup;

        for (int x = -worldSize; x < worldSize; x++)
        {
            for (int z = -worldSize; z < worldSize; z++)
            {
                // chunk position
                Vector3 position = new Vector3((x + grid.WorldToCell(player.position).x) * chunkSize,
                                0,
                                (z + grid.WorldToCell(player.position).z) * chunkSize);

                if (!chunkPositions.ContainsKey(position))
                {
                    // Instantiate chunk game object
                    GameObject chunkPrefab = Instantiate(this.chunk, position, Quaternion.identity, transform);

                    Chunk chunk = new Chunk(cTime, chunkPrefab, chunkSize, this);

                    // Add chunk to hashtable
                    chunkPositions.Add(position, chunk);
                }
                else
                {
                    ((Chunk)chunkPositions[position]).cTimeStamp = cTime;
                }
            }
        }

        foreach (Chunk chunk in chunkPositions.Values)
        {
            if (!chunk.cTimeStamp.Equals(cTime))
            {
                Destroy(chunk.chunkObject);
            }
            else
            {
                newChunks.Add(chunk.chunkObject, chunk);
            }
        }

        chunkPositions = newChunks;
        startPosition = player.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (grid && showGizmos)
        {
            for (int x = -worldSize; x < worldSize; x++)
            {
                for (int z = -worldSize; z < worldSize; z++)
                {
                    // chunk position
                    Vector3 position = new Vector3((x + grid.WorldToCell(player.position).x) * chunkSize,
                                                    0,
                                                    (z + grid.WorldToCell(player.position).z) * chunkSize);

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(position, 0.5f);
                }
            }
        }
    }

    private class Chunk
    {
        int size;
        public float cTimeStamp;
        public GameObject chunkObject;
        VoxelGenerator voxelGenerator;
        Mesh mesh;

        List<Vector3> treeVerticePositions = new List<Vector3>();

        public Chunk(float cTimeStamp, GameObject chunkObject, int size, VoxelGenerator voxelGenerator)
        {
            this.cTimeStamp = cTimeStamp;
            this.chunkObject = chunkObject;
            this.size = size;
            this.voxelGenerator = voxelGenerator;

            mesh = new Mesh();
            chunkObject.GetComponent<MeshFilter>().mesh = mesh;

            CreateMesh();

            chunkObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void CreateMesh()
        {
            Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
            int[] triangles = new int[size * size * 6];
            Vector2[] uv = new Vector2[vertices.Length];

            // Create mesh vertices
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    float y = GenerateNoise(x , z, voxelGenerator.detailScale) * voxelGenerator.noiseHeight;
                    vertices[i] = new Vector3(x, y, z);

                    // Add chunk position to positions list
                    treeVerticePositions.Add(vertices[i]);

                    i++;
                }
            }

            // Create UV for mesh
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            // Set mesh triangles
            int vert = 0;
            int tris = 0;

            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + size + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + size + 1;
                    triangles[tris + 5] = vert + size + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }

            // Update mesh
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            // Spawn trees
            if (voxelGenerator.scatterTrees)
            {
                SpawnTrees();
            }
        }

        private float GenerateNoise(int x, int z, float detailScale)
        {
            float noiseX = (x + chunkObject.transform.position.x) / detailScale;
            float noiseZ = (z + chunkObject.transform.position.z) / detailScale;

            return Mathf.PerlinNoise(noiseX, noiseZ);
        }

        private void SpawnTrees()
        {
            for (int c = 0; c < voxelGenerator.amountPerChunk; c++)
            {
                GameObject spawnedTree = Instantiate(voxelGenerator.tree, GenerateTreeSpawnLocation(), Quaternion.identity, chunkObject.transform);
            }
        }

        private Vector3 GenerateTreeSpawnLocation()
        {
            int randomIndex = Random.Range(0, treeVerticePositions.Count);

            Vector3 newPos = new Vector3(
                treeVerticePositions[randomIndex].x + chunkObject.transform.position.x,
                treeVerticePositions[randomIndex].y,
                treeVerticePositions[randomIndex].z + chunkObject.transform.position.z
            );
            treeVerticePositions.RemoveAt(randomIndex);

            return newPos;
        }
    }
}