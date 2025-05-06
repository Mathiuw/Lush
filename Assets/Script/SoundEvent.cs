using UnityEngine;

public class SoundEvent : MonoBehaviour
{
    [SerializeField]AudioSource AudioSource;
    Player player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (!player)
        {
            Debug.LogError("Cant find player");
            enabled = false;
        }
    }

    private void Update()
    {
        transform.position = player.transform.position + (player.transform.forward * -5);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
