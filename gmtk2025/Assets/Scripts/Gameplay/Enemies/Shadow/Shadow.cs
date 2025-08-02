using UnityEngine;

public class SimpleShadow2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) player = GameObject.Find("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Poruszaj się w kierunku gracza jeśli jest wystarczająco daleko
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Shadow doszło do kolizji");
        if (other.CompareTag("Player") && GameManager.instance)
        {
            GameManager.instance.StumbledOnEnemy();
        }
    }
}