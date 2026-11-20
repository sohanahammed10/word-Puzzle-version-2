using UnityEngine;

// নিজে নিজেই গেম শুরুতে তৈরি হয়, সিন বদলালেও থাকে। কোনো অবজেক্টে লাগাতে হবে না।
// গান: Assets/Resources ফোল্ডারে "bgm" নামে গান রাখো। ক্লিকের শব্দ: "click" নামে।
public class AudioManager : MonoBehaviour
{
    const string KSound = "set_sound";
    const string KMusic = "set_music";
    const string KSfx = "set_sfx";
    const string KMusicVol = "set_music_vol";
    const string KSfxVol = "set_sfx_vol";
    const string KVibration = "set_vibration";
    const string KLanguage = "set_language";

    static AudioManager inst;
    AudioSource music;
    AudioSource sfx;
    AudioClip clickClip;
    bool clickLoaded;

    public static bool SoundOn { get { return PlayerPrefs.GetInt(KSound, 1) == 1; } set { SetInt(KSound, value ? 1 : 0); } }
    public static bool MusicOn { get { return PlayerPrefs.GetInt(KMusic, 1) == 1; } set { SetInt(KMusic, value ? 1 : 0); } }
    public static bool SfxOn { get { return PlayerPrefs.GetInt(KSfx, 1) == 1; } set { SetInt(KSfx, value ? 1 : 0); } }
    public static bool VibrationOn { get { return PlayerPrefs.GetInt(KVibration, 1) == 1; } set { SetInt(KVibration, value ? 1 : 0); } }
    public static int Language { get { return PlayerPrefs.GetInt(KLanguage, 0); } set { SetInt(KLanguage, value); } }
    public static float MusicVolume { get { return PlayerPrefs.GetFloat(KMusicVol, 0.7f); } set { SetFloat(KMusicVol, value); } }
    public static float SfxVolume { get { return PlayerPrefs.GetFloat(KSfxVol, 0.8f); } set { SetFloat(KSfxVol, value); } }

    static float SfxLevel { get { return (SoundOn && SfxOn) ? SfxVolume : 0f; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        if (inst != null) return;
        GameObject go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        go.AddComponent<AudioManager>();
    }

    void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;

        music = gameObject.AddComponent<AudioSource>();
        music.loop = true;
        music.playOnAwake = false;

        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        music.clip = Resources.Load<AudioClip>("bgm");
        if (music.clip == null)
            Debug.LogWarning("AudioManager: Assets/Resources ফোল্ডারে bgm নামে গান নেই");

        Apply();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) PlayerPrefs.Save();
    }

    void Apply()
    {
        bool musicAudible = SoundOn && MusicOn;
        music.volume = MusicVolume;
        music.mute = !musicAudible;
        if (musicAudible && music.clip != null && !music.isPlaying) music.Play();
    }

    static void Refresh()
    {
        if (inst != null) inst.Apply();
    }

    static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
        Refresh();
    }

    static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);   // স্লাইডার টানার সময় বারবার সেভ করি না
        Refresh();
    }

    public static void Flush()
    {
        PlayerPrefs.Save();
    }

    public static void PlayClick()
    {
        if (inst == null || SfxLevel <= 0f) return;
        if (!inst.clickLoaded)
        {
            inst.clickClip = Resources.Load<AudioClip>("click");
            inst.clickLoaded = true;
        }
        if (inst.clickClip != null) inst.sfx.PlayOneShot(inst.clickClip, SfxLevel);
    }

    // ProfileMenu এর "open" আর "close" শব্দের জন্য
    public static void Play(string clipName)
    {
        if (inst == null || SfxLevel <= 0f) return;
        AudioClip c = Resources.Load<AudioClip>(clipName);
        if (c != null) inst.sfx.PlayOneShot(c, SfxLevel);
    }
    public static void PlaySfx(AudioClip clip)
    {
        if (inst == null || clip == null || SfxLevel <= 0f) return;
        inst.sfx.PlayOneShot(clip, SfxLevel);
    }

    public static void Vibrate()
    {
        if (!VibrationOn) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    // Sound Settings এর Default বাটন
    public static void ResetSound()
    {
        PlayerPrefs.SetInt(KMusic, 1);
        PlayerPrefs.SetInt(KSfx, 1);
        PlayerPrefs.SetFloat(KMusicVol, 0.7f);
        PlayerPrefs.SetFloat(KSfxVol, 0.8f);
        PlayerPrefs.Save();
        Refresh();
    }

    // Settings এর Reset Settings বাটন
    public static void ResetAll()
    {
        PlayerPrefs.SetInt(KSound, 1);
        PlayerPrefs.SetInt(KVibration, 1);
        PlayerPrefs.SetInt(KLanguage, 0);
        ResetSound();
    }
}
