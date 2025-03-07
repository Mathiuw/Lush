using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] int minutesToComplete = 1;
    float elapsedTime;

    CharacterController playerController;

    private void Start()
    {
        Player player = FindAnyObjectByType<Player>();

        if (!player)
        {
            Debug.LogError("Timer cant find Player, disabling component");
            enabled = false;
        }
        else
        {
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        Vector3 playerVelocity = new Vector3(playerController.velocity.x, 0, playerController.velocity.y);

        if (playerVelocity.magnitude > 0)
        {
            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime / 60 > minutesToComplete)
        {
            Debug.Log("Completed Timer");
            enabled = false;
        }
    }
}
