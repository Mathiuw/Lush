using UnityEngine;
using UnityEngine.Playables;

public class EndTrigger : MonoBehaviour
{
    PlayableDirector end_cutscene;
    Transform hud;

    private void Awake()
    {
        end_cutscene = FindAnyObjectByType<PlayableDirector>();

        hud = GameObject.Find("HUD").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hud)
        {
            Destroy(hud.gameObject);
        }

        if (end_cutscene)
        {
            end_cutscene.Play();
        }
        else
        {
            Debug.LogError("Cant find fade image");
        }

        Debug.Log("Game Ended");

        enabled = false;
    }
}
