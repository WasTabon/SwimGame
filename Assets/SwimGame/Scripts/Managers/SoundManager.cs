using System.Collections.Generic;
using UnityEngine;

public enum SfxType { Tap, Step, Win, Lose, Coin }

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const int PoolSize = 6;
    private const int SampleRate = 44100;

    private readonly List<AudioSource> sfxPool = new List<AudioSource>();
    private readonly Dictionary<SfxType, AudioClip> clips = new Dictionary<SfxType, AudioClip>();
    private AudioSource musicSource;
    private AudioClip musicClip;
    private int poolIndex;

    private float sfxVolume;
    private float musicVolume;
    private bool sfxMuted;
    private bool musicMuted;

    public static void Ensure()
    {
        if (Instance != null) return;
        new GameObject("SoundManager").AddComponent<SoundManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
        CreateSources();
        GenerateClips();
        PlayMusic();
    }

    private void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 1f);
        musicVolume = PlayerPrefs.GetFloat("music_volume", 1f);
        sfxMuted = PlayerPrefs.GetInt("sfx_muted", 0) == 1;
        musicMuted = PlayerPrefs.GetInt("music_muted", 0) == 1;
    }

    private void CreateSources()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void GenerateClips()
    {
        clips[SfxType.Tap] = CreateClip("sfx_tap", 0.07f, t => Tone(t, 880f, 0.07f, 0.45f));
        clips[SfxType.Step] = CreateClip("sfx_step", 0.1f, t => Tone(t, 320f, 0.1f, 0.35f));
        clips[SfxType.Coin] = CreateClip("sfx_coin", 0.22f, t =>
        {
            if (t < 0.1f) return Tone(t, 1320f, 0.1f, 0.4f);
            return Tone(t - 0.1f, 1760f, 0.12f, 0.4f);
        });
        clips[SfxType.Win] = CreateClip("sfx_win", 0.75f, t =>
        {
            if (t < 0.2f) return Tone(t, 523f, 0.25f, 0.45f);
            if (t < 0.4f) return Tone(t - 0.2f, 659f, 0.25f, 0.45f);
            return Tone(t - 0.4f, 784f, 0.35f, 0.45f);
        });
        clips[SfxType.Lose] = CreateClip("sfx_lose", 0.55f, t =>
        {
            if (t < 0.25f) return Tone(t, 220f, 0.3f, 0.5f);
            return Tone(t - 0.25f, 165f, 0.3f, 0.5f);
        });
        musicClip = CreateClip("music_loop", 6f, t =>
        {
            float lfo = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * t / 6f);
            float v = Mathf.Sin(2f * Mathf.PI * 110f * t)
                      + Mathf.Sin(2f * Mathf.PI * 165f * t) * 0.7f
                      + Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.5f;
            return v * 0.06f * lfo;
        });
    }

    private AudioClip CreateClip(string clipName, float duration, System.Func<float, float> generator)
    {
        int samples = Mathf.CeilToInt(duration * SampleRate);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            data[i] = generator((float)i / SampleRate);
        }
        var clip = AudioClip.Create(clipName, samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private float Tone(float t, float frequency, float duration, float volume)
    {
        float env = Mathf.Clamp01(1f - t / duration);
        env *= env;
        return Mathf.Sin(2f * Mathf.PI * frequency * t) * volume * env;
    }

    public void PlaySfx(SfxType type)
    {
        if (sfxMuted) return;
        var source = sfxPool[poolIndex];
        poolIndex = (poolIndex + 1) % PoolSize;
        source.PlayOneShot(clips[type], sfxVolume);
    }

    public void PlayMusic()
    {
        musicSource.clip = musicClip;
        musicSource.volume = musicMuted ? 0f : musicVolume;
        if (!musicSource.isPlaying) musicSource.Play();
    }

    public bool SfxMuted
    {
        get => sfxMuted;
        set
        {
            sfxMuted = value;
            PlayerPrefs.SetInt("sfx_muted", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool MusicMuted
    {
        get => musicMuted;
        set
        {
            musicMuted = value;
            musicSource.volume = value ? 0f : musicVolume;
            PlayerPrefs.SetInt("music_muted", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("sfx_volume", sfxVolume);
            PlayerPrefs.Save();
        }
    }

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            if (!musicMuted) musicSource.volume = musicVolume;
            PlayerPrefs.SetFloat("music_volume", musicVolume);
            PlayerPrefs.Save();
        }
    }
}
