using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct Event
{
    public SoundEvent eventObject;
    public int checkpointIndexTrigger;
    public bool triggerEndScene;
}

public class EventManager : MonoBehaviour
{
    [SerializeField] List<Event> events = new List<Event>();
    Checkpoints checkpoints;

    public List<Event> GetEvents() 
    {
        return events;
    }
    
    private void Start()
    {
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
                i--;
            }
            else
            {
                continue;
            }
        }
    }

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