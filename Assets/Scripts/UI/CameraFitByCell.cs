using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFitTilted : MonoBehaviour
{
    public PipeGridManager gridManager;

    [Header("Padding theo số ô")]
    public float horizontalCellsPadding = 1f;
    public float verticalCellsPadding = 3.5f;

    const float FIXED_TILT = 70f;

    [SerializeField] Camera cam;

    [Header("Camera Offset")]
    public float forwardOffset = 10f;

    float currentSpacing = 10f; 

    void Awake()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
    }

    void Start()
    {
        FitCamera();
    }
    public void SetSpacing(float spacing)
    {
        currentSpacing = spacing;
    }

    [ContextMenu("Fit Camera")]
    public void FitCamera()
    {
        if (gridManager == null) return;

        float spacing = currentSpacing;

        float boardWidth = gridManager.cols * spacing;
        float boardHeight = gridManager.rows * spacing;

        float totalWidth = boardWidth + horizontalCellsPadding * 2f * spacing;
        float totalHeight = boardHeight + verticalCellsPadding * 2f * spacing;

        Vector3 center = new Vector3(
            (gridManager.cols - 1) * spacing * 0.5f,
            0f,
            (gridManager.rows - 1) * spacing * 0.5f
        );

        transform.rotation = Quaternion.Euler(FIXED_TILT, 0f, 0f);

        Vector3 pos = transform.position;
        pos.x = center.x;
        pos.z = center.z - forwardOffset;
        transform.position = pos;

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = totalWidth / totalHeight;

        float size;

        if (screenRatio >= targetRatio)
            size = totalHeight * 0.5f;
        else
            size = totalHeight * 0.5f * (targetRatio / screenRatio);

        cam.orthographicSize = size;
    }
}