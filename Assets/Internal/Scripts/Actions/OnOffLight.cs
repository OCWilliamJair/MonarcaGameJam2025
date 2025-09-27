using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class OnOffLight : InteractableBase
{
    [Header("Configuración de la luz")]
    [SerializeField] private SoundData _sound;

    public Light targetLight;             
    public bool startOn = false;          
    [Range(0f, 10f)] public float onIntensity = 1f;  
    [Range(0f, 10f)] public float offIntensity = 0f; 
    public float switchDuration = 0.5f;   

    private bool isOn;


    private void Start()
    {
        if (targetLight != null)
        {
            isOn = startOn;
            targetLight.intensity = isOn ? onIntensity : offIntensity;
            targetLight.enabled = isOn;
        }
    }

    public override void Interact()
    {
        if (!canInteract || targetLight == null) return;

        isOn = !isOn;

        if (isOn)
        {
            AudioManager.Instance.Play(_sound.name, transform.position);
            targetLight.enabled = true;
            targetLight.DOIntensity(onIntensity, switchDuration);

        }
        else
        {
            AudioManager.Instance.Play(_sound.name, transform.position);
            targetLight.DOIntensity(offIntensity, switchDuration)
                .OnComplete(() => {
                    if (offIntensity <= 0f) targetLight.enabled = false;
                });
        }

        Debug.Log($"Interruptor activado ? Luz {(isOn ? "ENCENDIDA" : "APAGADA")}");
    }
}
