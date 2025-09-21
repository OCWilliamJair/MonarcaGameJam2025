using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

public class Bed : InteractableBase
{
    [SerializeField] private GameObject cameraBed;

    private bool inBed = false;

    [SerializeField] private PlayerInput player;
    public override void Interact()
    {
        if (inBed) return;

        cameraBed.SetActive(true);      
        inBed = true;
        PlayerActionBlocker.Instance.BlockAll();
    }
    private void OnEnable()
    {      
        player.actions["Cancel"].started += OnCancelPerformed;
    }

   
    private void OnDisable()
    {
        player.actions["Cancel"].started -= OnCancelPerformed;
    }
    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        CancelBed().Forget();
    }

    async UniTask CancelBed()
    {
        if(!inBed) return;

        cameraBed.SetActive(false);
        inBed = false;
        await UniTask.Delay(3000);
        PlayerActionBlocker.Instance.UnblockAll();                
    }
}
