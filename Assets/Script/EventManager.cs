using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

[Serializable]
public struct Event
{
    public SoundEvent eventObject;
    public float minutesToTrigger;
    public bool triggerToEndScene;
}

public class EventManager : MonoBehaviour
{
    [SerializeField] List<Event> events = new List<Event>();
    Timer timer;

    public List<Event> GetEvents() 
    {
        return events;
    }
    
    private void Start()
    {
        // Tries to find timer
        timer = FindAnyObjectByType<Timer>();

        if (!timer)
        {
            Debug.LogError("Cant find timer");
            enabled = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < events.Count; i++)
        {
            // Return if there are no sound events to trigger
            if (events.Count == 0) break;

            // Check for each sound event on the list for if there any event to trigger
            if ((timer.GetElapsedTime() / 60) > events[i].minutesToTrigger)
            {
                // Spawn the sound event
                SoundEvent soundEvent = Instantiate(events[i].eventObject, Vector3.zero, Quaternion.identity);

                // If the sound event triggers to next scene, the ChangeToEndScene() function will trigger after the event finishes
                if (events[i].triggerToEndScene)
                {
                    soundEvent.onEventEnd += ChangeToEndScene;
                    soundEvent.onEventEnd += SetBlackFade;
                }

                events.Remove(events[i]);
            }
        }
    }

    private void ChangeToEndScene() 
    {
        // Change game scene to the next scene on the build index
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void SetBlackFade()
    {
        UI_Fade fade = FindAnyObjectByType<UI_Fade>();

        fade.SetFadeValue(1);
        Debug.Log("Set fade to 1");
    }
}