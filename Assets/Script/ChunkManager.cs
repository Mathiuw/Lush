using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] MeshGenerator ChunkPrefab; 
    [SerializeField] int drawDistance = 2;
    Player player;
    Vector2 playerPosition = Vector2.zero;

    List<Transform> chunks = new List<Transform>();

    // DEBUG
    [SerializeField] Transform testGridPlayer;
    Vector2 nearestGridPoint = Vector2.zero;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    private void Update()
    {
        if (player)
        {
            playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        }

        CheckChunks();

        GetNearestGridPoint(testGridPlayer.position);
    }

    private void GetNearestGridPoint(Vector3 basePosition) 
    {
        int positionX = (int)Mathf.Round(basePosition.x);
        int positionZ = (int)Mathf.Round(basePosition.z);

        while (positionX % 20 != 0)
        {
            if (positionX % 20 <= 10)
            {
                positionX--;
            }
            else
            {
                positionX++;
            }
        }

        while (positionZ % 20 != 0)
        {
            if (positionZ % 20 <= 10)
            {
                positionZ--;
            }
            else
            {
                positionZ++;
            }
        }

        nearestGridPoint = new Vector2(positionX, positionZ);

        Debug.Log(nearestGridPoint);
    }

    private void CheckChunks()
    {
        foreach (Transform chunk in chunks) 
        {
            
            
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(nearestGridPoint.x, 0, nearestGridPoint.y), 0.25f);
    }
}
