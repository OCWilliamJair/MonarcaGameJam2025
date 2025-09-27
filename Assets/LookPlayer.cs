using UnityEngine;

public class LookPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Update()
    {
        transform.LookAt(player.transform);
    }
}
