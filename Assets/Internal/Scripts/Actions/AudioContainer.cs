using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    [SerializeField] SoundData _sound;
    public void playSound()
    {
        AudioManager.Instance.Play(_sound.name, transform.position);
    }
}
