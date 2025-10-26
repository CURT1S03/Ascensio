using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
Code from: https://johnleonardfrench.music/terrain-footsteps-in-unity-how-to-detect-different-textures/
***/

public class CheckTerrainTexture : MonoBehaviour
{
    public Terrain t;
    public int posX;
    public int posZ;
    public float[] textureValues;

    void Start()
    {
        t = Terrain.activeTerrain;
    }

    void Update()
    {

    }

    public void GetTerrainTexture()
    {
        ConvertPosition(this.gameObject.transform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;
        Vector3 mapPosition = new (terrainPosition.x / t.terrainData.size.x, 0,
            terrainPosition.z / t.terrainData.size.z);
        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;
        posX = (int)xCoord;
        posZ = (int)zCoord;
    }
    
    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        textureValues[0] = aMap[0, 0, 0];
        textureValues[1] = aMap[0, 0, 1];
        textureValues[2] = aMap[0, 0, 2];
        textureValues[3] = aMap[0, 0, 3];
    }
}
