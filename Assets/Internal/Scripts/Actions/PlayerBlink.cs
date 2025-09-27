using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerBlink : MonoBehaviour
{
    [Header("Material del Shader FullScreen")]
    [SerializeField] private Material blinkMaterial;

    [Header("Tiempos del parpadeo (segundos)")]
    [SerializeField] private float closeDuration = 0.08f; 
    [SerializeField] private float holdDuration = 0.05f;  
    [SerializeField] private float openDuration = 0.15f;  


    void Update()
    {      
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Blink().Forget();
        }
    }

    async UniTask Blink()
    {
        blinkMaterial.DOFloat(0.4f, "_BlinkProgress", closeDuration);
        await UniTask.Delay(2000);
        blinkMaterial.DOFloat(1f, "_BlinkProgress", openDuration);
    }
}
