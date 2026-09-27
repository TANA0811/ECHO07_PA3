using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;

public class ObjectiveGuideManager : MonoBehaviour
{
    [Serializable]
    public class ObjectiveCameraPoint
    {
        [Tooltip("Energy Core u objeto que debe mostrar la cámara")]
        public Transform target;

        [Tooltip("Punto interior desde donde se filmará ese objeto")]
        public Transform cameraPoint;
    }

    [Header("Flecha guía")]
    [SerializeField]
    private GameObject guideArrow;

    [SerializeField]
    private Vector3 arrowOffset = new Vector3(0f, 2.5f, 0f);

    [SerializeField]
    private float rotationSpeed = 90f;

    // NUEVO
    [Header("Flotación de la guía")]
    [SerializeField]
    private float floatAmplitude = 0.15f;

    [SerializeField]
    private float floatSpeed = 2f;

    [Header("Texto del objetivo")]
    [SerializeField]
    private TMP_Text objectiveText;

    [Header("Objetivo inicial")]
    [SerializeField]
    private Transform initialTarget;

    [SerializeField]
    private string initialObjective =
        "Ve a la consola científica.";

    [Header("Cámaras Cinemachine")]
    [SerializeField]
    private CinemachineCamera playerCamera;

    [SerializeField]
    private CinemachineCamera objectiveCamera;

    [Header("Personaje ECHO")]
    [SerializeField]
    private Transform player;

    [Header("Configuración del enfoque")]
    [SerializeField]
    private float focusDuration = 3f;

    [SerializeField]
    private int playerPriority = 10;

    [SerializeField]
    private int objectivePriority = 20;

    [Header("Posición automática de la cámara guía")]
    [SerializeField]
    private float cameraDistance = 2.5f;

    [SerializeField]
    private float cameraHeight = 1.2f;

    [Header("Puntos de cámara personalizados")]
    [SerializeField]
    private ObjectiveCameraPoint[] cameraPoints;

    [Header("Cinemática introductoria")]
    [SerializeField]
    private bool focusInitialTarget = false;

    private Transform currentTarget;

    private Coroutine focusCoroutine;

    private bool missionCompleted;

    private bool initialized;

    private void Start()
    {
        missionCompleted = false;
        initialized = false;

        // Al comenzar, la cámara principal pertenece a ECHO.
        RestorePlayerCamera();

        // La primera guía apunta a la consola científica.
        SetTarget(initialTarget, initialObjective);

        initialized = true;

        // La panorámica introductoria controla cuándo
        // enfocar la consola, salvo que se active esta opción.
        if (focusInitialTarget && initialTarget != null)
        {
            StartFocus();
        }
    }

    private void LateUpdate()
    {
        if (guideArrow == null)
            return;

        if (currentTarget == null || missionCompleted)
        {
            if (guideArrow.activeSelf)
            {
                guideArrow.SetActive(false);
            }

            return;
        }

        if (!guideArrow.activeSelf)
        {
            guideArrow.SetActive(true);
        }

        // NUEVO:
        // Movimiento vertical suave de la guía.
        float verticalFloat =
            Mathf.Sin(Time.time * floatSpeed) *
            floatAmplitude;

        // Guía encima del objetivo actual + flotación.
        guideArrow.transform.position =
            currentTarget.position +
            arrowOffset +
            Vector3.up * verticalFloat;

        // Rotación visual de la guía.
        guideArrow.transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }

    public void SetTarget(Transform target, string message)
    {
        if (missionCompleted)
            return;

        // Cancelar cualquier enfoque que esté en curso.
        StopCurrentFocus();

        // Devolver la cámara a ECHO antes de cambiar de objetivo.
        RestorePlayerCamera();

        currentTarget = target;

        if (objectiveText != null)
        {
            objectiveText.text = message;
        }

        Debug.Log(
            "ECHO-07 Objetivo: " + message
        );

        if (currentTarget == null)
        {
            if (guideArrow != null)
            {
                guideArrow.SetActive(false);
            }

            return;
        }

        if (guideArrow != null)
        {
            guideArrow.transform.position =
                currentTarget.position + arrowOffset;

            guideArrow.SetActive(true);
        }

        // El objetivo inicial espera a la cinemática.
        // Los siguientes se enfocan automáticamente.
        if (initialized)
        {
            StartFocus();
        }
    }

    // IntroObjectiveFocus llama este método
    // cuando termina la cinemática introductoria.
    public void FocusCurrentTarget()
    {
        if (missionCompleted || currentTarget == null)
            return;

        StartFocus();
    }

    private void StartFocus()
    {
        if (objectiveCamera == null || playerCamera == null)
        {
            Debug.LogWarning(
                "ECHO-07: Faltan asignar las cámaras Cinemachine."
            );

            return;
        }

        if (currentTarget == null || missionCompleted)
            return;

        StopCurrentFocus();

        // Colocar la cámara ANTES de darle prioridad.
        ConfigureObjectiveCamera();

        focusCoroutine = StartCoroutine(
            FocusObjectiveSequence()
        );
    }

    private void ConfigureObjectiveCamera()
    {
        if (objectiveCamera == null || currentTarget == null)
            return;

        Transform customPoint =
            FindCameraPoint(currentTarget);

        Vector3 cameraPosition;

        if (customPoint != null)
        {
            // Núcleos con posición de cámara personalizada.
            cameraPosition = customPoint.position;

            Debug.Log(
                "ECHO-07: Cámara personalizada para "
                + currentTarget.name
                + " desde "
                + customPoint.name
                + ". Posición: "
                + cameraPosition
            );
        }
        else
        {
            // Para objetivos sin CameraPoint, colocar la
            // cámara del lado donde se encuentra ECHO.
            Vector3 direction = Vector3.back;

            if (player != null)
            {
                direction =
                    player.position - currentTarget.position;

                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    direction.Normalize();
                }
                else
                {
                    direction = Vector3.back;
                }
            }

            cameraPosition =
                currentTarget.position
                + direction * cameraDistance
                + Vector3.up * cameraHeight;

            Debug.Log(
                "ECHO-07: Cámara automática para "
                + currentTarget.name
                + ". Posición: "
                + cameraPosition
            );
        }

        // IMPORTANTE:
        // CM_ObjectiveGuide debe tener
        // Position Control = None.
        //
        // Así, Cinemachine no reemplazará esta posición
        // por el Follow Offset.
        objectiveCamera.transform.position =
            cameraPosition;

        // Orientación inicial hacia el objetivo.
        Vector3 directionToTarget =
            currentTarget.position - cameraPosition;

        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            objectiveCamera.transform.rotation =
                Quaternion.LookRotation(
                    directionToTarget.normalized,
                    Vector3.up
                );
        }

        // Hard Look At utilizará este objetivo
        // para mantenerlo dentro de la imagen.
        objectiveCamera.Follow = currentTarget;
    }

    private Transform FindCameraPoint(Transform target)
    {
        if (cameraPoints == null || target == null)
            return null;

        foreach (ObjectiveCameraPoint point in cameraPoints)
        {
            if (point == null)
                continue;

            if (point.target == target)
            {
                if (point.cameraPoint == null)
                {
                    Debug.LogWarning(
                        "ECHO-07: "
                        + target.name
                        + " tiene una entrada de cámara "
                        + "sin Camera Point asignado."
                    );
                }

                return point.cameraPoint;
            }
        }

        return null;
    }

    private IEnumerator FocusObjectiveSequence()
    {
        // Guardar el objetivo que se está enfocando.
        Transform focusedTarget = currentTarget;

        Debug.Log(
            "ECHO-07: Enfocando el siguiente objetivo: "
            + focusedTarget.name
        );

        playerCamera.Priority = playerPriority;

        // La cámara guía toma el control.
        objectiveCamera.Priority = objectivePriority;

        yield return new WaitForSeconds(
            Mathf.Max(0.1f, focusDuration)
        );

        // Regresar a ECHO.
        RestorePlayerCamera();

        focusCoroutine = null;

        Debug.Log(
            "ECHO-07: Cámara devuelta al personaje."
        );
    }

    private void RestorePlayerCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.Priority = playerPriority;
        }

        if (objectiveCamera != null)
        {
            objectiveCamera.Priority = 0;
        }
    }

    private void StopCurrentFocus()
    {
        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
            focusCoroutine = null;
        }
    }

    public void ClearTarget(string message = "")
    {
        currentTarget = null;

        if (objectiveText != null)
        {
            objectiveText.text = message;
        }

        StopCurrentFocus();

        RestorePlayerCamera();

        if (guideArrow != null)
        {
            guideArrow.SetActive(false);
        }
    }

    public void CompleteMission(string message)
    {
        if (missionCompleted)
            return;

        missionCompleted = true;

        ClearTarget(message);

        Debug.Log(
            "ECHO-07: " + message
        );
    }

    private void OnDisable()
    {
        StopCurrentFocus();

        RestorePlayerCamera();
    }
}