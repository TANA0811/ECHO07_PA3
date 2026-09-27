using UnityEngine;
using UnityEngine.Playables;

public class IntroObjectiveFocus : MonoBehaviour
{
    [Header("Cinemática introductoria")]
    [SerializeField]
    private PlayableDirector introDirector;

    [Header("Sistema de objetivos")]
    [SerializeField]
    private ObjectiveGuideManager objectiveGuide;

    [Header("Audio de emergencia")]
    [SerializeField]
    private AudioSource emergencyAlarm;

    private bool introCompleted = false;

    private void OnEnable()
    {
        if (introDirector != null)
        {
            introDirector.stopped += OnIntroFinished;
        }
    }

    private void OnDisable()
    {
        if (introDirector != null)
        {
            introDirector.stopped -= OnIntroFinished;
        }
    }

    private void OnIntroFinished(PlayableDirector director)
    {
        if (introCompleted)
            return;

        introCompleted = true;

        Debug.Log(
            "ECHO-07: Cinemática introductoria finalizada."
        );

        // Activar alarma cuando termina la introducción.
        if (emergencyAlarm != null && !emergencyAlarm.isPlaying)
        {
            emergencyAlarm.Play();

            Debug.Log(
                "ECHO-07: Alarma de emergencia activada."
            );
        }

        if (objectiveGuide != null)
        {
            objectiveGuide.FocusCurrentTarget();
        }
    }
}