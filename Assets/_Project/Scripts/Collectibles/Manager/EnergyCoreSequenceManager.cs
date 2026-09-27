using System.Collections;
using TMPro;
using UnityEngine;

public class EnergyCoreSequenceManager : MonoBehaviour
{
    [Header("Energy Cores en orden de recolección")]
    [SerializeField] private EnergyCore3D[] energyCores =
        new EnergyCore3D[6];

    [Header("Tiempo antes de mostrar el siguiente núcleo")]
    [SerializeField] private float nextCoreDelay = 2.2f;

    [Header("Interfaz del contador")]
    [SerializeField] private GameObject energyCounterUI;

    [SerializeField] private TMP_Text energyCounterText;

    [Header("Guía del objetivo")]
    [SerializeField] private ObjectiveGuideManager objectiveGuideManager;

    [SerializeField] private Transform consoleTarget;

    [Header("Audio de recolección")]
    [SerializeField] private AudioSource pickupAudioSource;

    [SerializeField] private AudioClip pickupClip;

    private int collectedCount = 0;
    private bool missionStarted = false;

    public int CollectedCount => collectedCount;

    public int TotalCores =>
        energyCores != null ? energyCores.Length : 0;

    public bool MissionStarted => missionStarted;

    public bool AllCoresCollected =>
        missionStarted && collectedCount == TotalCores;

    private void Awake()
    {
        if (energyCounterUI != null)
        {
            energyCounterUI.SetActive(false);
        }

        collectedCount = 0;
        missionStarted = false;

        UpdateCounter();

        if (energyCores == null || energyCores.Length != 6)
        {
            Debug.LogError(
                "ECHO-07: Debes configurar exactamente 6 Energy Cores."
            );
            return;
        }

        foreach (EnergyCore3D core in energyCores)
        {
            if (core != null)
            {
                core.gameObject.SetActive(false);
            }
        }

        Debug.Log(
            "ECHO-07: Esperando el diagnóstico para iniciar la misión."
        );
    }

    public void StartMission()
    {
        if (missionStarted)
            return;

        if (energyCores == null || energyCores.Length != 6)
        {
            Debug.LogError(
                "ECHO-07: No se puede iniciar la misión. Se necesitan 6 Energy Cores."
            );
            return;
        }

        for (int i = 0; i < energyCores.Length; i++)
        {
            if (energyCores[i] == null)
            {
                Debug.LogError(
                    "ECHO-07: Falta asignar el Energy Core " + (i + 1)
                );
                return;
            }
        }

        missionStarted = true;

        UpdateCounter();

        if (energyCounterUI != null)
        {
            energyCounterUI.SetActive(true);
        }

        energyCores[0].gameObject.SetActive(true);
        UpdateGuideToCurrentCore();

        Debug.Log(
            "ECHO-07: Misión iniciada. Energía recuperada 0/6."
        );
    }

    public void RegisterCollection(EnergyCore3D collectedCore)
    {
        if (!missionStarted)
            return;

        if (collectedCount >= TotalCores)
            return;

        if (energyCores[collectedCount] != collectedCore)
        {
            Debug.LogWarning(
                "ECHO-07: Se intentó recoger un núcleo fuera de orden."
            );
            return;
        }

        collectedCount++;
        UpdateCounter();

        // NUEVO: reproducir sonido al recoger un Energy Core
        if (pickupAudioSource != null && pickupClip != null)
        {
            pickupAudioSource.PlayOneShot(pickupClip);
        }

        Debug.Log(
            "ECHO-07: Energía recuperada " +
            collectedCount + "/" + TotalCores
        );

        if (AllCoresCollected)
        {
            Debug.Log(
                "ECHO-07: Energía recuperada 6/6. Regresa a la consola para restablecer el sistema."
            );

            if (objectiveGuideManager != null && consoleTarget != null)
            {
                objectiveGuideManager.SetTarget(
                    consoleTarget,
                    "Regresa a la consola y repara el reactor presionando E."
                );
            }

            return;
        }

        int nextCoreIndex = collectedCount;
        StartCoroutine(ActivateNextCore(nextCoreIndex));
    }

    private IEnumerator ActivateNextCore(int nextCoreIndex)
    {
        yield return new WaitForSeconds(nextCoreDelay);

        if (nextCoreIndex >= TotalCores)
            yield break;

        EnergyCore3D nextCore = energyCores[nextCoreIndex];

        if (nextCore != null)
        {
            nextCore.gameObject.SetActive(true);

            Debug.Log(
                "ECHO-07: Energy Core " +
                (nextCoreIndex + 1) +
                " desbloqueado."
            );

            UpdateGuideToCurrentCore();
        }
    }

    private void UpdateGuideToCurrentCore()
    {
        if (objectiveGuideManager == null)
            return;

        if (collectedCount >= TotalCores)
            return;

        EnergyCore3D currentCore = energyCores[collectedCount];

        if (currentCore != null)
        {
            objectiveGuideManager.SetTarget(
                currentCore.transform,
                "Recolecta el Energy Core " +
                (collectedCount + 1) + " de " + TotalCores + "."
            );
        }
    }

    private void UpdateCounter()
    {
        if (energyCounterText != null)
        {
            energyCounterText.text =
                "ENERGY " + collectedCount + "/" + TotalCores;
        }
    }
}