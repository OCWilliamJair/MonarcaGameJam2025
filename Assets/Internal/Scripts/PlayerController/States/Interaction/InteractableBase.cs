using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Header("Opcional: material u outline para resaltar")]
    public Renderer targetRenderer;
    private Material[] originalMaterials; 
    public Material outlineMaterial;      
    public bool canInteract = true;

    protected virtual void Awake()
    {
        if (targetRenderer != null)
            originalMaterials = targetRenderer.materials; 
    }

    public virtual void OnFocus()
    {
        if (targetRenderer != null && outlineMaterial != null && canInteract)
        {
            var currentMats = targetRenderer.materials;
            foreach (var m in currentMats)
            {
                if (m == outlineMaterial)
                    return;
            }

            Material[] mats = new Material[currentMats.Length + 1];
            for (int i = 0; i < currentMats.Length; i++)
                mats[i] = currentMats[i];

            mats[mats.Length - 1] = outlineMaterial;

            targetRenderer.materials = mats;
        }
    }

    public virtual void OnLoseFocus()
    {
        if (targetRenderer != null && originalMaterials != null)
        {
            targetRenderer.materials = originalMaterials;
        }
    }

    public abstract void Interact();

    public void SetInteract(bool value)
    {
        canInteract = value;
    }
}
