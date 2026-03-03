using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelScriptale))]
public class LevelDataEditor : Editor
{
    private PipeType selectedPipeType = PipeType.Straight;
    private int selectedRotation = 0;

    private static readonly Dictionary<PipeType, Color> typeColors = new()
    {
        { PipeType.None,     new Color(0.3f, 0.3f, 0.3f) },
        { PipeType.Straight, new Color(0.2f, 0.6f, 1f)   },
        { PipeType.Elbow,    new Color(0.2f, 0.8f, 0.4f) },
        { PipeType.T_Shape,  new Color(1f, 0.8f, 0.2f)   },
        { PipeType.Cross,    new Color(1f, 0.4f, 0.4f)   },
        { PipeType.Home,     new Color(1f, 0.6f, 0.2f)   },
        { PipeType.Source,   new Color(0.6f, 0.2f, 1f)   },
    };

    private static readonly Dictionary<PipeType, string> typeLabels = new()
    {
        { PipeType.None,     "None"  },
        { PipeType.Straight, "Str"   },
        { PipeType.Elbow,    "Elb"   },
        { PipeType.T_Shape,  "T"     },
        { PipeType.Cross,    "Cross" },
        { PipeType.Home,     "Home"  },
        { PipeType.Source,   "Src"   },
    };

    public override void OnInspectorGUI()
    {
        var levelData = (LevelScriptale)target;

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Grid Size", EditorStyles.boldLabel);
        levelData.rows = EditorGUILayout.IntSlider("Rows", levelData.rows, 1, 10);
        levelData.cols = EditorGUILayout.IntSlider("Cols", levelData.cols, 1, 10);
        levelData.spacing = EditorGUILayout.FloatField("Spacing", levelData.spacing);
        levelData.rotation = EditorGUILayout.IntField("Moves", levelData.rotation);
        
        EditorGUILayout.Space(10);

        // ===== Brush =====
        GUILayout.Label("Brush", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        foreach (PipeType pt in System.Enum.GetValues(typeof(PipeType)))
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = (selectedPipeType == pt) ? typeColors[pt] : Color.gray;

            if (GUILayout.Button(typeLabels[pt], GUILayout.Width(40), GUILayout.Height(36)))
                selectedPipeType = pt;

            GUI.backgroundColor = oldBg;
        }
        GUILayout.EndHorizontal();

        // ===== Rotation =====
        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation:", GUILayout.Width(60));
        int[] rotations = { 0, 90, 180, 270 };
        foreach (int rot in rotations)
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = (selectedRotation == rot) ? Color.yellow : Color.gray;

            if (GUILayout.Button(rot + "deg", GUILayout.Width(50)))
                selectedRotation = rot;

            GUI.backgroundColor = oldBg;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        GUILayout.Label("Grid - Click o de ve", EditorStyles.boldLabel);

        float cellSize = 52f;

        // 🔥 FIX TRỤC Ở ĐÂY
        for (int row = levelData.rows - 1; row >= 0; row--)
        {
            GUILayout.BeginHorizontal();

            for (int col = 0; col < levelData.cols; col++)
            {
                // ✅ gridX = col, gridY = row
                TileData existing = levelData.tiles.Find(t => t.gridX == col && t.gridY == row);
                PipeType currentType = existing != null ? existing.pipeType : PipeType.None;

                Color oldBg = GUI.backgroundColor;
                GUI.backgroundColor = typeColors[currentType];

                string label = typeLabels[currentType];
                if (existing != null && existing.initialRotation != 0)
                    label += "\n" + existing.initialRotation;

                if (GUILayout.Button(label, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    PaintCell(levelData, col, row); // ⭐ cực kỳ quan trọng
                }

                GUI.backgroundColor = oldBg;
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Clear All", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear", "Xoa het tat ca tile?", "OK", "Cancel"))
                levelData.tiles.Clear();
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(levelData);
    }

    // 🔥 FIX TRỤC
    private void PaintCell(LevelScriptale levelData, int x, int y)
    {
        TileData existing = levelData.tiles.Find(t => t.gridX == x && t.gridY == y);

        if (existing != null)
        {
            existing.pipeType = selectedPipeType;
            existing.initialRotation = selectedRotation;
        }
        else
        {
            levelData.tiles.Add(new TileData
            {
                gridX = x,
                gridY = y,
                pipeType = selectedPipeType,
                initialRotation = selectedRotation,
            });
        }

        EditorUtility.SetDirty(target);
    }
}