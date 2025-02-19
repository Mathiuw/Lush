using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] GameObject voxel;

    int worldSizeX = 40;
    int worldSizeY = 40;

    int noiseHeight = 5;

    float gridOffset = 1.0f;

    private void Start()
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int z = 0; z < worldSizeY; z++)
            {
                Vector3 pos = new Vector3(x * gridOffset, GenerateNoise(x, z, 8f) * noiseHeight, z * gridOffset);

                Instantiate(voxel, pos, Quaternion.identity, transform);
            }
        }
    }

    private float GenerateNoise(int x, int z, float detailScale) 
    {
        float noiseX = (x + transform.position.x) / detailScale;
        float noiseZ = (z + transform.position.z) / detailScale;

        return Mathf.PerlinNoise(noiseX, noiseZ);        
    }

}
