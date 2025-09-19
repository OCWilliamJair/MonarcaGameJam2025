using UnityEngine;

public class SoundTester : MonoBehaviour
{
    public SoundData soundA;
    public SoundData soundB;
    public SoundData soundC;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AudioManager.Instance.PlayOneShot(soundA, transform.position);

        if (Input.GetKeyDown(KeyCode.S))
            AudioManager.Instance.PlayOneShot(soundB, transform.position);

        if (Input.GetKeyDown(KeyCode.D))
            AudioManager.Instance.PlayOneShot(soundC, transform.position);
    }
}
