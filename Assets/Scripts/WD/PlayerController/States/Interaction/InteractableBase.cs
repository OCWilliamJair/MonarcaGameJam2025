using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Header("Opcional: material o outline para resaltar")]
    public Renderer targetRenderer;
    private Material originalMaterial;
    public Material highlightMaterial;

    private void Awake()
    {
        if (targetRenderer != null)
            originalMaterial = targetRenderer.material;
    }

    // Se llama cuando el jugador mira el objeto
    public virtual void OnFocus()
    {
        if (targetRenderer != null && highlightMaterial != null)
            targetRenderer.material = highlightMaterial;
    }

    // Se llama cuando el jugador deja de mirar el objeto
    public virtual void OnLoseFocus()
    {
        if (targetRenderer != null && originalMaterial != null)
            targetRenderer.material = originalMaterial;
    }

    // Se llama al presionar el botón de interacción
    public abstract void Interact();
}
