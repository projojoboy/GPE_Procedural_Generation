using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public int seed { get; private set; }
    public int[,] worldMap { get; private set; }

    public SaveFile(int seed, int[,] worldMap)
    {
        this.seed = seed;
        this.worldMap = worldMap;
    }
}
