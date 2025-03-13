using System;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField] int size = 256;
    [SerializeField] float scale = 20f;
    [Range(0, 1)]
    [SerializeField] float amount = 0.5f;
    [Range(0, 1)]
    [SerializeField] float transparency = 0.5f;
    [Header("Offset")]
    [SerializeField] float offsetX = 0f;
    [SerializeField] float offsetY = 0f;
    Transform player;

    private void Awake()
    {
        Renderer meshRenderer = GetComponent<Renderer>();
        meshRenderer.material.mainTexture = GenerateTexture();

        // Set texture base transparency
        Color color = meshRenderer.material.color;
        color.a = transparency;
        meshRenderer.material.color = color;
    }

    private void Start()
    {
        // Try to find player
        player = FindAnyObjectByType<Player>().transform;

        if (!player)
        {
            Debug.LogError("Clouds cant find player, component disabled");
            enabled = false;
        }
    }

    private void Update()
    {
        // Follow player
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new(size, size);

        // Generate perlin noise map for texture
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Color color = CalculateColor(x, y);

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / size * scale + offsetX;
        float yCoord = (float)y / size * scale + offsetY;

        float colorSample = Mathf.PerlinNoise(xCoord, yCoord);

        if (colorSample <= amount)
        {
            return new Color(colorSample, colorSample, colorSample, 0);
        }

        return new Color(colorSample, colorSample, colorSample);
    }
}