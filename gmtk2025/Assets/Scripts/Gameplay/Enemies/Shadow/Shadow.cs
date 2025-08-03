using UnityEngine;
using UnityEngine.SceneManagement;

public class Shadow : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;
    public float chaseDistance = 8f; 
    private Transform player;
    private bool hasDetectedPlayer = false; 

    void Start()
    {
        ResetState();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetState();
    }

    private void ResetState()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) player = GameObject.Find("Player")?.transform;
        hasDetectedPlayer = false;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!hasDetectedPlayer && distanceToPlayer <= chaseDistance)
        {
            hasDetectedPlayer = true;
        }

        if (hasDetectedPlayer && distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Shadow doszło do kolizji");
        if (other.gameObject.CompareTag("Player") && GameManager.instance)
        {
            GameManager.instance.StumbledOnEnemy();
        }
    }
}