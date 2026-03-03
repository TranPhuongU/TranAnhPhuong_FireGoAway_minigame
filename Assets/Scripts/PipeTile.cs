using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PipeType { None, Straight, Elbow, T_Shape, Cross, Home, Source }

public class PipeTile : MonoBehaviour
{
    [Header("Info")]
    public PipeType pipeType;
    public int gridX;
    public int gridY;

    [Header("References")]
    public Transform pipeModel;

    [Header("Materials")]
    public Material normalMaterial;
    public Material connectedMaterial;

    [Tooltip("Material của thân ống cần bị thay")]
    public Material pipeBodyMaterial;

    [SerializeField] Renderer cachedRenderer;

    static readonly Dictionary<PipeType, int[]> BasePorts = new Dictionary<PipeType, int[]>
    {
        { PipeType.Straight, new int[] { 0, 2 } },
        { PipeType.Elbow,    new int[] { 0, 3 } },
        { PipeType.T_Shape,  new int[] { 2, 1, 3 } },
        { PipeType.Cross,    new int[] { 0, 1, 2, 3 } },
        { PipeType.Home,     new int[] { 3 } },
        { PipeType.Source,   new int[] { 1 } },
        { PipeType.None,     new int[] {} }
    };

    int rotationIndex;
    bool isRotating;
    bool isConnectedCached;
    int bodyMatIndex = -1;

    Material[] runtimeMats;
    Tween clickTween;
    Vector3 rootStartLocalPos;

    void Awake()
    {
        if (cachedRenderer == null)
            cachedRenderer = GetComponentInChildren<Renderer>();

        CacheBodyMaterialIndex();
        CacheRuntimeMaterials();
    }

    void Start()
    {
        ApplyRotationImmediate();
        SetConnected(pipeType == PipeType.Source, false);
        rootStartLocalPos = transform.localPosition;
    }

    void CacheRuntimeMaterials()
    {
        if (cachedRenderer == null) return;
        runtimeMats = cachedRenderer.materials;
    }

    void CacheBodyMaterialIndex()
    {
        if (cachedRenderer == null || pipeBodyMaterial == null) return;

        Material[] mats = cachedRenderer.sharedMaterials;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] == pipeBodyMaterial)
            {
                bodyMatIndex = i;
                break;
            }
        }
    }

    void ApplyRotationImmediate()
    {
        if (pipeModel == null) return;
        pipeModel.localRotation = Quaternion.Euler(0f, rotationIndex * 90f, 0f);
    }

    public HashSet<int> GetOpenPorts()
    {
        HashSet<int> result = new HashSet<int>();

        int[] ports = BasePorts[pipeType];
        for (int i = 0; i < ports.Length; i++)
        {
            result.Add((ports[i] + rotationIndex) % 4);
        }

        return result;
    }

    public void OnTapped()
    {
        if (isRotating) return;

        PlayClickBounce();

        if (pipeType == PipeType.None)
            return;

        isRotating = true;
        rotationIndex = (rotationIndex + 1) % 4;

        pipeModel.DOKill();
        pipeModel
            .DOLocalRotate(new Vector3(0f, rotationIndex * 90f, 0f), 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(OnRotateComplete);
    }

    void OnRotateComplete()
    {
        isRotating = false;
        GameManager.instance.UseMove();
        PipeGridManager.instance.CheckConnection();
    }

    public void SetConnected(bool connected, bool playSound = true)
    {
        if (cachedRenderer == null || bodyMatIndex == -1) return;
        if (isConnectedCached == connected) return;

        bool wasConnected = isConnectedCached;
        isConnectedCached = connected;

        // ⭐ DEBUG CHÍNH
        if (!wasConnected && connected && playSound)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPipeConnected();
        }

        runtimeMats[bodyMatIndex] = connected ? connectedMaterial : normalMaterial;
        cachedRenderer.materials = runtimeMats;
    }

    public void SetRotation(int index)
    {
        rotationIndex = ((index % 4) + 4) % 4;
        ApplyRotationImmediate();
    }

    void PlayClickBounce()
    {
        if (clickTween != null)
            clickTween.Kill();

        transform.localPosition = rootStartLocalPos;

        clickTween = transform
            .DOLocalMoveY(rootStartLocalPos.y - 1.5f, 0.1f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);
    }
}