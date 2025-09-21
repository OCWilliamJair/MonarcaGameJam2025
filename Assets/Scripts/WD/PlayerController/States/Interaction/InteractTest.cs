using Cysharp.Threading.Tasks;
using UnityEngine;

public class InteractTest : InteractableBase
{
    public override void Interact()
    {
        Debug.Log("Interactuando");
        PlayerActionBlocker.Instance.BlockAll();
        CameraManager.Instance.SwitchCameraTemporarily("Box", 4f).Forget();
    }   
}
