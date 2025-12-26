using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [HideInInspector] public List<MapTile> mapTiles;
    public EMapTileType[,] mapTileData;
    public int mamSizeX;
    public int mapSizeY;
    public Dictionary<EMapTileType, GameObject> mapTilePrefab;
    public List<EMapTileType> eMapTileTypeInInspectorSpecial;
    public List<GameObject> mapTilePrefabInInspectorSpecial;
    public List<EMapTileType> eMapTileTypeInInspectorRandom;
    public List<GameObject> mapTilePrefabInInspectorRandom;
    public Transform playerTransform;

    void Awake()
    {
        //// Initialize the dictionary from the inspector lists
        //int index = 0;

        //mapTilePrefab = new Dictionary<EMapTileType, GameObject>();
        //for (; index < Mathf.Min(eMapTileTypeInInspector.Count, mapTilePrefabInInspector.Count); index++)
        //{
        //    mapTilePrefab[eMapTileTypeInInspector[index]] = mapTilePrefabInInspector[index];
        //}

        //for (; index < Mathf.Min(eMapTileTypeInInspector.Count, mapTilePrefabInInspector.Count); i++)
        //{
        //    mapTilePrefab[eMapTileTypeInInspector[i]] = mapTilePrefabInInspector[i];
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        





    }


}