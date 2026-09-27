using UnityEngine;
using System.Collections;

public class EnergyCore3D : MonoBehaviour
{
    [Header("Efecto de recolección")]
    [SerializeField] private ParticleSystem pickupParticles;

    [Header("Mensaje de recolección")]
    [SerializeField] private GameObject collectedMessage;

    [Header("Duración del mensaje")]
    [SerializeField] private float messageDuration = 2f;

    [Header("Secuencia energética")]
    [SerializeField] private EnergyCoreSequenceManager sequenceManager;

    private bool collected = false;

    private void Start()
    {
        // El mensaje no debe estar visible al comenzar.
        if (collectedMessage != null)
        {
            collectedMessage.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Evitar recolecciones repetidas.
        if (collected)
            return;

        // Solo ECHO puede recoger el núcleo.
        if (!other.CompareTag("Player"))
            return;

        collected = true;

        Debug.Log("¡Energy Core recogido!");

        // =====================================================
        // 1. MOSTRAR MENSAJE DE RECOLECCIÓN
        // =====================================================

        if (collectedMessage != null)
        {
            collectedMessage.SetActive(true);
        }

        // =====================================================
        // 2. DESACTIVAR EL COLLIDER
        // =====================================================

        Collider myCollider = GetComponent<Collider>();

        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // =====================================================
        // 3. REPRODUCIR PARTÍCULAS
        // =====================================================

        if (pickupParticles != null)
        {
            pickupParticles.transform.position = transform.position;

            // Separar las partículas del núcleo para que
            // sigan reproduciéndose al desactivarlo.
            pickupParticles.transform.SetParent(null);

            pickupParticles.gameObject.SetActive(true);

            pickupParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            pickupParticles.Clear();

            pickupParticles.Play();

            Debug.Log("Partículas de recolección reproducidas.");
        }
        else
        {
            Debug.LogWarning(
                "ECHO-07: PickupParticles no está asignado."
            );
        }

        // =====================================================
        // 4. OCULTAR VISUALMENTE EL ENERGY CORE
        // =====================================================

        Renderer myRenderer = GetComponent<Renderer>();

        if (myRenderer != null)
        {
            myRenderer.enabled = false;
        }

        // =====================================================
        // 5. INFORMAR AL ADMINISTRADOR
        // =====================================================

        if (sequenceManager != null)
        {
            sequenceManager.RegisterCollection(this);
        }
        else
        {
            Debug.LogWarning(
                "ECHO-07: Sequence Manager no está asignado en " +
                gameObject.name
            );
        }

        // =====================================================
        // 6. OCULTAR MENSAJE DESPUÉS DE 2 SEGUNDOS
        // =====================================================

        StartCoroutine(FinishPickup());
    }

    private IEnumerator FinishPickup()
    {
        yield return new WaitForSeconds(messageDuration);

        if (collectedMessage != null)
        {
            collectedMessage.SetActive(false);
        }

        Debug.Log("ECHO-07: Mensaje de recolección ocultado.");

        // Desactivar únicamente el núcleo recogido.
        gameObject.SetActive(false);
    }
}