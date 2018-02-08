using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public Area[,] areas;
    public const int tileSize = 4;
    public const int areaSizeInTiles = 5;

    // Use this for initialization
    void Start() {
        areas = CreateAreasFromAreaTypes();
        for (int i = 0; i < areas.GetLength(0); i++)
        {
            for (int j = 0; j < areas.GetLength(1); j++)
            {
                Debug.Log("hi");
                SpawnArea(areas[i, j]);
            }
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public class Area
    {
        public AreaType areaType;
        public Vector2 position;
        public Area()
        {
        }
        public Area(AreaType areaType, Vector2 position)
        {
            this.areaType = areaType;
            this.position = position;
        }
    }
   
    
    public void SpawnArea(Area area)
    {
        GameObject spawnedArea = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = area.areaType.floorColor;
        spawnedArea.GetComponent<Renderer>().sharedMaterial =  mat;
        spawnedArea.transform.parent = this.transform;
        spawnedArea.transform.position = new Vector3(area.position.x, 0, area.position.y);
        spawnedArea.transform.localScale = new Vector3(areaSizeInTiles * tileSize, 1f, tileSize * areaSizeInTiles) / 10;
        spawnedArea.name = area.areaType.name; 

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
                Vector2 position = new Vector2(((width - 1) - i+ 0.5f) * (tileSize) * areaSizeInTiles,((height - 1) - j + 0.5f) * (tileSize) * areaSizeInTiles);
                areas[i, j] = new Area(mapData.areaTypeMap[i, j],position);
            }
        }
        return areas;
    }
}
[CreateAssetMenu()]
public class AreaType : ScriptableObject
{
    public new string name;
    public Color floorColor;
    public GameObject border;

}

