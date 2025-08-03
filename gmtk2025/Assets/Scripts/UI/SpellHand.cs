using UnityEngine;

public class SpellHand : MonoBehaviour
{
    public GameObject spell_00;
    public GameObject spell_11;
    public GameObject spell_22;
    public GameObject spell_33;
    public GameObject spell_44;
    public BoxCollider spell_0;
    public BoxCollider spell_1;
    public BoxCollider spell_2;
    public BoxCollider spell_3;
    public BoxCollider spell_4;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        
        if(!GameManager.instance.knotInventory.ContainsKey(0)){
            spell_00.SetActive(false);
        }
        if(!GameManager.instance.knotInventory.ContainsKey(1)){
            spell_11.SetActive(false);
        }
        if(!GameManager.instance.knotInventory.ContainsKey(2)){
            spell_22.SetActive(false);
        }
        if(!GameManager.instance.knotInventory.ContainsKey(3)){
            spell_33.SetActive(false);
        }
        if(!GameManager.instance.knotInventory.ContainsKey(4)){
            spell_44.SetActive(false);
        }
        
        if(GameManager.instance.knotInventory.ContainsKey(0)){
            spell_00.SetActive(true);
        }
        if(GameManager.instance.knotInventory.ContainsKey(1)){
            spell_11.SetActive(true);
        }
        if(GameManager.instance.knotInventory.ContainsKey(2)){
            spell_22.SetActive(true);
        }
        if(GameManager.instance.knotInventory.ContainsKey(3)){
            spell_33.SetActive(true);
        }
        if(GameManager.instance.knotInventory.ContainsKey(4)){
            spell_44.SetActive(true);
        }
        
        Vector2 worldPoint  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPoint3 = new Vector3(worldPoint.x, worldPoint.y, 100.0f);
        RaycastHit hit;
        if (Physics.Raycast(worldPoint3, Vector3.forward, out hit)){
            Debug.Log(hit.transform.GetSiblingIndex());
        }
    }
}
