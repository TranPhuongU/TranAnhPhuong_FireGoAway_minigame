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

        GUILayout.Label("Brush", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        foreach (PipeType pt in System.Enum.GetValues(typeof(PipeType)))
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = (selectedPipeType == pt) ? typeColors[pt] : Color.gray;

            if (GUILayout.Button(GetShape(pt), GUILayout.Width(40), GUILayout.Height(36)))
                selectedPipeType = pt;

            GUI.backgroundColor = oldBg;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation:", GUILayout.Width(60));
        int[] rotations = { 0, 90, 180, 270 };
        foreach (int rot in rotations)
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = (selectedRotation == rot) ? Color.yellow : Color.gray;

            if (GUILayout.Button(rot + "°", GUILayout.Width(50)))
                selectedRotation = rot;

            GUI.backgroundColor = oldBg;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        GUILayout.Label("Preview", EditorStyles.boldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(60, 60);
        GUI.Box(previewRect, GUIContent.none);

        Matrix4x4 oldMatrixPreview = GUI.matrix;
        GUIUtility.RotateAroundPivot(selectedRotation, previewRect.center);

        GUIStyle previewStyle = new GUIStyle(EditorStyles.boldLabel);
        previewStyle.alignment = TextAnchor.MiddleCenter;
        previewStyle.fontSize = 28;

        GUI.Label(previewRect, GetShape(selectedPipeType), previewStyle);
        GUI.matrix = oldMatrixPreview;

        EditorGUILayout.Space(10);
        GUILayout.Label("Grid - Click o de ve", EditorStyles.boldLabel);

        float cellSize = 52f;

        for (int row = levelData.rows - 1; row >= 0; row--)
        {
            GUILayout.BeginHorizontal();

            for (int col = 0; col < levelData.cols; col++)
            {
                TileData existing = levelData.tiles.Find(t => t.gridX == col && t.gridY == row);
                PipeType currentType = existing != null ? existing.pipeType : PipeType.None;

                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize);

                Color oldBg = GUI.backgroundColor;
                GUI.backgroundColor = typeColors[currentType];
                GUI.Box(rect, GUIContent.none);
                GUI.backgroundColor = oldBg;

                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 22;

                if (currentType == PipeType.None)
                {
                    style.normal.textColor = new Color(1f, 1f, 1f, 0.4f);
                    GUI.Label(rect, "None", style);
                }
                else
                {
                    Matrix4x4 oldMatrix = GUI.matrix;

                    int rotation = existing != null ? existing.initialRotation : 0;
                    GUIUtility.RotateAroundPivot(rotation, rect.center);

                    style.normal.textColor = Color.white;
                    GUI.Label(rect, GetShape(currentType), style);

                    GUI.matrix = oldMatrix;
                }

                if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                {
                    PaintCell(levelData, col, row);
                }
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

    private string GetShape(PipeType type)
    {
        return type switch
        {
            PipeType.None => "None",
            PipeType.Straight => "│",
            PipeType.Elbow => "┘",
            PipeType.T_Shape => "┬",
            PipeType.Cross => "┼",
            PipeType.Home => "Home",
            PipeType.Source => "Src",
            _ => ""
        };
    }
}