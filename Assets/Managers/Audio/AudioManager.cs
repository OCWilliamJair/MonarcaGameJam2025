using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singletons<AudioManager>
{
    [Header("Lista de sonidos disponibles (asignar en el inspector)")]
    public List<SoundData> sounds = new List<SoundData>();

    private Dictionary<string, AudioSource> activeSounds = new Dictionary<string, AudioSource>();

    [Header("Mixer principal de Audio (Opcional)")]
    public AudioMixerGroup defaultMixer;

    /// <summary>
    /// Reproduce un sonido por su nombre.
    /// </summary>
    public void Play(string soundName, Vector3? position = null)
    {
        if (activeSounds.ContainsKey(soundName))
        {
            Debug.LogWarning($"El sonido '{soundName}' ya está reproduciéndose.");
            return;
        }

        SoundData data = sounds.Find(s => s.soundName == soundName);
        if (data == null)
        {
            Debug.LogError($"No se encontró el sonido con nombre: {soundName}");
            return;
        }

        GameObject go = new GameObject($"Audio_{data.soundName}");
        go.transform.SetParent(this.transform);

        // Si se pasa una posición → poner el sonido en el mundo
        if (position.HasValue)
            go.transform.position = position.Value;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;
        source.spatialBlend = data.spatialBlend; // 👈 aquí se configura si es 2D o 3D
        if (defaultMixer != null) source.outputAudioMixerGroup = defaultMixer;

        source.Play();

        if (!data.loop)
        {
            Destroy(go, data.clip.length);
        }
        else
        {
            activeSounds[soundName] = source;
        }
    }

    /// <summary>
    /// Detiene un sonido en reproducción.
    /// </summary>
    public void Stop(string soundName)
    {
        if (!activeSounds.ContainsKey(soundName)) return;

        AudioSource source = activeSounds[soundName];
        Destroy(source.gameObject);
        activeSounds.Remove(soundName);
    }

    /// <summary>
    /// Reproduce música (solo una a la vez).
    /// </summary>
    public void PlayMusic(string soundName)
    {
        // Si ya hay música, detenerla
        List<string> toStop = new List<string>();
        foreach (var kvp in activeSounds)
        {
            if (kvp.Value.loop) toStop.Add(kvp.Key);
        }
        foreach (var s in toStop) Stop(s);

        Play(soundName);
    }

    /// <summary>
    /// Reproduce un efecto de sonido en el mundo (3D) o en el centro (2D).
    /// </summary>
    public void PlaySFX(string soundName, Vector3? position = null)
    {
        SoundData data = sounds.Find(s => s.soundName == soundName);
        if (data == null)
        {
            Debug.LogError($"No se encontró el SFX: {soundName}");
            return;
        }

        if (data.spatialBlend == 0f)
        {
            // 2D: reproducir sin posición
            AudioSource.PlayClipAtPoint(data.clip, Vector3.zero, data.volume);
        }
        else
        {
            // 3D: reproducir en posición
            Vector3 pos = position ?? Vector3.zero;
            AudioSource.PlayClipAtPoint(data.clip, pos, data.volume);
        }
    }
}
