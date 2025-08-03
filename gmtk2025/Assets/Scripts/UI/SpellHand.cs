using UnityEngine;

public class SpellHand : MonoBehaviour
{
    public BoxCollider2D spell_0;
    public BoxCollider2D spell_1;
    public BoxCollider2D spell_2;
    public BoxCollider2D spell_3;
    public BoxCollider2D spell_4;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        Vector2 worldPoint  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPoint3 = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if(hit.collider != null){
            Debug.Log(hit.transform.GetSiblingIndex());
        }
    }
}
