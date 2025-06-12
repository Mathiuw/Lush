using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class EndTrigger : MonoBehaviour
{
    PlayableDirector end_cutscene;
    Transform hud;

    [SerializeField] Transform focusTransform;
    [SerializeField] AudioSource glitchSound;

    private void Awake()
    {
        end_cutscene = FindAnyObjectByType<PlayableDirector>();

        hud = GameObject.Find("HUD").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (hud)
        //{
        //    Destroy(hud.gameObject);
        //}

        //if (end_cutscene)
        //{
        //    end_cutscene.Play();
        //}
        //else
        //{
        //    Debug.LogError("Cant find fade image");
        //}

        StartCoroutine(EndGameCoroutine(other));

        //enabled = false;
    }

    IEnumerator EndGameCoroutine(Collider other) 
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            // Disable player input
            player.GetInput().Disable();
            // Focus player camera on hanging rope
            player.transform.LookAt(focusTransform);
            // Play glitching sound
            glitchSound.Play();
        }

        yield return new WaitForSeconds(1.0f);

#if UNITY_EDITOR
        glitchSound.Stop();
#endif

        Debug.Log("Game Ended");
        Application.Quit();

        yield return null;
    }
}
