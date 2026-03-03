using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "PipeGame/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelScriptale> levels = new();
}