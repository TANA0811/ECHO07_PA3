using UnityEngine;

public class ReactorSuspenseTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioSource reactorSuspense;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player"))
            return;

        activated = true;

        if (reactorSuspense != null && !reactorSuspense.isPlaying)
        {
            reactorSuspense.Play();
            Debug.Log("ECHO-07: Música de suspenso del reactor activada.");
        }
    }
}