using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Cameras")]
    public List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    private CinemachineCamera currentCam;
    private CinemachineCamera lastCam;
    private Dictionary<string, CinemachineCamera> camDictionary;

    [Header("Player Camera")]
    public CinemachineCamera playerCamera;

    [Header("Blend Settings")]
    public float defaultBlendDuration = 1f;

    private CinemachineBrain brain;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        camDictionary = new Dictionary<string, CinemachineCamera>();
        foreach (var cam in cameras)
        {
            if (!camDictionary.ContainsKey(cam.name))
                camDictionary.Add(cam.name, cam);

            cam.Priority = 0;
        }

        if (cameras.Count > 0)
        {
            currentCam = cameras[0];
            currentCam.Priority = 10;
            lastCam = currentCam;
        }

        brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null)
            Debug.LogWarning("CameraManager: Main Camera necesita un CinemachineBrain.");
    }

    #region Cambio de cámara

    public void SwitchCamera(string camName, bool saveLast = true, float blendTime = -1f)
    {
        if (!camDictionary.ContainsKey(camName)) return;
        SwitchCamera(camDictionary[camName], saveLast, blendTime);
    }

    public void  SwitchCamera(CinemachineCamera newCam, bool saveLast = true, float blendTime = 1f)
    {
        if (newCam == null || newCam == currentCam) return;

        if (saveLast)
            lastCam = currentCam;

        brain.DefaultBlend.Time = blendTime;

        // Prioridades para forzar blend
        newCam.Priority = 20;
        currentCam.Priority = 10;

        // Ajusta prioridades finales
        currentCam.Priority = 0;
        newCam.Priority = 10;
        currentCam = newCam;

    }

    public async UniTask SwitchCameraTemporarily(string camName, float duration, float blendTime = -1f)
    {
        if (!camDictionary.ContainsKey(camName)) return;
        await SwitchCameraTemporarily(camDictionary[camName], duration, blendTime);
    }

    public async UniTask SwitchCameraTemporarily(CinemachineCamera newCam, float duration, float blendTime = -1f)
    {
        if (newCam == null || newCam == currentCam) return;

        var previousCam = currentCam;

        // Bloquea acciones desde el inicio hasta que termine la cámara temporal
        PlayerActionBlocker.Instance.BlockAll();

        // Cambia a la nueva cámara
        SwitchCamera(newCam, saveLast: true, blendTime);

        // Espera toda la duración de la cámara temporal
        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        // Vuelve a la cámara anterior
        SwitchCamera(previousCam, saveLast: false, blendTime);
    }

    public void ReturnToLastCamera(float blendTime = -1f)
    {
        if (lastCam != null && lastCam != currentCam)
           SwitchCamera(lastCam, saveLast: false, blendTime);
    }

    public void SetPermanentCamera(string camName, float blendTime = -1f)
    {
        if (!camDictionary.ContainsKey(camName)) return;
        SetPermanentCamera(camDictionary[camName], blendTime);
    }

    public void SetPermanentCamera(CinemachineCamera newCam, float blendTime = -1f)
    {
        if (newCam == null) return;
        lastCam = null;
        SwitchCamera(newCam, saveLast: false, blendTime);
    }

    public void SwitchToPlayerCamera(float blendTime = -1f)
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("CameraManager: No se asignó una cámara de Player en el inspector.");
            return;
        }

        SwitchCamera(playerCamera, saveLast: false, blendTime);
    }

    #endregion

    #region Helpers

    public CinemachineCamera GetCurrentCamera() => currentCam;

    public CinemachineCamera GetCameraByName(string camName)
    {
        if (camDictionary.TryGetValue(camName, out var cam))
            return cam;
        return null;
    }

    #endregion
}
