using UnityEngine;

[RequireComponent(typeof(Light))]
public class EmergencyLightBlink : MonoBehaviour
{
    [Header("Parpadeo de emergencia")]
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float blinkSpeed = 2.5f;

    private Light emergencyLight;

    private void Awake()
    {
        emergencyLight = GetComponent<Light>();
    }

    private void Update()
    {
        float pulse = Mathf.PingPong(
            Time.time * blinkSpeed,
            1f
        );

        emergencyLight.intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            pulse
        );
    }
}