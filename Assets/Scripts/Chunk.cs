using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EnumCollection;

public class Chunk
{
    public TileBase[] Tiles { get; private set; }
    public BiomeType CurrentBiome { get; private set; }
    public GameObject[] Resources { get; private set; }
    public Chunk[] Neighbors { get; private set; }

    private bool render = false;

    public void SetBiome(BiomeType biome)
    {
        CurrentBiome = biome;
    }

    public void AddTile(TileBase tile)
    {
        Tiles ??= new TileBase[0];

        TileBase[] temp = Tiles;
        Tiles = new TileBase[Tiles.Length + 1];

        for (int i = 0; i < temp.Length; i++)
            Tiles[i] = temp[i];

        Tiles[^1] = tile;
    }

    public void AddResource(GameObject resource)
    {
        this.Resources ??= new GameObject[0];

        GameObject[] temp = Resources;
        Resources = new GameObject[Resources.Length + 1];

        for (int i = 0; i < temp.Length; i++)
            Resources[i] = temp[i];

        Resources[^1] = resource;

        if (render) resource.SetActive(true);
    }

    public void RenderResources(bool rend)
    {
        if (rend == render || this.Resources == null)
            return;

        foreach (GameObject obj in this.Resources)
            obj.SetActive(rend);

        render = rend;
    }
}
