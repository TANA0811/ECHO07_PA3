using System.Collections;
using UnityEngine;

public class ReactorRestorationController : MonoBehaviour
{
    [Header("Reactor")]
    [SerializeField] private Light reactorLight;
    [SerializeField] private Renderer reactorRenderer;

    [Header("Luces de emergencia")]
    [SerializeField] private Light[] emergencyLights;

    [Header("Configuración de restauración")]
    [SerializeField] private float restorationDuration = 4f;
    [SerializeField] private float restoredLightIntensity = 8f;

    private bool isRestored = false;

    public bool IsRestored => isRestored;

    public void RestoreReactor()
    {
        if (isRestored)
            return;

        isRestored = true;

        StartCoroutine(RestorationSequence());
    }

    private IEnumerator RestorationSequence()
    {
        Debug.Log("ECHO-07: Iniciando restauración del reactor.");

        // Apagar las luces rojas de emergencia.
        foreach (Light emergencyLight in emergencyLights)
        {
            if (emergencyLight != null)
                emergencyLight.enabled = false;
        }

        // Incrementar gradualmente la intensidad cyan.
        if (reactorLight != null)
        {
            reactorLight.enabled = true;

            float initialIntensity = reactorLight.intensity;
            float elapsedTime = 0f;

            while (elapsedTime < restorationDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(
                    elapsedTime / restorationDuration
                );

                reactorLight.intensity = Mathf.Lerp(
                    initialIntensity,
                    restoredLightIntensity,
                    progress
                );

                yield return null;
            }

            reactorLight.intensity = restoredLightIntensity;
        }

        Debug.Log("ECHO-07: Reactor restaurado correctamente.");
    }
}