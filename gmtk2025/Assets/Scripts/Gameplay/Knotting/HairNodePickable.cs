using UnityEngine;

public class HairNodePickable : MonoBehaviour
{
    public enum DragState{
		None,
		Fresh,
		ForceEnd
	}
	
	public DragState dragState;
	
	void Start(){
		dragState = DragState.None;
	}

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("hit!");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null){
                dragState = DragState.Fresh;
                Debug.Log("hit?");
            }
            else {
                dragState = DragState.None;
            }
        }
        //AudioManager.instance.PlaySFX("button_click");
        if(dragState == DragState.Fresh){
            //hand.gameObject.SetActive(true);
            this.transform.position = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        }
    }
}
