using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class HallwayGenerator : MonoBehaviour
{
    [Header("Hallway Generation")]
    [SerializeField] Transform hallwayPrefab;
    [SerializeField] Transform hallwayPipes;
    [SerializeField] int hallwayChunkSize = 4;
    [SerializeField] int hallwayAmount = 4;
    Transform playerTransform;
    Hashtable hallwayPositions = new Hashtable();
    Grid grid;

    [Header("End game setting")]
    [SerializeField] Transform endPrefab;
    [SerializeField] uint distanceToTriggerEnd = 100;
    float distanceWalked;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        grid.cellSize = new Vector3(hallwayChunkSize, hallwayChunkSize, hallwayChunkSize);
    }

    private void Start()
    {
        Player player = FindAnyObjectByType<Player>();

        if (!player)
        {
            GameObject worldPivot = new GameObject("World_Pivot");
            worldPivot.transform.position = Vector3.zero;
            playerTransform = worldPivot.transform;
        }
        else
        {
            playerTransform = player.transform;
        }

        GenerateHallway();
    }

    private void Update()
    {
        GenerateHallway();

        // Calculate the player distance from start point
        distanceWalked = Vector3.Distance(Vector3.zero, playerTransform.position);
        Debug.Log("Distance walked: " + distanceWalked);
    }

    private void GenerateHallway()
    {
        float cTime = Time.realtimeSinceStartup;

        for (int z = -hallwayAmount; z < hallwayAmount + 1; z++)
        {
            Vector3 spawnPosition = new Vector3(0, 2, (z + grid.WorldToCell(playerTransform.position).z) * hallwayChunkSize);

            if (!hallwayPositions.ContainsKey(spawnPosition))
            {
                // Instantiate hallway chunk
                Transform hallwayTransform = Instantiate(hallwayPrefab, spawnPosition, Quaternion.identity, transform);

                HallwayChunk hallwayChunk = new HallwayChunk(cTime, hallwayTransform);

                // Add hallway chink to hastable
                hallwayPositions.Add(spawnPosition, hallwayChunk);
            }
            else
            {
                ((HallwayChunk)hallwayPositions[spawnPosition]).cTimeStamp = cTime;
            }
        }

        Hashtable newChunkPositions = new Hashtable();

        foreach (HallwayChunk hallwayChunk in hallwayPositions.Values)
        {
            if (!hallwayChunk.cTimeStamp.Equals(cTime))
            {
                Destroy(hallwayChunk.hallwayTransform.gameObject);
            }
            else
            {
                newChunkPositions.Add(hallwayChunk.hallwayTransform.position, hallwayChunk);
            }
        }

        hallwayPositions = newChunkPositions;

        // End game trigger
        if (distanceWalked > distanceToTriggerEnd)
        {
            // Spawn final room
            // Front
            Vector3 spawnPositionFront = new Vector3(0, 2, ((hallwayAmount + 1) + grid.WorldToCell(playerTransform.position).z) * hallwayChunkSize);
            Instantiate(endPrefab, spawnPositionFront, Quaternion.Euler(0, 180, 0), transform);
            Instantiate(hallwayPipes, spawnPositionFront, Quaternion.Euler(0, 0, 0), transform);
            // Back
            Vector3 spawnPositionBack = new Vector3(0, 2, ((-hallwayAmount - 1) + grid.WorldToCell(playerTransform.position).z) * hallwayChunkSize);
            Instantiate(endPrefab, spawnPositionBack, Quaternion.identity, transform);
            Instantiate(hallwayPipes, spawnPositionBack, Quaternion.Euler(0, 0, 0), transform);
            Debug.Log("Hallway generation ended");

            enabled = false;
        }
    }

    private class HallwayChunk 
    {
        public float cTimeStamp;
        public Transform hallwayTransform;
        
        public HallwayChunk(float cTimeStamp, Transform hallwayTransform) 
        {
            this.cTimeStamp = cTimeStamp;
            this.hallwayTransform = hallwayTransform;
        }
    }

    private void OnDrawGizmos()
    {
        if (grid)
        {
            for (int z = -hallwayAmount; z < hallwayAmount + 1; z++)
            {
                Vector3 spawnPosition = new Vector3(0, 2, (z + grid.WorldToCell(playerTransform.position).z) * hallwayChunkSize);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPosition, .25f);
            }
        }
    }
}