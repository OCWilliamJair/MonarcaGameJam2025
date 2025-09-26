using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class DrinkCoffee : ActivityBase
{
    [SerializeField] private CinemachineCamera _camera;

    [SerializeField] private SoundData _sound;

    [SerializeField] private GameObject _cupModelToDrink;

    [SerializeField] private GameObject _cupModelToTake;
    public override void ActivityProcess()
    {
        DrinkCoffeeRoutine().Forget();
    }

    async UniTask DrinkCoffeeRoutine()
    {
        if (_camera == null) return;

        // Cambiar a la cámara de beber café
        CameraManager.Instance.SwitchCamera(_camera, true, 0.5f);
        PlayerActionBlocker.Instance.BlockAll();

        _cupModelToDrink.SetActive(true);       
        // Guardar estado inicial
        Transform camTransform = _camera.transform;
        Quaternion initialRot = camTransform.localRotation;

        // Reproducir sonido
        AudioManager.Instance.Play(_sound.name);

        // Simulación de beber café
        float duration = 2.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            
            float tilt = Mathf.Sin(t * Mathf.PI);
            camTransform.localRotation = initialRot *
                                         Quaternion.Euler(-15f * tilt, 0f, 2f * Mathf.Sin(t * 6f));

            await UniTask.Yield();
        }

        // Restaurar rotación
        camTransform.localRotation = initialRot;

        // Regresar a la cámara de gameplay (asumiendo que tienes una principal configurada)
        CameraManager.Instance.ReturnToLastCamera(0.5f);
        CompleteActivity();
        _cupModelToDrink.SetActive(false);
        PlayerActionBlocker.Instance.UnblockAll();
        _cupModelToTake.SetActive(false);
        
    }
}
