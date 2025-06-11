using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SoundEvent : MonoBehaviour
{
    public delegate void OnEventEnd();
    public event OnEventEnd onEventEnd;

    AudioSource audioSource;
    Player player;
    Vector3 desiredPosition;
    [SerializeField] bool approaching = false;
    [SerializeField] bool destroyIfTooClose = false;
    [SerializeField] float distanceScale = 1.0f;
    [SerializeField] float distanceToDestroy = .2f;
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

        x = Random.Range(-10, -15) * distanceScale;
        z = Random.Range(-20, 20) * distanceScale;

        desiredPosition = player.transform.position + (player.transform.forward * x) + (player.transform.right * z);
        transform.position = desiredPosition;

        audioSource.Play();
        Invoke("OnAudioEnd", audioSource.clip.length);
    }

    private void Update()
    {
        if (approaching)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * speed);

            if (destroyIfTooClose && Vector3.Distance(transform.position, player.transform.position) < distanceToDestroy)
            {
                OnAudioEnd();
            }
        }
        else 
        {
            transform.position = desiredPosition;
        }

        Debug.DrawLine(player.transform.position, transform.position, Color.yellow);
    }

    private void OnAudioEnd() 
    {
        if (audioSource.loop) return;

        onEventEnd?.Invoke();

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
}