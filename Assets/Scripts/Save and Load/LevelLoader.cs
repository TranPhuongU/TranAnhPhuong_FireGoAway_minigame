using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject landStraightPrefab;
    public GameObject landElbowPrefab;
    public GameObject landTShapePrefab;
    public GameObject landCrossPrefab;
    public GameObject landHomePrefab;
    public GameObject landSourcePrefab;
    public GameObject landNonePrefab;

    [Header("Level Database")]
    public LevelDatabase levelDatabase;
    public int currentLevelIndex = 0;

    [SerializeField] private CameraFitTilted cameraFitTilted;

    private Dictionary<PipeType, GameObject> prefabMap;

    void Awake()
    {
        prefabMap = new Dictionary<PipeType, GameObject>()
        {
            { PipeType.Straight, landStraightPrefab },
            { PipeType.Elbow,    landElbowPrefab    },
            { PipeType.T_Shape,  landTShapePrefab   },
            { PipeType.Cross,    landCrossPrefab    },
            { PipeType.Home,     landHomePrefab     },
            { PipeType.Source,   landSourcePrefab   },
            { PipeType.None,     landNonePrefab     },
        };
    }

    void Start()
    {
        currentLevelIndex = LevelSelection.SelectedLevelIndex;
        LoadCurrentLevel();
    }

    public void LoadLevel(LevelScriptale data)
    {
        // ===== Clear old =====
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        PipeGridManager manager = PipeGridManager.instance;
        manager.rows = data.rows;
        manager.cols = data.cols;

        // ⭐ OPTIMIZE lookup
        Dictionary<Vector2Int, TileData> tileLookup = new Dictionary<Vector2Int, TileData>();
        for (int i = 0; i < data.tiles.Count; i++)
        {
            TileData td = data.tiles[i];
            tileLookup[new Vector2Int(td.gridX, td.gridY)] = td;
        }

        List<PipeTile> spawnedTiles = new List<PipeTile>();

        for (int y = 0; y < data.rows; y++)
        {
            for (int x = 0; x < data.cols; x++)
            {
                TileData tileData;
                tileLookup.TryGetValue(new Vector2Int(x, y), out tileData);

                PipeType pipeType = tileData != null ? tileData.pipeType : PipeType.None;
                int rotationDeg = tileData != null ? tileData.initialRotation : 0;

                GameObject prefab;
                if (!prefabMap.TryGetValue(pipeType, out prefab))
                    continue;

                Vector3 pos = new Vector3(x * data.spacing, 0f, y * data.spacing);

                GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);

                PipeTile tile = go.GetComponent<PipeTile>();
                if (tile == null) continue;

                tile.pipeType = pipeType;
                tile.gridX = x;
                tile.gridY = y;
                tile.SetRotation(rotationDeg / 90);

                spawnedTiles.Add(tile);
            }
        }

        manager.allTiles = spawnedTiles.ToArray();
        manager.sourceTile = spawnedTiles.Find(t => t.pipeType == PipeType.Source);
        manager.homeTiles = spawnedTiles.FindAll(t => t.pipeType == PipeType.Home).ToArray();

        manager.BuildGrid();
        manager.CheckConnection(true);

        if (cameraFitTilted != null)
        {
            cameraFitTilted.SetSpacing(data.spacing);
            cameraFitTilted.FitCamera();
        }
    }

    public void LoadCurrentLevel()
    {
        if (levelDatabase == null || levelDatabase.levels.Count == 0)
        {
            Debug.LogError("LevelDatabase is empty!");
            return;
        }

        currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, levelDatabase.levels.Count - 1);

        LevelScriptale data = levelDatabase.levels[currentLevelIndex];

        GameManager.instance.SetStartMoves(data.rotation);
        LoadLevel(data);
    }
}