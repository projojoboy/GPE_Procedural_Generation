using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EnumCollection;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public BiomeType biome;
    
    public bool canSpawnResources;

    public GameObject[] resources;

    [Range(0, 1)]
    public float resourceDensity;
}
