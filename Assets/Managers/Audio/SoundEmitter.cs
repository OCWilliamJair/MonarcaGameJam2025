using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public SoundData sound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = sound.spatial ? 1f : 0f;
        audioSource.minDistance = sound.minDistance;
        audioSource.maxDistance = sound.maxDistance;
        audioSource.rolloffMode = sound.rolloffMode;
        audioSource.loop = false;
    }

    public void Play()
    {
        if (sound == null || sound.clip == null) return;

        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume
                             * AudioManager.Instance.GetChannelVolume(sound.channel);
        audioSource.Play();
    }
}
