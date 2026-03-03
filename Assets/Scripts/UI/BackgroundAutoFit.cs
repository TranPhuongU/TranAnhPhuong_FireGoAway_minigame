using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundAutoFit : MonoBehaviour
{
    [Header("Scale multiplier")]
    [Range(1f, 1.5f)]
    public float extraScale = 1.08f;

    [Header("Z offset from camera")]
    public float zOffset = 10f;

    Camera cam;
    SpriteRenderer sr;

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!sr) sr = GetComponent<SpriteRenderer>();

        if (cam == null || !cam.orthographic || sr.sprite == null) return;

        FitToScreen();
        FollowCameraCenter();
    }

    void FitToScreen()
    {
        float worldScreenHeight = cam.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * cam.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;

        float scale = Mathf.Max(
            worldScreenWidth / spriteSize.x,
            worldScreenHeight / spriteSize.y
        );

        scale *= extraScale;

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    void FollowCameraCenter()
    {
        Vector3 camPos = cam.transform.position;

        transform.position = new Vector3(
            camPos.x,
            -2,
            camPos.z + zOffset
        );
    }
}