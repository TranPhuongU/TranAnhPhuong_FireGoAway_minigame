using UnityEngine;
using UnityEngine.UI;

public class SettingsSoundUI : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private Image musicIcon;
    [SerializeField] private Image sfxIcon;

    [Header("Sprites")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private void Start()
    {
        RefreshUI();
    }
    public void ToggleMusic()
    {
        bool newValue = !SoundManager.Instance.IsMusicOn();
        SoundManager.Instance.SetMusic(newValue);
        RefreshUI();
    }
    public void ToggleSFX()
    {
        bool newValue = !SoundManager.Instance.IsSFXOn();
        SoundManager.Instance.SetSFX(newValue);
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (musicIcon != null)
        {
            musicIcon.sprite = SoundManager.Instance.IsMusicOn()
                ? onSprite
                : offSprite;
        }

        if (sfxIcon != null)
        {
            sfxIcon.sprite = SoundManager.Instance.IsSFXOn()
                ? onSprite
                : offSprite;
        }
    }
}