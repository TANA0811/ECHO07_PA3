using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Salto")]
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError(
                "PlayerMovement3D necesita un CharacterController en el mismo GameObject."
            );
        }

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogWarning(
                "No se encontró una cámara con el Tag 'MainCamera'. " +
                "El movimiento utilizará los ejes del mundo."
            );
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (controller == null)
            return;

        // --------------------------------------------------
        // 1. LEER INPUT
        // --------------------------------------------------

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;

            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;
        }

        // Evita que la diagonal sea más rápida
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        // --------------------------------------------------
        // 2. MOVIMIENTO RELATIVO A LA CÁMARA
        // --------------------------------------------------

        Vector3 move;

        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Eliminamos la inclinación vertical de la cámara
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            move = cameraForward * input.y
                 + cameraRight * input.x;
        }
        else
        {
            // Respaldo si no existe Main Camera
            move = new Vector3(input.x, 0f, input.y);
        }

        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        // --------------------------------------------------
        // 3. ROTACIÓN DEL PERSONAJE
        // --------------------------------------------------

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // --------------------------------------------------
        // 4. GRAVEDAD
        // --------------------------------------------------

        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        // --------------------------------------------------
        // 5. SALTO
        // --------------------------------------------------

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            verticalVelocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --------------------------------------------------
        // 6. MOVIMIENTO FINAL
        // --------------------------------------------------

        Vector3 finalMovement =
            move * moveSpeed + verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }
}