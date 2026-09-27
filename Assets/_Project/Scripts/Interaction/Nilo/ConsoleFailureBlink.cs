using UnityEngine;

public class ConsoleFailureBlink : MonoBehaviour
{
    [Header("Pantalla de la consola")]
    [SerializeField] private Renderer screenRenderer;

    [Header("Configuración del parpadeo")]
    [SerializeField] private Color failureColor = Color.red;

    [SerializeField] private float intensity = 3f;

    [SerializeField] private float blinkSpeed = 0.3f;

    [Header("Color de la consola reparada")]
    [SerializeField] private Color repairedColor =
        new Color(0f, 1f, 1f);

    [SerializeField] private float repairedIntensity = 4f;

    private Material screenMaterial;

    private float timer = 0f;

    private bool lightOn = true;

    private bool isRepaired = false;

    private void Start()
    {
        // Buscar automáticamente el Renderer.
        if (screenRenderer == null)
        {
            screenRenderer = GetComponent<Renderer>();
        }

        // Verificar que exista la pantalla.
        if (screenRenderer == null)
        {
            Debug.LogWarning(
                "ConsoleFailureBlink: No se encontró el Renderer de la pantalla."
            );

            enabled = false;
            return;
        }

        // Crear una instancia del material de la pantalla.
        screenMaterial = screenRenderer.material;

        // Activar emisión.
        screenMaterial.EnableKeyword("_EMISSION");

        // Comenzar con la pantalla roja.
        screenMaterial.SetColor(
            "_BaseColor",
            failureColor
        );

        screenMaterial.SetColor(
            "_EmissionColor",
            failureColor * intensity
        );

        Debug.Log(
            "ECHO-07: Falla eléctrica de consola activada."
        );
    }

    private void Update()
    {
        // No parpadear si la consola ya fue reparada.
        if (isRepaired)
            return;

        if (screenMaterial == null)
            return;

        timer += Time.deltaTime;

        if (timer >= blinkSpeed)
        {
            timer = 0f;

            lightOn = !lightOn;

            if (lightOn)
            {
                // PANTALLA ENCENDIDA: ROJO BRILLANTE.
                screenMaterial.SetColor(
                    "_BaseColor",
                    failureColor
                );

                screenMaterial.SetColor(
                    "_EmissionColor",
                    failureColor * intensity
                );
            }
            else
            {
                // PANTALLA APAGADA: NEGRO.
                screenMaterial.SetColor(
                    "_BaseColor",
                    Color.black
                );

                screenMaterial.SetColor(
                    "_EmissionColor",
                    Color.black
                );
            }
        }
    }

    // Se ejecuta cuando Echo restaura el reactor.
    public void RepairConsole()
    {
        // Evitar ejecutar la reparación varias veces.
        if (isRepaired)
            return;

        isRepaired = true;

        // Detener definitivamente el parpadeo.
        enabled = false;

        if (screenMaterial == null)
        {
            Debug.LogWarning(
                "ECHO-07: No se pudo cambiar el color de la consola."
            );

            return;
        }

        // Activar la emisión cyan estable.
        screenMaterial.EnableKeyword("_EMISSION");

        screenMaterial.SetColor(
            "_BaseColor",
            repairedColor
        );

        screenMaterial.SetColor(
            "_EmissionColor",
            repairedColor * repairedIntensity
        );

        Debug.Log(
            "ECHO-07: Pantalla de consola reparada. Cyan estable."
        );
    }

    private void OnDestroy()
    {
        if (screenMaterial != null)
        {
            Destroy(screenMaterial);
        }
    }
}