using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -10);
    [SerializeField] private float cameraSmoothSpeed = 0.125f;
    [SerializeField] private float cameraAngle = 45f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float rotationOffset = -90f; // Domyślnie dla sprite'ów skierowanych w górę

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 mouseWorldPosition;

    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;
    private bool wasMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if(playerCamera != null)
        {
            playerCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
            playerCamera.transform.position = transform.position + cameraOffset;
        }
    }

    void Update()
    {
        ProcessInputs();
        CalculateMousePosition();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotateTowardsMouse();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void CalculateMousePosition()
    {
        // gówno
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -playerCamera.transform.position.z; 
        mouseWorldPosition = playerCamera.ScreenToWorldPoint(mousePos);
    }

    void RotateTowardsMouse()
    {
        Vector2 lookDirection = mouseWorldPosition - rb.position;
        
        if (lookDirection.sqrMagnitude > 0.01f) // Unikamy obrotu gdy mysz jest blisko środka
        {
            float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg + rotationOffset;
            float currentAngle = rb.rotation;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            
            rb.MoveRotation(newAngle); // Lepsze dla fizyki niż bezpośrednie ustawianie rotation
        }
    }

    void MovePlayer()
    {
        // Ruch postaci
        rb.linearVelocity = moveDirection * moveSpeed;

        // Kamera podążająca
        if(playerCamera != null)
        {
            Vector3 desiredPosition = transform.position + cameraOffset;
            Vector3 smoothedPosition = Vector3.Lerp(playerCamera.transform.position, desiredPosition, cameraSmoothSpeed);
            playerCamera.transform.position = smoothedPosition;
        }

    }

    void HandleFootstepSounds()
    {
        bool isMoving = moveDirection.magnitude > 0.1f;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            
            if (footstepTimer <= 0f)
            {
                AudioManager.instance.PlaySFX("walking-stone");
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = footstepInterval; // Reset timer when not moving
        }

        wasMoving = isMoving;
    }
}