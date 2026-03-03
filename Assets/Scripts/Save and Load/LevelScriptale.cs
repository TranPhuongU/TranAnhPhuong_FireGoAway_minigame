// LevelData.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileData
{
    public int gridX, gridY;
    public PipeType pipeType;
    public int initialRotation;
}

[CreateAssetMenu(fileName = "Level_1", menuName = "PipeGame/Level Data")]
public class LevelScriptale : ScriptableObject
{
    [Header("Grid Size")]
    public int rows = 3;
    public int cols = 3;

    [Header("Tile Spacing")]
    public float spacing = 10f; 

    [Header("Tiles")]
    public List<TileData> tiles = new();

    public int rotation = 25;
}