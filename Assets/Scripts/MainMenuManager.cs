using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] GameObject tutorialPanel;

    [Header("Level Container")]
    [SerializeField] RectTransform levelContainer;
    [SerializeField] CanvasGroup levelCanvasGroup;

    [Header("Tutorial Container")]
    [SerializeField] RectTransform tutorialContainer;
    [SerializeField] CanvasGroup tutorialCanvasGroup;

    [Header("Anim Settings")]
    [SerializeField] float showDuration = 0.35f;
    [SerializeField] float hideDuration = 0.25f;
    [SerializeField] Ease showEase = Ease.OutBack;
    [SerializeField] Ease hideEase = Ease.InBack;

    [Header("Scroll Rect")]
    public ScrollRect scrollRect;

    // =================================================
    // ⭐ INIT
    // =================================================
    void Awake()
    {
        InitContainer(levelContainer, levelCanvasGroup);
        InitContainer(tutorialContainer, tutorialCanvasGroup);

        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void InitContainer(RectTransform container, CanvasGroup cg)
    {
        if (container != null)
            container.localScale = Vector3.zero;

        if (cg != null)
            cg.alpha = 0f;
    }

    // =================================================
    // ⭐ PLAY
    // =================================================
    public void OnClickPlay()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        ShowContainer(levelContainer, levelCanvasGroup);

        StartCoroutine(ResetScrollTop());
    }

    IEnumerator ResetScrollTop()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // =================================================
    // ⭐ BACK FROM LEVEL
    // =================================================
    public void OnClickBack()
    {
        HideContainer(levelContainer, levelCanvasGroup, () =>
        {
            levelSelectPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        });
    }

    // =================================================
    // ⭐ TUTORIAL
    // =================================================
    public void OnClickTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        ShowContainer(tutorialContainer, tutorialCanvasGroup);
    }

    public void OnClickBackTutorial()
    {
        HideContainer(tutorialContainer, tutorialCanvasGroup, () =>
        {
            tutorialPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        });
    }

    // =================================================
    // ⭐ GENERIC SHOW
    // =================================================
    void ShowContainer(RectTransform container, CanvasGroup cg)
    {
        if (container == null || cg == null) return;

        container.DOKill();
        cg.DOKill();

        container.localScale = Vector3.zero;
        cg.alpha = 0f;

        container.DOScale(Vector3.one, showDuration)
            .SetEase(showEase);

        cg.DOFade(1f, showDuration);
    }

    // =================================================
    // ⭐ GENERIC HIDE
    // =================================================
    void HideContainer(RectTransform container, CanvasGroup cg, System.Action onComplete = null)
    {
        if (container == null || cg == null)
        {
            onComplete?.Invoke();
            return;
        }

        container.DOKill();
        cg.DOKill();

        container.DOScale(Vector3.zero, hideDuration)
            .SetEase(hideEase);

        cg.DOFade(0f, hideDuration)
            .OnComplete(() => onComplete?.Invoke());
    }
}