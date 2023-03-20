using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject grid = null;

    [SerializeField]
    private Tile[] tiles = null;

    public void VisualizeMap(int[,] m)
    {
        GameObject map = Instantiate(grid, Vector3.zero, Quaternion.identity);

        Tilemap tm = map.GetComponentInChildren<Tilemap>();

        for (int x = 0; x < m.GetLength(0); x++)
        {
            for (int y = 0; y < m.GetLength(1); y++)
            {
                tm.SetTile(new Vector3Int(x, y), tiles[m[x, y]]);
            }
        }

        FindObjectOfType<WorldManager>().SetWorldMap(m);
        FindObjectOfType<WorldManager>().AssignTileData();
    }
}
