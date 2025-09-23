using System.Collections.Generic;
using UnityEngine;

public class PlayerActionBlocker : MonoBehaviour
{
    public enum PlayerAction
    {
        Move,
        Look,
        Interact,
        Jump,
        Attack,       
    }
    public static PlayerActionBlocker Instance { get; private set; }

    private HashSet<PlayerAction> blockedActions = new HashSet<PlayerAction>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    /// <summary>Bloquea una acción específica</summary>
    public void BlockAction(PlayerAction action)
    {
        blockedActions.Add(action);
    }

    /// <summary>Desbloquea una acción específica</summary>
    public void UnblockAction(PlayerAction action)
    {
        blockedActions.Remove(action);
    }

    /// <summary>Verifica si una acción está bloqueada</summary>
    public bool IsBlocked(PlayerAction action)
    {
        return blockedActions.Contains(action);
    }

    /// <summary>Bloquea todas las acciones</summary>
    public void BlockAll()
    {
        foreach (PlayerAction action in System.Enum.GetValues(typeof(PlayerAction)))
            blockedActions.Add(action);
        Debug.Log("Todas las acciones bloqueadas");
    }

    /// <summary>Desbloquea todas las acciones</summary>
    public void UnblockAll()
    {
        blockedActions.Clear();
        Debug.Log("Todas las acciones desbloqueadas");
    }
}
