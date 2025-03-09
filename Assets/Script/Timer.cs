using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] int minutesToComplete = 1;
    CharacterController playerController;
    float elapsedTime;

    public delegate void OnTimerComplete();
    public static event OnTimerComplete onTimerComplete;

    public float GetElapsedTime() 
    {
        return elapsedTime; 
    }

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

        onTimerComplete += OnTimerCompleteFunc;
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
            onTimerComplete?.Invoke();
            enabled = false;
        }
    }

    private void OnTimerCompleteFunc() 
    {
        Debug.Log("Completed Timer");
    }
}
