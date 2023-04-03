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
    private int chunkSize = 0;
    private int[,] worldMap = null;

    private Tilemap map = null;

    [SerializeField]
    private List<TileData> tileDatas = null;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private Chunk[,] chunks = new Chunk[0, 0];

    private ObjectRenderer objR = null;

    private void Awake()
    {
        SetWorldSeed(seed);
        Random.InitState(GetWorldSeed());

        objR = GetComponent<ObjectRenderer>();
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

        InitializeChunks();
        CalculateResourceSpawns();
    }

    public TileBase GetCurrectTile(Vector3Int pos)
    {
        return map.GetTile(map.WorldToCell(pos));
    }

    private void CalculateResourceSpawns()
    {
        GameObject resParent = new("Resource Parent");
        resParent.transform.position = Vector3.zero;

        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                TileBase tile = GetCurrectTile(new Vector3Int(x, 0, y));

                var data = dataFromTiles[tile];

                if (!data.canSpawnResources)
                    continue;

                bool spawn = Random.Range(0f, 1f) < data.resourceDensity;

                if (!spawn)
                    continue;

                int res = Random.Range(0, data.resources.Length);
                GameObject obj = Instantiate(data.resources[res], new Vector3(x + Random.Range(.2f, .7f), 0, y + Random.Range(.2f, .7f)), Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(30, 0, 0);
                obj.transform.parent = resParent.transform;
                obj.SetActive(false);
                chunks[x / chunkSize, y / chunkSize].AddResource(obj);
            }
        }
    }

    private void InitializeChunks()
    {
        chunks = new Chunk[worldMap.GetLength(0) / chunkSize, worldMap.GetLength(1) / chunkSize];

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
                chunks[x, y] = new Chunk();
        }

        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(0); y++)
            {
                Vector2Int pos = new(x / chunkSize, y / chunkSize);
                Chunk c = chunks[pos.x, pos.y];
                TileBase t = GetCurrectTile(new Vector3Int(x, 0, y));
                c.AddTile(t);
                c.SetBiome(dataFromTiles[t].biome);
            }
        }
    }

    public Chunk[,] GetChunks() { return chunks; }
    public void SetChunkSize(int size) { chunkSize = size; }
    public int GetChunkSize() { return chunkSize; }
    static int GetWorldSeed() { return worldSeed; }
    public static void SetWorldSeed(int s) { worldSeed = s; }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                if (chunks[x, y].CurrentBiome == BiomeType.Water)
                    Gizmos.color = Color.blue;
                else if (chunks[x, y].CurrentBiome == BiomeType.Forest)
                    Gizmos.color = Color.green;
                else if (chunks[x, y].CurrentBiome == BiomeType.Mountain)
                    Gizmos.color = Color.gray;
                else if (chunks[x, y].CurrentBiome == BiomeType.Desert)
                    Gizmos.color = Color.yellow;

                if (chunks[x, y] == objR.GetCurrentChunk())
                    Gizmos.color = Color.red;

                Gizmos.DrawWireCube(new Vector3((chunkSize / 2) + (chunkSize * x), 0, (chunkSize / 2) + (chunkSize * y)), new Vector3(chunkSize, 1, chunkSize));
            }
        }
    }
}
