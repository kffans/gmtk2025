using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera playerCamera;

    public Rigidbody2D rb;
    
    private Vector2 moveDirection;

    void Update()
    {
        ProcessInputs();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY);
    }

    void Move()
    {
        // player movement
        rb.linearVelocity = new Vector2(moveDirection.x*moveSpeed, moveDirection.y*moveSpeed);

        // camera movement
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
    }

    void FixedUpdate()
    {
        Move();
    }
}