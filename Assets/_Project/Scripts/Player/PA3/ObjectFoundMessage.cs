using UnityEngine;

public class ObjectFoundMessage : MonoBehaviour
{
    public Transform player;
    public GameObject objectFoundText;

    public float detectionDistance = 2f;

    void Start()
    {
        objectFoundText.SetActive(false);
    }

    void Update()
    {
        if (player == null || objectFoundText == null)
            return;

        float distance = Vector2.Distance(
            new Vector2(player.position.x, player.position.z),
            new Vector2(transform.position.x, transform.position.z)
        );

        if (distance <= detectionDistance)
        {
            objectFoundText.SetActive(true);
        }
        else
        {
            objectFoundText.SetActive(false);
        }
    }
}