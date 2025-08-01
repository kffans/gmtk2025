using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -10);
    [SerializeField] private float cameraSmoothSpeed = 0.125f;
    [SerializeField] private float cameraAngle = 45f;
    [SerializeField] private float rotationSpeed = 10f;

    public Rigidbody2D rb;
    
    private Vector2 moveDirection;
    private Vector2 mousePosition;

    void Start()
    {
        if(playerCamera != null)
        {
            playerCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
            playerCamera.transform.position = transform.position + cameraOffset;
        }
    }

    void Update()
    {
        ProcessInputs();
        RotateTowardsMouse();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void RotateTowardsMouse()
    {
        Vector2 lookDirection = mousePosition - rb.position;
        float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        
        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
        rb.rotation = angle;
    }

    void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed;

        if(playerCamera != null)
        {
            Vector3 desiredPosition = transform.position + cameraOffset;
            Vector3 smoothedPosition = Vector3.Lerp(playerCamera.transform.position, desiredPosition, cameraSmoothSpeed);
            playerCamera.transform.position = smoothedPosition;
        }
    }

    void FixedUpdate()
    {
        Move();
    }
}