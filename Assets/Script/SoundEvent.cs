using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SoundEvent : MonoBehaviour
{
    AudioSource audioSource;
    Player player;
    Vector3 desiredPosition;
    [SerializeField] bool approaching = false;
    [SerializeField] float speed = 1.0f;
    float x;
    float z;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (!player)
        {
            Debug.LogError("Cant find player");
            enabled = false;
        }

        x = Random.Range(-10, -15);
        z = Random.Range(-20, 20);

        desiredPosition = player.transform.position + (player.transform.forward * x) + (player.transform.right * z);
        transform.position = desiredPosition;

        audioSource.Play();
    }

    private void Update()
    {
        if (approaching)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * speed);
        }
        else 
        {
            transform.position = desiredPosition;
        }

        Debug.DrawLine(player.transform.position, transform.position, Color.yellow);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
