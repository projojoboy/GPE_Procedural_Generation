using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EnumCollection;

public class WorldManager : MonoBehaviour
{
    static int worldSeed = 1;

    [SerializeField]
    private int seed = 1;
    
    private Tilemap map = null;

    [SerializeField]
    private List<TileData> tileDatas = null;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private int[,] worldMap = null;

    private void Awake()
    {
        SetWorldSeed(seed);
        Random.InitState(GetWorldSeed());
    }

    public void SetWorldMap(int[,] m)
    {
        worldMap = m;
    }

    public void AssignTileData()
    {
        map = FindObjectOfType<Tilemap>();
        dataFromTiles = new Dictionary<TileBase, TileData>();
        
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        CalculateResourceSpawns();
    }

    public TileBase GetCurrectTile(Vector3Int pos) 
    {
        return map.GetTile(map.WorldToCell(pos));
    }

    private void CalculateResourceSpawns()
    {
        GameObject resParent = new GameObject("Resource Parent");
        resParent.transform.position = Vector3.zero;

        for (int i = 0; i < worldMap.GetLength(0); i++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                TileBase tile = GetCurrectTile(new Vector3Int(i, 0, y));

                var data = dataFromTiles[tile];

                if (!data.canSpawnResources)
                    continue;

                bool spawn = Random.Range(0f, 1f) < data.resourceDensity;

                if (!spawn)
                    continue;

                int res = Random.Range(0, data.resources.Length);
                GameObject obj = Instantiate(data.resources[res], new Vector3(i + Random.Range(.2f, .7f), 0, y + Random.Range(.2f, .7f)), Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(30, 0, 0);
                obj.transform.parent = resParent.transform;
            }
        }
    }
    
    static int GetWorldSeed() { return worldSeed; }
    static void SetWorldSeed(int s) { worldSeed = s; }
}
