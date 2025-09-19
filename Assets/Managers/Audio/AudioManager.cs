using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singletons<AudioManager>
{
    [System.Serializable]
    public class AudioChannelSettings
    {
        public AudioChannel channel;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField] private List<AudioChannelSettings> channelSettings;
    private Dictionary<AudioChannel, float> channelVolumes = new();

    private AudioSource musicSource;

    private void Awake()
    {
        foreach (var setting in channelSettings)
            channelVolumes[setting.channel] = setting.volume;
    }

    public void PlayOneShot(SoundData sound, Vector3? position = null)
    {
        if (sound == null || sound.clip == null) return;

        GameObject tempGO = new GameObject($"TempAudio_{sound.clip.name}");
        tempGO.transform.position = position ?? Vector3.zero;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = sound.clip;
        source.volume = sound.volume * channelVolumes[sound.channel];
        source.loop = false;

        if (sound.spatial || position.HasValue)
        {
            source.spatialBlend = 1f;
            source.minDistance = sound.minDistance;
            source.maxDistance = sound.maxDistance;
            source.rolloffMode = sound.rolloffMode;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();
        Destroy(tempGO, sound.clip.length);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        var music = GetOrCreateMusicSource();
        music.clip = clip;
        music.loop = loop;
        music.volume = channelVolumes[AudioChannel.Music];
        music.spatialBlend = 0f; 
        music.Play();
    }

    private AudioSource GetOrCreateMusicSource()
    {
        if (musicSource == null)
        {
            var go = new GameObject("MusicSource");
            go.transform.SetParent(transform);
            musicSource = go.AddComponent<AudioSource>();
            DontDestroyOnLoad(go);
        }
        return musicSource;
    }

    public void SetChannelVolume(AudioChannel channel, float volume)
    {
        channelVolumes[channel] = Mathf.Clamp01(volume);
        if (channel == AudioChannel.Music && musicSource != null)
            musicSource.volume = channelVolumes[channel];
    }

    public float GetChannelVolume(AudioChannel channel) =>
        channelVolumes.ContainsKey(channel) ? channelVolumes[channel] : 1f;
}
