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

    public async UniTask SwitchCamera(string camName, bool saveLast = true, float blendTime = -1f)
    {
        if (!camDictionary.ContainsKey(camName)) return;
        await SwitchCamera(camDictionary[camName], saveLast, blendTime);
    }

    public async UniTask SwitchCamera(CinemachineCamera newCam, bool saveLast = true, float blendTime = -1f)
    {
        if (newCam == null || newCam == currentCam) return;

        if (saveLast)
            lastCam = currentCam;

        blendTime = blendTime < 0 ? defaultBlendDuration : blendTime;

        // Bloquea todas las acciones del jugador
        PlayerActionBlocker.Instance.BlockAll();

        // Prioridades para forzar blend
        newCam.Priority = 20;
        currentCam.Priority = 10;

        // Espera el blend de Cinemachine o fallback por tiempo
        if (brain != null)
        {
            await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(blendTime)),
                UniTask.WaitUntil(() => brain.ActiveBlend == null)
            );
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(blendTime));
        }

        // Ajusta prioridades finales
        currentCam.Priority = 0;
        newCam.Priority = 10;
        currentCam = newCam;

        // Desbloquea las acciones
        PlayerActionBlocker.Instance.UnblockAll();
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
        await SwitchCamera(newCam, saveLast: true, blendTime);

        // Espera toda la duración de la cámara temporal
        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        // Vuelve a la cámara anterior
        await SwitchCamera(previousCam, saveLast: false, blendTime);
    }

    public async UniTask ReturnToLastCamera(float blendTime = -1f)
    {
        if (lastCam != null && lastCam != currentCam)
            await SwitchCamera(lastCam, saveLast: false, blendTime);
    }

    public async UniTask SetPermanentCamera(string camName, float blendTime = -1f)
    {
        if (!camDictionary.ContainsKey(camName)) return;
        await SetPermanentCamera(camDictionary[camName], blendTime);
    }

    public async UniTask SetPermanentCamera(CinemachineCamera newCam, float blendTime = -1f)
    {
        if (newCam == null) return;
        lastCam = null;
        await SwitchCamera(newCam, saveLast: false, blendTime);
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
