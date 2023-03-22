using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MapVisualizer))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private int resampleAmount = 5;
    [SerializeField]
    private int smoothenAmount = 3;
    [SerializeField]
    private int chunkSize = 16;

    private Dictionary<Vector2, int[,]> chunkData;

    private int[,] map;

    private MapVisualizer mv = null;
    private WorldManager wm = null;

    private void Awake()
    {
        mv = GetComponent<MapVisualizer>();
        wm = FindObjectOfType<WorldManager>();
    }

    private void Start()
    {
        //map = new int[5, 5] {
        //    { 0, 0, 0, 0, 0 },
        //    { 0, 2, 1, 1, 0 },
        //    { 0, 2, 2, 2, 0 },
        //    { 0, 2, 3, 3, 0 },
        //    { 0, 0, 0, 0, 0 }
        //};

        map = new int[5, 5] {
            { 0, 0, 0, 0, 0 },
            { 0, Random.Range(1,4), Random.Range(1,4), Random.Range(1,4), 0 },
            { 0, Random.Range(1,4), Random.Range(1,4), Random.Range(1,4), 0 },
            { 0, Random.Range(1,4), Random.Range(1,4), Random.Range(1,4), 0 },
            { 0, 0, 0, 0, 0 }
        };

        for (int i = 0; i < resampleAmount; i++) map = ResampleWorld(map, map.GetLength(0) * 2 - 1);
        map = CreateChunks(map);
        for (int i = 0; i < smoothenAmount; i++) map = SmoothenWorld(map);

        wm.SetChunkData(chunkData, chunkSize);
        mv.VisualizeMap(map);
    }

    int[,] SmoothenWorld(int[,] oldWorld)
    {
        for (int x = 0; x < oldWorld.GetLength(0); x++)
        {
            for (int y = 0; y < oldWorld.GetLength(1); y++)
            {
                // [ ][a][ ]
                // [b][t][c]
                // [ ][d][ ]
                int t = oldWorld[x, y];
                int a = y == oldWorld.GetLength(1) - 1 ? 0 : oldWorld[x, y + 1];
                int b = x == 0 ? 0 : oldWorld[x - 1, y];
                int c = x == oldWorld.GetLength(0) - 1 ? 0 : oldWorld[x + 1, y];
                int d = y == 0 ? 0 : oldWorld[x, y - 1];

                if (a == d && b == c) t = Random.Range(0, 2) == 0 ? a : b;
                else if (a == d) t = a;
                else if (a == d) t = b;

                oldWorld[x, y] = t;
            }
        }

        return oldWorld;
    }

    int[,] CreateChunks(int[,] oldWorld)
    {
        chunkData = new Dictionary<Vector2, int[,]>();

        int oldWidth = oldWorld.GetLength(0);
        int oldHeight = oldWorld.GetLength(1);
        int newWidth = oldWidth * chunkSize;
        int newHeight = oldHeight * chunkSize;
        int[,] newWorld = new int[newWidth, newHeight];

        for (int x = 0; x < oldWidth; x++)
        {
            for (int y = 0; y < oldHeight; y++)
            {
                int newX = x * chunkSize;
                int newY = y * chunkSize;
                int tile = oldWorld[x, y];

                for (int cx = 0; cx < chunkSize; cx++)
                {
                    for (int cy = 0; cy < chunkSize; cy++)
                    {
                        int newTileX = newX + cx;
                        int newTileY = newY + cy;
                        newWorld[newTileX, newTileY] = tile;

                        if (tile != 0)
                        {
                            Vector2 chunkPos = new Vector2(x, y);
                            if (!chunkData.ContainsKey(chunkPos))
                                chunkData.Add(chunkPos, new int[chunkSize, chunkSize]);

                            chunkData[chunkPos][cx, cy] = tile;
                        }
                    }
                }
            }
        }

        return newWorld;
    }

    int[,] ResampleWorld(int[,] oldWorld, int sampleSize)
    {
        int[,] newWorld = new int[sampleSize, sampleSize];

        int xSize = oldWorld.GetLength(0);
        int ySize = oldWorld.GetLength(1);

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                newWorld[x * 2, y * 2] = oldWorld[x, y];

                // [x y] [ . ]
                // [ . ] [ ? ]
                if (x < xSize - 1 && y < ySize - 1)
                {
                    newWorld[x * 2 + 1, y * 2 + 1] =
                        CoinFlip(
                            oldWorld[x, y],
                            oldWorld[x, y + 1],
                            oldWorld[x + 1, y],
                            oldWorld[x + 1, y + 1]
                            );
                }

                // [x y] [ ? ]
                // [ . ] [ . ]
                if (x < xSize - 1)
                {
                    newWorld[x * 2 + 1, y * 2] = CoinFlip(oldWorld[x, y], oldWorld[x + 1, y]);
                }

                // [x y] [ . ]
                // [ ? ] [ . ]
                if (y < ySize - 1)
                {
                    newWorld[x * 2, y * 2 + 1] = CoinFlip(oldWorld[x, y], oldWorld[x, y + 1]);
                }
            }
        }

        return newWorld;
    }


    int CoinFlip(int a, int b)
    {
        if (Random.value < 0.5f)
        {
            return a;
        }
        else
        {
            return b;
        }
    }


    int CoinFlip(int a, int b, int c, int d)
    {
        float val = Random.value;

        if (val < 0.25f && val >= 0.50f)
        {
            return a;
        }
        else if (val < 0.50f && val >= 0.75f)
        {
            return b;
        }
        else if (val < 0.75f)
        {
            return c;
        }
        else
        {
            return d;
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(new Vector3((chunkSize / 2) + (chunkSize * x), 0, (chunkSize / 2) + (chunkSize * y)), new Vector3(chunkSize, 1, chunkSize));
            }
        }
    }
}
