using UnityEngine;

public class ReactorFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 2.5f;
    [SerializeField] private float flickerSpeed = 8f;

    private void Reset()
    {
        targetLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        float noise = Mathf.PerlinNoise(
            Time.time * flickerSpeed,
            0f
        );

        targetLight.intensity =
            Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}