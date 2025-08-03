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
    [SerializeField] private float rotationOffset = -90f;
    [SerializeField] private Animator _animator;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 lastNonZeroDirection = Vector2.up; // Domyślnie w górę
    
    private bool upPressed, downPressed, leftPressed, rightPressed;

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
        UpdateAnimations();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void ProcessInputs()
    {
        upPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        downPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        float moveX = 0f;
        float moveY = 0f;

        if (upPressed) moveY += 1f;
        if (downPressed) moveY -= 1f;
        if (leftPressed) moveX -= 1f;
        if (rightPressed) moveX += 1f;

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection != Vector2.zero)
        {
            lastNonZeroDirection = moveDirection;
        }
    }

    void UpdateAnimations()
    {
        // Resetuj wszystkie parametry animacji
        ResetAllAnimationParameters();

        if (moveDirection == Vector2.zero)
        {
            return;
        }

        if (upPressed)
        {
            _animator.SetBool("isWalkingUp", true);
        }
        else if (downPressed)
        {
            _animator.SetBool("isWalkingDown", true);
        }
        else if (leftPressed)
        {
            _animator.SetBool("isWalkingLeft", true);
        }
        else if (rightPressed)
        {
            _animator.SetBool("isWalkingRight", true);
        }
    }

    void ResetAllAnimationParameters()
    {
        _animator.SetBool("isWalkingUp", false);
        _animator.SetBool("isWalkingDown", false);
        _animator.SetBool("isWalkingLeft", false);
        _animator.SetBool("isWalkingRight", false);
    }

    void MovePlayer()
    {
        rb.linearVelocity = moveDirection * moveSpeed;

        // Kamera podążająca
        if (playerCamera != null)
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
            footstepTimer = footstepInterval;
        }

        wasMoving = isMoving;
    }
}