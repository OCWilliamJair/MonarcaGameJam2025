using UnityEngine;

public class PlayPlayerSound : MonoBehaviour
{
    [SerializeField] private SoundData[] _sounds;

    [SerializeField] private GameObject _rightPosition;

    [SerializeField] private GameObject _leftPosition;

    [SerializeField] private GameObject _upPosition;

    [SerializeField] private GameObject _downPosition;


    public void PlaySoundRight(int num)
    {
        AudioManager.Instance.Play(_sounds[num].name, _rightPosition.transform.position);
    }

    public void PlaySoundLeft(int num)
    {
        AudioManager.Instance.Play(_sounds[num].name, _leftPosition.transform.position);
    }

    public void PlaySoundUp(int num)
    {
        AudioManager.Instance.Play(_sounds[num].name, _upPosition.transform.position);
    }

    public void PlaySoundDown(int num)
    {
        AudioManager.Instance.Play(_sounds[num].name, _downPosition.transform.position);
    }
}
