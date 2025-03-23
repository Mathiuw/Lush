using System.Collections;
using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    [Header("Spawn Player")]
    [SerializeField] Transform player;
    [SerializeField] float playerSpawnHeight = 30;
    [SerializeField] LayerMask groundMask;
    [Header("Collision Check")]
    [SerializeField] bool showGizmos = true;
    [SerializeField] float sphereRadius = 3;

    private IEnumerator Start()
    {
        if (!player)
        {
            Debug.LogError("Player not assigned");
            yield return null;
        }

        // Lift player to prevent groun clippiing
        player.position = new Vector3(player.position.x, playerSpawnHeight, player.position.z);

        while (!Physics.Raycast(player.position, Vector3.down, 1000, groundMask)) 
        {
            Debug.LogWarning("Cant find ground");
            yield return new WaitForEndOfFrame();
        }

        if (Physics.Raycast(player.position, Vector3.down, out RaycastHit hit, 1000, groundMask))
        {
            Debug.DrawLine(player.position, hit.point, Color.red, 5f, false);

            // Teleports player to ground
            player.position = new Vector3(player.position.x, hit.point.y + 1.1f, player.position.z);
        }

        // Check player are collinding with anything around itself
        Collider[] colliders = Physics.OverlapSphere(player.position, sphereRadius);

        foreach (Collider collider in colliders) 
        {
            // Destroy any overlaping trees
            if (collider.CompareTag("Tree"))
            {
                Destroy(collider.gameObject);
            }
        }

        // Destroy itself after spawning player
        Destroy(gameObject);
        yield return null;
    }

    // DEBUG Gizmos
    private void OnDrawGizmos()
    {
        if (player && showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, sphereRadius);
        }
    }
}
