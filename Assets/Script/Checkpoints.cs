using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public delegate void OnCheckpoint();
    public event OnCheckpoint OnCheckpointClear;

    Transform player;
    Vector3 currentCheckpointStartPosition;
    [SerializeField] float[] checkpointsDistance;
    int checkpointIndex = 0;

    public int GetCheckpointIndex() 
    {
        return checkpointIndex;
    }

    private void Start()
    {
        // Find player
        player = FindAnyObjectByType<Player>().transform;

        if (!player)
        {
            Debug.LogError("Cant find player");
            enabled = false;
        }

        // Checkpoint starts measuring at player's starting position
        currentCheckpointStartPosition = transform.position;
    }

    private void Update()
    {
        float distance = Vector3.Distance(currentCheckpointStartPosition, player.position);
        Debug.Log("Walked " + distance + " out of " + checkpointsDistance[checkpointIndex]);

        // Check if player cleared the checkpoint and if there are any checkpoints left
        if (distance > checkpointsDistance[checkpointIndex])
        {
            // Trigger event
            OnCheckpointClear?.Invoke();
            Debug.Log("Checkpoint " + checkpointIndex + " cleared");

            // Advance to next checkpoint
            checkpointIndex++;

            if (checkpointIndex >= checkpointsDistance.Length)
            {
                // Destroy the component if the checkpoints are over
                Debug.LogWarning("Checkpoints ended");
                Destroy(gameObject);
                return;
            }

            // The new starting point is player's location
            currentCheckpointStartPosition = player.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        // Draw sphere at current checkpoint start position
        Gizmos.DrawSphere(currentCheckpointStartPosition, 0.5f);

        if (player)
        {
            // Draw radius where the player should walk out of
            Gizmos.DrawWireSphere(currentCheckpointStartPosition, checkpointsDistance[checkpointIndex]);

            // Draw at the player position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(player.position, 0.5f);
        }
    }
}
