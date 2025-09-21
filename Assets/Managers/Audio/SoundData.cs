using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("Identificador único del sonido")]
    public string soundName;

    [Header("Clip de audio")]
    public AudioClip clip;

    [Header("Configuraciones")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    public bool loop = false;

    [Header("Espacialización")]
    [Tooltip("0 = 2D, 1 = 3D")]
    [Range(0f, 1f)] public float spatialBlend = 0f;

    [Header("Categoría (ej. Música, SFX, UI)")]
    public string category = "SFX";
}
