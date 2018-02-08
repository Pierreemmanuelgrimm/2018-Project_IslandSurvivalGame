using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 10;
    public int mapHeight = 10;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;

    public bool useFalloff;
    public bool autoUpdate;

    public AnimationCurve falloffAnimationCurve;
    public TerrainType[] regions;

    public AreaType[] areaTypes;

    public MapData mapData;

    float[,] falloffMap;


    void Awake()
    {
        falloffMap = Noise.GenerateFalloffMap(mapWidth, mapHeight, falloffAnimationCurve);
        mapData = GenerateMapData(mapWidth, mapHeight);
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(mapWidth,mapHeight);
        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapWidth, mapHeight));
 
    }
    public MapData GenerateMapData(int mapWidth, int mapHeight)
    {
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity);
        Color[] colourMap = new Color[mapWidth * mapHeight];
        AreaType[,] areaTypeMap = new AreaType[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - falloffMap[x, y], 0, 1);
                }
                for (int i = 0; i < regions.Length; i++)
                {
                    if (noiseMap[x, y] >= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        areaTypeMap[x, y] = regions[i].areaType;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(mapWidth, mapHeight, noiseMap, colourMap,areaTypeMap);

    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        if(mapHeight <= 0)
        {
            mapHeight = 1;
        }
        if(mapWidth <= 0)
        {
            mapHeight = 1;
        }

        falloffMap = Noise.GenerateFalloffMap(mapWidth,mapHeight, falloffAnimationCurve);
    }

}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
    public AreaType areaType;
}
public struct MapData
{
    public readonly int mapWidth;
    public readonly int mapHeight;
    public readonly AreaType[,] areaTypeMap;
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(int mapWidth, int mapHeight, float[,] heightMap, Color[] colourMap, AreaType[,] areaTypeMap)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.heightMap = heightMap;
        this.colourMap = colourMap;
        this.areaTypeMap = areaTypeMap;
    }
}