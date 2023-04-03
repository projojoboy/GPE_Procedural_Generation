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

    [Header("Biome %")]
    [SerializeField]
    [Range(0, 100)]
    private Vector2Int forestPercentRange = new(0, 70);
    [SerializeField]
    [Range(0, 100)]
    private Vector2Int desertPercentRange = new(70, 85);
    [SerializeField]
    [Range(0, 100)]
    private Vector2Int mountainPercentRange = new(85, 100);

    private int[,] map;

    private MapVisualizer mv = null;
    private WorldManager wm = null;

    private void Awake()
    {
        mv = GetComponent<MapVisualizer>();
        wm = FindObjectOfType<WorldManager>();
        
        map = GenerateWorld();

        for (int i = 0; i < resampleAmount; i++) map = ResampleWorld(map, map.GetLength(0) * 2 - 1);
        map = CreateChunks(map);
        for (int i = 0; i < smoothenAmount; i++) map = SmoothenWorld(map);

        wm.SetChunkSize(chunkSize);
        mv.VisualizeMap(map);
    }

    private int[,] GenerateWorld()
    {
        map = new int[5, 5];

        int lengthX = map.GetLength(0);
        int lengthY = map.GetLength(1);

        int[] t = new int[4];

        // Populate map
        for (int x = 0; x < lengthX; x++)
        {
            for (int y = 0; y < lengthY; y++)
            {
                // If side, make water biome. Else, make random biome
                if ((x == 0 || x == lengthX - 1) || (y == 0 || y == lengthY - 1))
                    map[x, y] = 0;
                else
                {
                    map[x, y] = GetRandomBiome();
                    t[map[x, y]]++;
                }
            }
        }

        for (int i = 0; i < t.Length; i++)
        {
            if (t[i] == 0)
            {
                int replace = GetHighestValuePosition(t);

                Vector2Int pos = GetRandomTile(lengthX, lengthY, replace);
                map[pos.x, pos.y] = i;
            }
        }

        return map;
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

    private bool IsBetween(int x, int y, int point) { return point >= x && point < y; }

    private int GetHighestValuePosition(int[] arr)
    {
        int loc = 0;
        int max = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > max)
            {
                max = arr[i];
                loc = i;
            }
        }

        return loc;
    }

    private Vector2Int GetRandomTile(int lengthX, int lengthY, int overwritableBiome)
    {
        int i = 99;
        Vector2Int pos = new(1, 1);

        while (i != overwritableBiome)
        {
            pos = new(Random.Range(1, lengthX - 1), Random.Range(1, lengthY - 1));
            i = map[pos.x, pos.y];
        }

        return pos;
    }

    private int GetRandomBiome()
    {
        int biome = Random.Range(0, 100);

        if (IsBetween(forestPercentRange.x, forestPercentRange.y, biome))
            return 2;
        else if (IsBetween(desertPercentRange.x, desertPercentRange.y, biome))
            return 1;
        else if (IsBetween(mountainPercentRange.x, mountainPercentRange.y, biome))
            return 3;

        return 0;
    }
}
