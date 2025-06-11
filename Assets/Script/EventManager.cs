using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct Event
{
    public SoundEvent eventObject;
    public int checkpointIndexTrigger;
    //public float minutesToTrigger;
    public bool triggerEndScene;
}

public class EventManager : MonoBehaviour
{
    [SerializeField] List<Event> events = new List<Event>();
    //Timer timer;
    Checkpoints checkpoints;

    public List<Event> GetEvents() 
    {
        return events;
    }
    
    private void Start()
    {
        // Tries to find timer
        //timer = FindAnyObjectByType<Timer>();

        //if (!timer)
        //{
        //    Debug.LogError("Cant find timer");
        //    enabled = false;
        //}

        checkpoints = FindAnyObjectByType<Checkpoints>();

        if (!checkpoints)
        {
            Debug.LogError("cant find checkpoints");
            enabled = false;
            return;
        }

        checkpoints.OnCheckpointClear += OnCheckpointClear;
    }

    private void OnCheckpointClear()
    {
        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].checkpointIndexTrigger == checkpoints.GetCheckpointIndex())
            {
                SoundEvent soundEvent = Instantiate(events[i].eventObject, Vector3.zero, Quaternion.identity);

                if (events[i].triggerEndScene)
                {
                    soundEvent.onEventEnd += ChangeToEndScene;
                    soundEvent.onEventEnd += SetFadeToBlack;
                }

                events.Remove(events[i]);
            }
            else
            {
                continue;
            }
        }
    }

    //private void Update()
    //{
    //    for (int i = 0; i < events.Count; i++)
    //    {
    //        // Return if there are no sound events to trigger
    //        if (events.Count == 0) break;

    //        // Check for each sound event on the list for if there any event to trigger
    //        if ((timer.GetElapsedTime() / 60) > events[i].minutesToTrigger)
    //        {
    //            // Spawn the sound event
    //            SoundEvent soundEvent = Instantiate(events[i].eventObject, Vector3.zero, Quaternion.identity);

    //            // If the sound event triggers to next scene, the ChangeToEndScene() function will trigger after the event finishes
    //            if (events[i].triggerToEndScene)
    //            {
    //                soundEvent.onEventEnd += ChangeToEndScene;
    //                soundEvent.onEventEnd += SetBlackFade;
    //            }

    //            events.Remove(events[i]);
    //        }
    //    }
    //}

    private void ChangeToEndScene() 
    {
        // Change game scene to the next scene on the build index
        SceneManager.LoadScene("End");
    }

    private void SetFadeToBlack()
    {
        UI_Fade fade = FindAnyObjectByType<UI_Fade>();

        fade.SetFadeValue(1);
        Debug.Log("Set fade to 1");
    }
}