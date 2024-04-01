using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class RaiseTerrainHeightmap : MonoBehaviour
    {
public Terrain myTerrain;
    private TerrainData terrainData;
    private int heightmapWidth;
    private int heightmapHeight;
    private float[,] heightmapData = new float[0, 0];

    public float raiseHeightInUnits = 20.0f;

    [Button]
    public void RaiseHeights()
    {
        // - GetTerrainData -

        if (!myTerrain)
        {
            //myTerrain = Terrain.activeTerrain;
            Debug.LogError(gameObject.name + " has no terrain assigned in the inspector");
        }

        terrainData = myTerrain.terrainData;
        heightmapWidth = myTerrain.terrainData.heightmapResolution;
        heightmapHeight = myTerrain.terrainData.heightmapResolution;

        // --

        // store old heightmap data
        heightmapData = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        float terrainHeight = terrainData.size.y;

        // --

        var y = 0;
        var x = 0;

        // raise heights
        for (y = 0; y < heightmapHeight; y++)
        {
            for (x = 0; x < heightmapWidth; x++)
            {
                float newHeight = Mathf.Clamp01(heightmapData[y, x] + (raiseHeightInUnits / terrainHeight));

                heightmapData[y, x] = newHeight;
            }
        }

        terrainData.SetHeights(0, 0, heightmapData);

        Debug.Log("RaiseHeights() completed");
    }
    }
}
