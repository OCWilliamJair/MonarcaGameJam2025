using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("Clip")]
    public AudioClip clip;
    public AudioChannel channel = AudioChannel.SFX;

    [Header("General Settings")]
    [Range(0f,1f)] public float volume = 1f;
    public bool spatial = false;

    [Header("3D Settings (si spatial = true)")]
    public float minDistance = 1f;
    public float maxDistance = 15f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
}
