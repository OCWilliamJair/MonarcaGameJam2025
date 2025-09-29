using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioContainer> audioContainers = new List<AudioContainer>();
    [SerializeField] private float minDelay = 0.05f; // en segundos
    [SerializeField] private float maxDelay = 0.2f;  // en segundos

    private bool isPlaying = false;

    public void PlayAllOnce()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            PlaySequence().Forget();
        }
    }

    private async UniTaskVoid PlaySequence()
    {
        // Copiamos la lista para no modificar la original
        List<AudioContainer> tempList = new List<AudioContainer>(audioContainers);

        // Mientras aún haya sonidos en la lista
        while (tempList.Count > 0)
        {
            // Elegir uno aleatorio
            int index = Random.Range(0, tempList.Count);
            AudioContainer chosen = tempList[index];

            // Reproducir sonido
            if (chosen != null)
                chosen.playSound();

            // Eliminarlo de la lista temporal para que no se repita
            tempList.RemoveAt(index);

            // Esperar un delay aleatorio en milisegundos
            float delay = Random.Range(minDelay, maxDelay);
            await UniTask.Delay((int)(delay * 1000));
        }

        // Terminó la secuencia
        isPlaying = false;
    }
}
