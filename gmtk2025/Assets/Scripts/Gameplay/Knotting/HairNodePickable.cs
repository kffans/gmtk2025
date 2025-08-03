using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class HairNodePickable : MonoBehaviour {
    private bool dragging = false;
    private CircleCollider2D circleCollider2D;
    public GameObject hairNodePrefab;
    public MeshCollider lineRendererMeshCollider;
    public LineRenderer lr;
    private Transform firstPrevNode;
    private Transform secondPrevNode;
    public static float angleThreshold = 50.0f;
    public static float nodeDistance   = 10.0f;
    public static int   nodeCount      = 250;
    public static int   nodeLastNumber = nodeCount;
    public static List<Tuple<int, bool>> knottingOrder = new List<Tuple<int, bool>>();
    public Camera camera;
    public GameObject handPointing;
    public GameObject handGrabbingLower;
    public GameObject handGrabbingUpper;
    public GameObject currentHand;
    
    public GameObject leftHand;
    
    
    private float zDepth     = 489.31f;
    private float zUpDepth   = 500.0f;
    private float zDownDepth = 489.31f;
    
    
    public static float zDepthChange = 0.001f;
    private bool isInsideCollider = false;
    private bool isGoingDown = false;
    
    private bool isKnotDone = false;
    private float finishingTime = 1.8f;
    private float currentFinishTime = 0.0f;
     
    
    void Awake() {
        Cursor.visible = false;
        currentHand = handPointing;
        circleCollider2D = this.GetComponent<CircleCollider2D>();
        firstPrevNode = this.transform.parent.transform.GetChild(1);
        secondPrevNode = this.transform.parent.transform.GetChild(2);
        /*this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, zDepth);
        
        for (int i = 1; i <= nodeCount; i++) {            
            GameObject hairNode = Instantiate(hairNodePrefab, this.transform.parent);
            hairNode.name = "HairNode_" + i.ToString();
            hairNode.transform.position = this.transform.position;
        }
        firstPrevNode = GameObject.Find("HairNode_1").transform;
        secondPrevNode = GameObject.Find("HairNode_2").transform;
        secondPrevNode.position += Vector3.left;*/
    }

    void FixedUpdate() {
        Vector2 worldPoint  = camera.ScreenToWorldPoint(Input.mousePosition);
        
        float threeWayAngle = GetAngle(secondPrevNode.position, firstPrevNode.position, worldPoint);
        
        if (dragging) {
            if(180.0f - threeWayAngle < angleThreshold){
                Mesh mesh = new Mesh();
                lr.BakeMesh(mesh, true);
                lineRendererMeshCollider.sharedMesh = mesh;
            }
        }
    }
    
    void Update() {
        Vector2 worldPoint  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPoint3 = new Vector3(worldPoint.x, worldPoint.y, zDepth);
        
        if(isKnotDone) {
            if(currentFinishTime > finishingTime){
                GameManager.instance.mainCamera.SetActive(true);
                GameManager.instance.gameCanvas.SetActive(true);
                GameManager.instance.inKnottingView = false;
                Cursor.visible = true;
                Destroy(GameObject.Find("hair"));
            }
            else{
                currentFinishTime += Time.deltaTime;
            }
        }
        
        
        if(!isKnotDone){
            if (Input.GetKeyDown(KeyCode.Q)){
                isKnotDone = true;
                Tuple<int, bool, bool> knotResult = SolveKnot();

                int indexKey = 0;
                for(int i = 0; i < 5; i++){
                    if(!GameManager.instance.knotInventory.ContainsKey(i)){
                        indexKey = i;
                        break;
                    }
                }
                
                GameManager.Effect effect = GameManager.Effect.None;
                if     (knotResult.Item1 == 3){ // electricity
                    effect = GameManager.Effect.Electricity;
                }
                else if(knotResult.Item1 == 4){ // ice
                    effect = GameManager.Effect.Ice;
                }
                else if(knotResult.Item1 == 5){ // speed
                    effect = GameManager.Effect.Speed;
                }
                else if(knotResult.Item1 == 6){ // fire
                    effect = GameManager.Effect.Fire;
                }
                else if(knotResult.Item1 >= 7){ // invisibility
                    effect = GameManager.Effect.Invisibility;
                    
                }
                GameManager.instance.knotInventory[indexKey] = new Tuple<GameManager.Effect, bool, bool>(effect, knotResult.Item2, knotResult.Item3);
                //GameObject.Find("HairNode_" + nodeLastNumber.ToString()).transform.position
                Vector3 dest = GameObject.Find("HairNode_" + nodeLastNumber.ToString()).transform.position;
                dest = new Vector3(dest.x, dest.y, 0.0f);
                LeanTween.move(leftHand, dest, 1.7f).setEase(LeanTweenType.easeOutQuart);
                
                return;
                
            }
            
            
            if(!Input.GetMouseButton(0)){
                currentHand = handPointing;
                handPointing.SetActive(true);
                handGrabbingLower.SetActive(false);
                handGrabbingUpper.SetActive(false);
            }
            else{
                if(Input.GetKey(KeyCode.Space)){
                    currentHand = handGrabbingLower;
                    handPointing.SetActive(false);
                    handGrabbingLower.SetActive(true);
                    handGrabbingUpper.SetActive(false);
                }
                else{
                    currentHand = handGrabbingUpper;
                    handPointing.SetActive(false);
                    handGrabbingLower.SetActive(false);
                    handGrabbingUpper.SetActive(true);
                }
            }
            currentHand.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

            
            
            if(!isInsideCollider) {
                isGoingDown = false;
                if (Input.GetKey(KeyCode.Space)) { isGoingDown = true;  zUpDepth   += zDepthChange; zDepth = zUpDepth; }
                else                             { isGoingDown = false; zDownDepth -= zDepthChange; zDepth = zDownDepth; }
            }
            else {
                if(isGoingDown) { zUpDepth   += zDepthChange; zDepth = zUpDepth; }
                else            { zDownDepth -= zDepthChange; zDepth = zDownDepth; }
            }
            

            
            float threeWayAngle = GetAngle(secondPrevNode.position, firstPrevNode.position, worldPoint);
            float draggedNodeDistance  = Mathf.Sqrt(Mathf.Pow(firstPrevNode.position.x - worldPoint.x,2) + Mathf.Pow(firstPrevNode.position.y - worldPoint.y, 2));

            if (Input.GetMouseButton(0)) {
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if(hit.collider != null){
                    if (hit.collider == circleCollider2D) {
                        dragging = true;
                    }
                }
            }
            else {
                dragging = false;
            }
            
            
            if (dragging) {
                if(180.0f - threeWayAngle < angleThreshold){
                    transform.position = worldPoint3;

                    Vector2 v1 = new Vector2(firstPrevNode.position.x, firstPrevNode.position.y);
                    Vector2 normalizedDirection = (worldPoint - v1).normalized;
                    if (draggedNodeDistance >= nodeDistance) {
                        Vector2 normalizedPos = (normalizedDirection * nodeDistance) + v1;

                        secondPrevNode = firstPrevNode;
                        GameObject lastNode = GameObject.Find("HairNode_" + nodeLastNumber.ToString());
                        //lastNode.name = "HairNode_"
                        lastNode.transform.SetSiblingIndex(1);
                        nodeLastNumber--;
                        if(nodeLastNumber == 0) {
                            nodeLastNumber = nodeCount;
                        }
                        
                        lastNode.transform.position = new Vector3(normalizedPos.x, normalizedPos.y, zDepth);
                        firstPrevNode = lastNode.transform;

                        knottingOrder.RemoveAll(pair => pair.Item1 == nodeLastNumber);
                    }
                    Vector2 raycastVector = (normalizedDirection * (nodeDistance + 0.5f)/2.0f) + worldPoint;
                    Vector3 raycastVector3 = new Vector3(raycastVector.x, raycastVector.y, 0.0f);
                    
                    RaycastHit hit;
                    if (Physics.Raycast(raycastVector3, Vector3.forward, out hit)){
                        if (hit.triangleIndex > 4) {
                            if (!isInsideCollider) {
                                isInsideCollider = true;
                                int nodeHitIndex = nodeLastNumber + (hit.triangleIndex / 2);
                                if (nodeHitIndex > nodeCount) {
                                    nodeHitIndex = nodeHitIndex - nodeCount;
                                }
                                Debug.Log("Hit!   " + (nodeHitIndex) + "    " + hit.point.z + "    " + transform.position.z);
                                bool isGoingUnder = false;
                                if (transform.position.z > hit.point.z){
                                    isGoingUnder = true;
                                }
                                
                                knottingOrder.Add(new Tuple<int, bool>(nodeHitIndex, isGoingUnder));
                                
                                
                                
                                
                                string items = "";
                                foreach (Tuple<int, bool> item in knottingOrder){
                                    items += item.Item1.ToString() + "," + item.Item2.ToString() + "\n";
                                }
                                Debug.Log(items);
                            }
                        }
                    }
                    else {
                        if (isInsideCollider) {
                            isInsideCollider = false;
                            Debug.Log("Out of hit");
                        }
                    }

                    
                }
                else {
                    dragging = false;
                }
            }
        }
    }
    
    public static Tuple<int, bool, bool> SolveKnot() {
        List<Tuple<int, bool>> knottingOrderTemp = new List<Tuple<int, bool>>();
        int knotCrossingCount = 1;
        bool isImpure = false;
        bool isOrdered = true;
        
        
        foreach (Tuple<int, bool> item in knottingOrder){
            int index = GameObject.Find("HairNode_" + item.Item1.ToString()).transform.GetSiblingIndex();
            knottingOrderTemp.Add(new Tuple<int, bool>(index, item.Item2));
        } // ^ or you could just shift all the numbers by the current offset... whatever
        
        
        // {(24,false), (97,true), (53, false)} -> {(1,false), (3,true), (2, false)}
        var sort           = knottingOrderTemp.Select(t => t.Item1).Distinct().OrderBy(x => x).ToList();
        var map            = sort.Select((val, index) => new { val, weight = index + 1 }).ToDictionary(x => x.val, x => x.weight);
        var sortedKnotting = knottingOrderTemp.Select(t => Tuple.Create(map[t.Item1], t.Item2)).ToList();
        
        int crossCount = sortedKnotting.Count;
        if(crossCount >= 1){
            
            
            do{
                crossCount = sortedKnotting.Count;
                for (int i = 0; i < sortedKnotting.Count - 1; i++){
                    if(Math.Abs(sortedKnotting[i].Item1 - sortedKnotting[i + 1].Item1) == 1 && sortedKnotting[i].Item2 == sortedKnotting[i + 1].Item2){
                        sortedKnotting.RemoveAt(i + 1);
                        sortedKnotting.RemoveAt(i);
                    }
                }
            } while(crossCount != sortedKnotting.Count);
            
            // check if loose at beginning
            // if at the beginning it increases each by one, or is lower than next
            crossCount = sortedKnotting.Count;
            if(crossCount >= 1){
                while(true){
                    crossCount = sortedKnotting.Count;
                    if(crossCount >= 2){
                        if(sortedKnotting[1].Item1 - sortedKnotting[0].Item1 == 1){
                            sortedKnotting.RemoveAt(1);
                            sortedKnotting.RemoveAt(0);
                        }
                        else if(sortedKnotting[1].Item1 > sortedKnotting[0].Item1) {
                            sortedKnotting.RemoveAt(0);
                        }
                        else{
                            break;
                        }
                    }
                    else{
                        break;
                    }
                }
            }
            
            
            
            // check if numbers are bigger than the first
            crossCount = sortedKnotting.Count;
            if(crossCount >= 1){
                int firstCrossingNumber = sortedKnotting[0].Item1;
                for (int i = 1; i < sortedKnotting.Count; i++){
                    if(sortedKnotting[i].Item1 > firstCrossingNumber){
                        sortedKnotting.RemoveAt(i);
                        i--;
                    }
                }
            }
            
            // if increasing, czeck if theres at least one decreasing pair before
            crossCount = sortedKnotting.Count;
            if(crossCount >= 1){
                int firstCrossingNumber = sortedKnotting[0].Item1;
            }
            
            
            
            crossCount = sortedKnotting.Count;
            if(crossCount >= 1){
                bool isCrossingUnder = sortedKnotting[0].Item2;
                for (int i = 1; i < crossCount; i++){
                    if(sortedKnotting[i - 1].Item1 - sortedKnotting[i].Item1 != 1){
                        isOrdered = false;
                    }
                    if(isCrossingUnder == !sortedKnotting[i].Item2){
                        isCrossingUnder = !isCrossingUnder;
                        knotCrossingCount++;
                    }
                }
                if(knotCrossingCount != crossCount){
                    isImpure = true;
                }
            }
        }
        
        
        string items = "";
        foreach (Tuple<int, bool> item in knottingOrderTemp){
            items += item.Item1.ToString() + "," + item.Item2.ToString() + "\n";
        }
        items += "\nknotting:\n";
        foreach (Tuple<int, bool> item in sortedKnotting){
            items += item.Item1.ToString() + "," + item.Item2.ToString() + "\n";
        }
        Debug.Log(items + "  " + knotCrossingCount + " " + isImpure + " " + isOrdered);
        
        return new Tuple<int, bool, bool>(knotCrossingCount, isImpure, isOrdered);
    }
    
    public static float GetAngle(Vector2 vec1, Vector2 vec2, Vector2 vec3) {
        float lengthA = Mathf.Sqrt(Mathf.Pow(vec2.x - vec1.x,2) + Mathf.Pow(vec2.y - vec1.y, 2));
        float lengthB = Mathf.Sqrt(Mathf.Pow(vec3.x - vec2.x,2) + Mathf.Pow(vec3.y - vec2.y, 2));
        float lengthC = Mathf.Sqrt(Mathf.Pow(vec3.x - vec1.x,2) + Mathf.Pow(vec3.y - vec1.y, 2));

        float calc = ((lengthA * lengthA) + (lengthB * lengthB) - (lengthC * lengthC)) / (2 * lengthA * lengthB);

        return Mathf.Acos(calc) * Mathf.Rad2Deg;

    }
}
