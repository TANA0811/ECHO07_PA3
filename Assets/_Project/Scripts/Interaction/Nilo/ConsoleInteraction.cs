using TMPro;
using UnityEngine;

public class ConsoleInteraction : MonoBehaviour
{
    [Header("Configuración de interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private Light targetLight;

    [Header("Diagnóstico del reactor")]
    [SerializeField] private GameObject diagnosticPanel;

    [SerializeField] private TMP_Text consoleStatusText;

    [SerializeField] private TMP_Text consoleHeaderText;

    [SerializeField] private TMP_Text reactorStatusText;

    [SerializeField] private TMP_Text energyStatusText;

    [Header("Misión de recuperación energética")]
    [SerializeField]
    private EnergyCoreSequenceManager sequenceManager;

    [Header("Restauración del reactor")]
    [SerializeField]
    private ReactorRestorationManager reactorRestorationManager;

    private bool diagnosticOpen;

    private void Start()
    {
        diagnosticOpen = false;

        if (diagnosticPanel != null)
        {
            diagnosticPanel.SetActive(false);
        }

        if (targetLight != null)
        {
            targetLight.enabled = false;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (diagnosticOpen)
        {
            HandleOpenConsoleAction();
            return;
        }

        if (player == null)
        {
            Debug.LogWarning(
                "ECHO-07: Falta asignar el personaje."
            );
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
        {
            Debug.Log(
                "ECHO-07: Acércate más a la consola."
            );
            return;
        }

        OpenConsole();
    }

    private void HandleOpenConsoleAction()
    {
        if (reactorRestorationManager != null &&
            reactorRestorationManager.IsRestored)
        {
            CloseConsole();
            return;
        }

        if (sequenceManager != null &&
            sequenceManager.AllCoresCollected)
        {
            if (reactorRestorationManager != null)
            {
                Debug.Log(
                    "ECHO-07: Iniciando reparación del reactor."
                );

                SetConsoleMessage(
                    "RESTAURANDO REACTOR...\nESPERE."
                );

                reactorRestorationManager.StartRestoration();
            }
            else
            {
                Debug.LogWarning(
                    "ECHO-07: Falta asignar ReactorRestorationManager."
                );
            }

            return;
        }

        CloseConsole();
    }

    private void OpenConsole()
    {
        diagnosticOpen = true;

        if (diagnosticPanel != null)
        {
            diagnosticPanel.SetActive(true);
        }

        if (targetLight != null)
        {
            targetLight.enabled = true;
        }

        if (reactorRestorationManager != null &&
            reactorRestorationManager.IsRestored)
        {
            ShowRestoredState();
            return;
        }

        if (sequenceManager == null)
        {
            SetConsoleMessage(
                "ERROR: Falta EnergyCoreSequenceManager."
            );

            Debug.LogWarning(
                "ECHO-07: Falta asignar EnergyCoreSequenceManager."
            );
            return;
        }

        if (!sequenceManager.MissionStarted)
        {
            SetConsoleMessage(
                "DIAGNÓSTICO COMPLETADO.\nRecupera los 6 Energy Cores."
            );

            sequenceManager.StartMission();

            Debug.Log(
                "ECHO-07: Diagnóstico del reactor consultado."
            );

            return;
        }

        if (sequenceManager.AllCoresCollected)
        {
            SetConsoleMessage(
                "ENERGY 6/6\nECHO, repara el reactor.\nPresiona E nuevamente."
            );

            Debug.Log(
                "ECHO-07: Los 6 núcleos fueron recuperados. El sistema está listo para restablecerse."
            );

            return;
        }

        SetConsoleMessage(
            "Recupera los Energy Cores.\nENERGY " +
            sequenceManager.CollectedCount + "/" +
            sequenceManager.TotalCores
        );

        Debug.Log(
            "ECHO-07: Diagnóstico del reactor consultado."
        );
    }

    private void CloseConsole()
    {
        diagnosticOpen = false;

        if (diagnosticPanel != null)
        {
            diagnosticPanel.SetActive(false);
        }

        if (targetLight != null)
        {
            targetLight.enabled = false;
        }

        Debug.Log(
            "ECHO-07: Diagnóstico cerrado."
        );
    }

    private void SetConsoleMessage(string message)
    {
        if (consoleStatusText != null)
        {
            consoleStatusText.text = message;
        }
    }

    public void ShowRestoredState()
    {
        if (consoleHeaderText != null)
        {
            consoleHeaderText.text =
                "ECHO-07\nSISTEMA RESTAURADO";
        }

        if (reactorStatusText != null)
        {
            reactorStatusText.text =
                "REACTOR OPERATIVO";
        }

        if (energyStatusText != null)
        {
            energyStatusText.text =
                "SUMINISTRO ENERGÉTICO ESTABLE";
        }

        if (consoleStatusText != null)
        {
            consoleStatusText.text =
                "ESTADO: NORMAL\nMISIÓN CUMPLIDA";
        }

        Debug.Log(
            "ECHO-07: Consola actualizada al estado restaurado."
        );
    }
}