using UnityEngine;

public class Seed : MonoBehaviour
{
    private int seedValue;
    public int SeedValue { get => seedValue; private set => seedValue = value; }

    int seedLength = 4;

    private void Awake()
    {
        string value = "";

        for (int i = 0; i < seedLength; i++)
        {
            value += Random.Range(0, 9);
        }

        seedValue = int.Parse(value);

        Debug.Log("seed = " + seedValue);
    }
}