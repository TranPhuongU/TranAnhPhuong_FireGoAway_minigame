using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private const string MUSIC_KEY = "music_on";
    private const string SFX_KEY = "sfx_on";

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>();

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip pipeClickClip;
    [SerializeField] private AudioClip pipeConnectedClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip buttonClickClip;


    float lastPipeSoundTime;
    const float PIPE_SOUND_COOLDOWN = 0.05f;

    private bool musicOn = true;
    private bool sfxOn = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadSettings();
            DisablePlayOnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DisablePlayOnAwake()
    {
        if (musicSource != null) musicSource.playOnAwake = false;
        if (sfxSource != null) sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayRandomMusic();
    }
    public void PlayRandomMusic()
    {
        if (!musicOn) return;
        if (musicClips.Count == 0) return;
        if (musicSource == null) return;

        int index = Random.Range(0, musicClips.Count);
        musicSource.clip = musicClips[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusic(bool value)
    {
        musicOn = value;
        PlayerPrefs.SetInt(MUSIC_KEY, musicOn ? 1 : 0);

        if (musicOn) PlayRandomMusic();
        else if (musicSource != null) musicSource.Stop();
    }
    public bool IsMusicOn()
    {
        return musicOn;
    }
    public void SetSFX(bool value)
    {
        sfxOn = value;
        PlayerPrefs.SetInt(SFX_KEY, sfxOn ? 1 : 0);
    }

    public bool IsSFXOn()
    {
        return sfxOn;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (!sfxOn) return;
        if (clip == null) return;
        if (sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayPipeClick()
    {
        PlaySFX(pipeClickClip);
    }
    public void PlayButton()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayPipeConnected()
    {
        if (Time.time - lastPipeSoundTime < PIPE_SOUND_COOLDOWN)
            return;

        lastPipeSoundTime = Time.time;
        PlaySFX(pipeConnectedClip);
    }

    public void PlayWin()
    {
        PlaySFX(winClip);
    }

    public void PlayLose()
    {
        PlaySFX(loseClip);
    }

    private void LoadSettings()
    {
        musicOn = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
        sfxOn = PlayerPrefs.GetInt(SFX_KEY, 1) == 1;
    }
}