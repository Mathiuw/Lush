using System;
using System.Collections;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField] int size = 256;
    float scale = 50f;
    [SerializeField] float zoom = 1.0f;
    [Range(0, 1)]
    [SerializeField] float amount = 0.5f;
    [Header("Offset")]
    [SerializeField] float speed = 0.1f;
    public float tileOffset;
    public float offset;

    Renderer meshRenderer;
    Transform player;
    Texture2D cloudTexture;

    private void Awake()
    {
        cloudTexture = new(size, size);

        meshRenderer = GetComponent<Renderer>();
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

        // Set seed
        Seed seed = FindAnyObjectByType<Seed>();

        if (seed)
        {
            offset = seed.SeedValue;
        }
        else
        {
            Debug.LogError("Cloud cant find Seed class");
        }

        GenerateTexture();

        meshRenderer.material.mainTexture = cloudTexture;
    }

    // TODO: OPTIMIZATION
    private void Update()
    {
        // Follow player
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        
        // Set texture scale
        meshRenderer.material.SetTextureScale(meshRenderer.material.GetTexturePropertyNameIDs()[1], new Vector2(zoom, zoom));

        // Move cloud texture
        tileOffset += speed * Time.deltaTime;

        meshRenderer.material.SetTextureOffset(meshRenderer.material.GetTexturePropertyNameIDs()[1], new Vector2(tileOffset, tileOffset));
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
        float xCoord = (float)x / size * scale + offset;
        float yCoord = (float)y / size * scale + offset;

        float colorSample = Mathf.PerlinNoise(xCoord, yCoord);

        if (colorSample <= amount)
        {
            return new Color(colorSample, colorSample, colorSample, 0);
        }

        return new Color(colorSample, colorSample, colorSample);
    }
}