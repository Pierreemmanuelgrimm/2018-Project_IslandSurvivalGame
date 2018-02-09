using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public Area[,] areas;
    public const int tileSize = 4;
    public const int areaSizeInTiles = 5;

    // Use this for initialization
    void Start()
    {
        areas = CreateAreasFromAreaTypes();

    }

    // Update is called once per frame
    void Update()
    {

    }

    Area SpawnArea(Vector2 Coord, AreaType areaType, Vector2 position)
    {
        GameObject spawnedArea = GameObject.CreatePrimitive(PrimitiveType.Plane);      
        Area area = spawnedArea.AddComponent<Area>();
        area.Define(areaType, position);
        
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = area.areaType.floorColor;
        spawnedArea.GetComponent<Renderer>().sharedMaterial = mat;
        spawnedArea.transform.parent = this.transform;
        spawnedArea.transform.position = new Vector3(area.position.x, 0, area.position.y);
        spawnedArea.transform.localScale = new Vector3(areaSizeInTiles * tileSize, 1f, tileSize * areaSizeInTiles) / 10;
        spawnedArea.name = "[" + Coord.x.ToString() + "," + Coord.y.ToString() + "] " + area.areaType.name;
        return area;

    }
    public Area[,] CreateAreasFromAreaTypes()
    {
        MapData mapData = (FindObjectOfType<MapGenerator>()).mapData;
        Debug.Log(mapData.areaTypeMap.GetLength(0));
        Debug.Log(mapData.areaTypeMap.GetLength(1));
        int width = mapData.mapWidth;
        int height = mapData.mapHeight;
        Debug.Log(width);
        Debug.Log(height);
        Area[,] areas = new Area[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 position = new Vector2(((width - 1) - i + 0.5f) * (tileSize) * areaSizeInTiles, ((height - 1) - j + 0.5f) * (tileSize) * areaSizeInTiles);
                areas[i,j] = SpawnArea(new Vector2(i,j),mapData.areaTypeMap[i, j], position);    
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {             
                areas[i, j].areaTeleporters = new AreaTeleporter[4];
                if (!areas[i, j].areaType.isAccessible) continue;
                if (i != (width - 1))
                {
                    if (areas[i + 1, j].areaType.isAccessible)
                        areas[i, j].areaTeleporters[(int)Direction.West] = CreateAreaTeleporter(Direction.West, areas[i, j]);
                }
                if (i != 0)
                {
                    if (areas[i - 1, j].areaType.isAccessible)
                        areas[i, j].areaTeleporters[(int)Direction.East] = CreateAreaTeleporter(Direction.East, areas[i, j]);
                }
                if (j != 0)
                {
                    if (areas[i, j - 1].areaType.isAccessible)
                        areas[i, j].areaTeleporters[(int)Direction.North] = CreateAreaTeleporter(Direction.North, areas[i, j]);
                }
                if (j != (height - 1))
                {
                    if (areas[i, j + 1].areaType.isAccessible)
                        areas[i, j].areaTeleporters[(int)Direction.South] = CreateAreaTeleporter(Direction.South, areas[i, j]);
                }
            }           
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < areas[i, j].areaTeleporters.Length; k++)
                {
                    AreaTeleporter teleporter = areas[i, j].areaTeleporters[k];
                    if (teleporter == null) continue;
                    switch (teleporter.direction)
                    {
                        case Direction.North:
                            teleporter.SetDestination(areas[i, j - 1].areaTeleporters[(int)Direction.South]);
                            break;
                        case Direction.South:
                            teleporter.SetDestination(areas[i, j + 1].areaTeleporters[(int)Direction.North]);
                            break;
                        case Direction.West:
                            teleporter.SetDestination(areas[i + 1, j].areaTeleporters[(int)Direction.East]);
                            break;
                        case Direction.East:
                            teleporter.SetDestination(areas[i - 1, j].areaTeleporters[(int)Direction.West]);
                            break;

                    }
                }
            }
        }
            return areas;
    }
    AreaTeleporter CreateAreaTeleporter(Direction direction, Area currentArea)
    {
        GameObject go = new GameObject();
        AreaTeleporter areaT =  go.AddComponent<AreaTeleporter>();
        BoxCollider boxC = go.AddComponent<BoxCollider>();
        Vector3 position = new Vector3(currentArea.position.x,0, currentArea.position.y);
        Vector3 rotation = 90 * Vector3.up;

        const float colliderDepth = 0.1f;
        const float colliderHeight = 0.4f;

        Vector3 localScale = tileSize * areaSizeInTiles * new Vector3(1, colliderHeight, colliderDepth);
        position.y += tileSize * areaSizeInTiles * colliderHeight / 2;
        switch (direction)
        {
            case Direction.North:
                go.name = "North";
                position.z += tileSize * areaSizeInTiles * (0.5f - colliderDepth/2);
                rotation *= 0;
                break;
            case Direction.South:
                go.name = "South";
                position.z -= tileSize * areaSizeInTiles * (0.5f - colliderDepth / 2);
                rotation *= 0;
                break;
            case Direction.East:
                go.name = "East";
                position.x += tileSize * areaSizeInTiles * (0.5f - colliderDepth / 2);
                break;
            case Direction.West:
                go.name = "West";
                position.x -= tileSize * areaSizeInTiles * (0.5f - colliderDepth / 2);
                break;
        }
        go.transform.position = position;
        go.transform.localEulerAngles = rotation;
        boxC.size = localScale;
        boxC.isTrigger = true;
        areaT.Set(direction, currentArea);
        go.transform.parent = currentArea.transform;
        return areaT;
    }
}
[CreateAssetMenu()]
public class AreaType : ScriptableObject
{
    public new string name;
    public Color floorColor;
    public bool isAccessible = true;

}
public class Area : MonoBehaviour
{
    public AreaType areaType;
    public Vector2 position;
    public AreaTeleporter[] areaTeleporters;
    public Area()
    {
    }
    public Area(AreaType areaType, Vector2 position)
    {
        this.areaType = areaType;
        this.position = position;
    }
    public void Define(AreaType areaType, Vector2 position)
    {
        this.areaType = areaType;
        this.position = position;
    }
}
