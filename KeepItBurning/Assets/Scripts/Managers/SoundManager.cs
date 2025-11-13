using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundAction
{
    Click,
    StartGame,
    back,
    WinGame,
    GameOver,
    PickItem,
    DropItem,
    PauseMenu,
    Clock,
    NewVisitor,
    HighScore,
    MainCharacterWalkingPath,
    MainCharacterWalikForest,
    VisitorWalking,
    ChopWood,
    CampfireNormal,
    CampfireLow,
    CampFireHigh,
    DropWoodOnFire,
    VisitorHappy
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SoundList")]
    [SerializeField] private List<SoundDefinition> sounds = new();

    [Header("Master Setings")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private bool dontDestroyOnLoad = true;

    private readonly Dictionary<SoundAction, SoundDefinition> _defs = new();
    private readonly Queue<AudioSource> _pool = new();
    private readonly HashSet<AudioSource> _inUse = new();
    private readonly Dictionary<SoundAction, AudioSource> _looping = new();

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        BuildDefinitionsDictionary();
        WarmPool();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    #endregion
    #region Public static API (one-liners)
    public static AudioSource Play(SoundAction action) =>
        Instance != null ? Instance.PlayInternal(action, is3D: false, pos: null) : null;

    public static AudioSource PlayAtPosition(SoundAction action, Vector3 position) =>
        Instance != null ? Instance.PlayInternal(action, is3D: true, pos: position) : null;

    public static AudioSource PlayLoop(SoundAction action) =>

        Instance != null ? Instance.PlayLoopInternal(action) : null;

    public static void StopLoop(SoundAction action, float fadeOutSeconds = 0f)
    {
        if (Instance != null) Instance.StopLoopInternal(action, fadeOutSeconds);
    }

    public static void SetMasterVolume(float value)
    {
        if (Instance != null) Instance.masterVolume = Mathf.Clamp01(value);
    }
    #endregion

    #region Core
    private AudioSource PlayInternal(SoundAction action, bool is3D, Vector3? pos)
    {
        if (!_defs.TryGetValue(action, out var def) || def.IsEmpty)
        {
            Debug.LogWarning($"[SoundManager]no sound: {action}");
            return null;
        }

        var src = GetSource();
        ConfigureSource(src, def, isLoop: false, is3D: is3D);

        if (pos.HasValue)
        {
            src.transform.position = pos.Value;
        }

        src.Play();
        StartCoroutine(ReturnToPoolWhenFinished(src));
        return src;
    }

    private AudioSource PlayLoopInternal(SoundAction action)
    {
        if (_looping.ContainsKey(action)) return _looping[action];

        if (!_defs.TryGetValue(action, out var def) || def.IsEmpty)
        {
            Debug.LogWarning($"[SoundManager]no sound: {action}");
            return null;
        }

        var src = GetSource();
        ConfigureSource(src, def, isLoop: true, is3D: def.playAs3D);
        src.Play();
        _looping[action] = src;
        return src;
    }

    private void StopLoopInternal(SoundAction action, float fadeOutSeconds)
    {
        if (!_looping.TryGetValue(action, out var src) || src == null) return;

        _looping.Remove(action);
        if (fadeOutSeconds > 0f && gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOutAndRelease(src, fadeOutSeconds));
        }
        else
        {
            ReleaseSource(src);
        }
    }
    #endregion

    #region Pooling & helpers
    private void WarmPool()
    {
        for (int i = 0; i < Mathf.Max(1, initialPoolSize); i++)
            _pool.Enqueue(CreateAudioSource());
    }

    private AudioSource CreateAudioSource()
    {
        var go = new GameObject("SFX_AudioSource");
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // domyślnie 2D
        src.rolloffMode = AudioRolloffMode.Linear;
        src.maxDistance = 25f;
        return src;
    }

    private AudioSource GetSource()
    {
        var src = _pool.Count > 0 ? _pool.Dequeue() : CreateAudioSource();
        _inUse.Add(src);
        src.gameObject.SetActive(true);
        return src;
    }

    private void ReleaseSource(AudioSource src)
    {
        if (src == null) return;
        src.Stop();
        src.clip = null;
        src.outputAudioMixerGroup = null;
        src.loop = false;
        src.transform.localPosition = Vector3.zero;
        src.spatialBlend = 0f;
        src.gameObject.SetActive(false);
        _inUse.Remove(src);
        _pool.Enqueue(src);
    }

    private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource src)
    {
        if (src == null) yield break;
        yield return new WaitWhile(() => src.isPlaying);
        ReleaseSource(src);
    }

    private System.Collections.IEnumerator FadeOutAndRelease(AudioSource src, float time)
    {
        if (src == null) yield break;
        float startVol = src.volume;
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, t / time);
            yield return null;
        }
        ReleaseSource(src);
    }

    private void ConfigureSource(AudioSource src, SoundDefinition def, bool isLoop, bool is3D)
    {
        var clip = def.GetRandomClip();
        src.clip = clip;
        src.loop = isLoop;
        src.volume = (def.volume * masterVolume);
        src.pitch = UnityEngine.Random.Range(def.pitchRange.x, def.pitchRange.y);
        src.outputAudioMixerGroup = def.output;
        src.spatialBlend = is3D ? 1f : 0f;

        if (is3D || def.playAs3D)
        {
            src.minDistance = def.minDistance;
            src.maxDistance = def.maxDistance;
            src.rolloffMode = def.rolloffMode;
        }
    }

    private void BuildDefinitionsDictionary()
    {
        _defs.Clear();
        foreach (var def in sounds)
        {
            if (def == null) continue;
            _defs[def.action] = def;
        }
    }
    #endregion
}

#region Inspector data

[Serializable]
public class SoundDefinition
{
    [Header("Akcja")]
    public SoundAction action;

    [Header("Audio Clips")]
    public List<AudioClip> clips = new();

    [Header("Setings")]
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 pitchRange = new Vector2(1f, 1f);
    public bool playAs3D = false;
    [Min(0.1f)] public float minDistance = 1f;
    [Min(1f)] public float maxDistance = 20f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

    [Header("Routing")]
    public AudioMixerGroup output;

    public bool IsEmpty => clips == null || clips.Count == 0;

    public AudioClip GetRandomClip()
    {
        if (IsEmpty) return null;
        if (clips.Count == 1) return clips[0];
        int idx = UnityEngine.Random.Range(0, clips.Count);
        return clips[idx];
    }
}

#endregion
