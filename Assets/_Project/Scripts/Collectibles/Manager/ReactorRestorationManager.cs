using System.Collections;
using UnityEngine;

public class ReactorRestorationManager : MonoBehaviour
{
    [Header("Pantalla de la consola")]
    [SerializeField]
    private ConsoleFailureBlink consoleFailureBlink;

    [Header("Interfaz de consola")]
    [SerializeField]
    private ConsoleInteraction consoleInteraction;

    [Header("Reactor")]
    [SerializeField]
    private Light reactorCyanLight;

    [SerializeField]
    private MonoBehaviour reactorFlickerScript;

    [SerializeField]
    private float restoredLightIntensity = 8f;

    [SerializeField]
    private float restorationDuration = 4f;

    [Header("Luces y parpadeos a desactivar")]
    [SerializeField]
    private MonoBehaviour[] blinkingScriptsToDisable;

    [SerializeField]
    private Light[] emergencyLightsToDisable;

    [Header("Luces estables a activar")]
    [SerializeField]
    private Light[] stableLightsToEnable;

    [Header("Guía de misión")]
    [SerializeField]
    private ObjectiveGuideManager objectiveGuideManager;

    [Header("Audio de emergencia")]
    [SerializeField]
    private AudioSource emergencyAlarmSource;

    [SerializeField]
    private AudioSource reactorSuspenseSource;

    [Header("Audio de restauración")]
    [SerializeField]
    private AudioSource restoredAmbienceSource;

    [SerializeField]
    private AudioSource repairSfxSource;

    private bool isRestored = false;
    private bool restorationInProgress = false;

    public bool IsRestored => isRestored;

    public bool RestorationInProgress => restorationInProgress;

    public void StartRestoration()
    {
        // Evitar que la restauración se ejecute varias veces.
        if (isRestored || restorationInProgress)
        {
            return;
        }

        StartCoroutine(RestoreSequence());
    }

    private IEnumerator RestoreSequence()
    {
        restorationInProgress = true;

        Debug.Log(
            "ECHO-07: Iniciando restauración del reactor."
        );

        // PASO 1:
        // Detener el parpadeo del reactor.
        if (reactorFlickerScript != null)
        {
            reactorFlickerScript.enabled = false;
        }

        // PASO 2:
        // Detener los scripts de parpadeo de emergencia.
        if (blinkingScriptsToDisable != null)
        {
            foreach (MonoBehaviour script in blinkingScriptsToDisable)
            {
                if (script != null)
                {
                    script.enabled = false;
                }
            }
        }

        // PASO 3:
        // Reproducir sonido de restauración.
        if (repairSfxSource != null)
        {
            repairSfxSource.Play();

            Debug.Log(
                "ECHO-07: Sonido de restauración activado."
            );
        }

        // PASO 4:
        // Detener la alarma de emergencia.
        if (emergencyAlarmSource != null)
        {
            emergencyAlarmSource.Stop();

            Debug.Log(
                "ECHO-07: Alarma de emergencia detenida."
            );
        }

        // PASO 5:
        // Detener la música de suspenso del reactor.
        if (reactorSuspenseSource != null)
        {
            reactorSuspenseSource.Stop();

            Debug.Log(
                "ECHO-07: Música de suspenso detenida."
            );
        }

        // PASO 6:
        // Intensificar gradualmente la luz cyan del reactor.
        if (reactorCyanLight != null)
        {
            reactorCyanLight.enabled = true;

            reactorCyanLight.color = new Color(
                0.4f,
                1f,
                1f
            );

            float initialIntensity =
                reactorCyanLight.intensity;

            float elapsedTime = 0f;

            float duration = Mathf.Max(
                0.01f,
                restorationDuration
            );

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(
                    elapsedTime / duration
                );

                reactorCyanLight.intensity = Mathf.Lerp(
                    initialIntensity,
                    restoredLightIntensity,
                    progress
                );

                yield return null;
            }

            reactorCyanLight.intensity =
                restoredLightIntensity;
        }
        else
        {
            yield return new WaitForSeconds(
                restorationDuration
            );
        }

        // PASO 7:
        // Apagar las luces rojas de emergencia.
        if (emergencyLightsToDisable != null)
        {
            foreach (Light emergencyLight in emergencyLightsToDisable)
            {
                if (emergencyLight != null)
                {
                    emergencyLight.enabled = false;
                }
            }
        }

        // PASO 8:
        // Encender luces normales de la estación,
        // si existen luces asignadas.
        if (stableLightsToEnable != null)
        {
            foreach (Light stableLight in stableLightsToEnable)
            {
                if (stableLight != null)
                {
                    stableLight.enabled = true;
                }
            }
        }

        // PASO 9:
        // Reparar visualmente la pantalla física de la consola.
        if (consoleFailureBlink != null)
        {
            consoleFailureBlink.RepairConsole();
        }
        else
        {
            Debug.LogWarning(
                "ECHO-07: Falta asignar ConsoleFailureBlink."
            );
        }

        // PASO 10:
        // Cambiar los textos de la consola al estado restaurado.
        if (consoleInteraction != null)
        {
            consoleInteraction.ShowRestoredState();
        }
        else
        {
            Debug.LogWarning(
                "ECHO-07: Falta asignar ConsoleInteraction."
            );
        }

        // PASO 11:
        // Reproducir la ambientación tranquila/restaurada.
        if (restoredAmbienceSource != null)
        {
            if (!restoredAmbienceSource.isPlaying)
            {
                restoredAmbienceSource.Play();

                Debug.Log(
                    "ECHO-07: Ambientación restaurada activada."
                );
            }
        }

        // PASO 12:
        // Marcar el reactor como restaurado.
        isRestored = true;
        restorationInProgress = false;

        // PASO 13:
        // Finalizar la misión y ocultar la guía holográfica.
        if (objectiveGuideManager != null)
        {
            objectiveGuideManager.CompleteMission(
                "MISIÓN CUMPLIDA: Reactor restablecido."
            );
        }
        else
        {
            Debug.LogWarning(
                "ECHO-07: Falta asignar ObjectiveGuideManager."
            );
        }

        Debug.Log(
            "ECHO-07: Reactor restaurado correctamente."
        );
    }
}