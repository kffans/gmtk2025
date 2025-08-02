using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelPortal : MonoBehaviour
{
    public GameManager.PortalType portalType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (GameManager.instance == null)
        {
            Debug.LogError("Brak GameManager!", this);
            return;
        }
        
        GameManager.instance.OnPortalEntered(portalType);
    }
}