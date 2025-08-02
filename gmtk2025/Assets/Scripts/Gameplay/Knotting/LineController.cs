using UnityEngine;

public class LineController : MonoBehaviour
{
    public Transform hairNodesParent;
    public GameObject hairNodePickable;
    private LineRenderer lr;
    private Transform[] points;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = HairNodePickable.nodeCount + 1;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach(Transform child in hairNodesParent) {
            lr.SetPosition(i, child.position);
            i++;
        }
    }
}
