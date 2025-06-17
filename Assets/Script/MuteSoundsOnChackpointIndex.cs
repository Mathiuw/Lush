using UnityEngine;

public class MuteSoundsOnChackpointIndex : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] Transform soundContainer;
    [Header("Checkpoint index to mute sounds")]
    [SerializeField] int indexToMute;
    [Header("Sounds")]
    [SerializeField] string[] soundNames;
    Checkpoints checkpoints;

    private void Start()
    {
        checkpoints = GetComponent<Checkpoints>();

        if (checkpoints)
        {
            checkpoints.OnCheckpointClear += OnCheckpointClear;
        }
    }

    private void OnCheckpointClear()
    {
        if (checkpoints.GetCheckpointIndex() == indexToMute)
        {
            foreach (var soundName in soundNames)
            {
                Transform soundTransform = soundContainer.Find(soundName);

                if (soundTransform)
                {
                    Debug.Log(soundTransform.name + " sound destroyed");
                    Destroy(soundTransform.gameObject);
                }
                else
                {
                    Debug.LogWarning("No sound was found");
                }
            }
        }
    }
}
