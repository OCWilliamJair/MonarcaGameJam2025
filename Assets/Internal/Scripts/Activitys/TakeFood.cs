using UnityEngine;

public class TakeFood : ActivityBase
{
    [SerializeField] private GameObject _food;

    [SerializeField] private SoundData _takeFoodSound;

    public override void ActivityProcess()
    {
        _food.SetActive(true);
        AudioManager.Instance.Play(_takeFoodSound.name);
        CompleteActivity();
    }
}
