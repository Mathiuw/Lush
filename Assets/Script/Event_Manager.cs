using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct Event
{
    public Transform eventObject;
    public float minutesToTrigger;
}

public class Event_Manager : MonoBehaviour
{
    [SerializeField] List<Event> events = new List<Event>();
    Timer timer;

    private void Start()
    {
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
            if (events.Count == 0) break;

            if ((timer.GetElapsedTime() / 60) > events[i].minutesToTrigger)
            {
                Instantiate(events[i].eventObject, Vector3.zero, Quaternion.identity);
                events.Remove(events[i]);
            }
        }
    }
}
