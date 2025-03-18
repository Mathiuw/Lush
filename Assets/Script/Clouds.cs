using System;
using System.Collections;
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
    [SerializeField] float speed = 0.1f;
    float offsetX = 0f;
    float offsetY = 0f;

    Renderer meshRenderer;
    Transform player;
    Texture2D cloudTexture;

    private void Awake()
    {
        cloudTexture = new(size, size);

        meshRenderer = GetComponent<Renderer>();

        // Set texture base transparency
        Color color = meshRenderer.material.color;
        color.a = transparency;
        meshRenderer.material.color = color;
    }

    private IEnumerator Start()
    {
        // Try to find player
        player = FindAnyObjectByType<Player>().transform;

        if (!player)
        {
            Debug.LogError("Clouds cant find player, component disabled");
            enabled = false;
        }

        GenerateTexture();

        meshRenderer.material.mainTexture = cloudTexture;

        while (true)
        {
            // Move cloud texture
            offsetX += speed;
            offsetY += speed;

            GenerateTexture();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        // Follow player
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
    }

    private Texture2D GenerateTexture()
    {
        // Generate perlin noise map for texture
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Color color = CalculateColor(x, y);

                if (cloudTexture.GetPixel(x, y) != color)
                {
                    cloudTexture.SetPixel(x, y, color);
                }                        
            }
        }

        cloudTexture.Apply();
        return cloudTexture;
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