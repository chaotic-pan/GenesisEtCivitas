using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Plane = UnityEngine.Plane;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private int rangeRadius = 5;
    
    private Tilemap map;
    private AnimManager npcAnim;
    private TileManager TM = TileManager.Instance;
    private MapExtractor ME = MapExtractor.Instance;

    private List<Vector3Int> range;
    private Stack<Vector3Int> path = new Stack<Vector3Int>();
    private Vector3 pathPoint;
    private Plane clickPlane;
    
    void Start()
    {
        map = TM.map;

        pathPoint = transform.position;
        npcAnim = transform.GetComponentInChildren<AnimManager>();
        
        //DEBUG
        clickPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
    }

    void Update()
    {
        //DEBUG Click on tile moves NPC there
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main is not null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (clickPlane.Raycast(ray, out float enter))
                {
                    var clickPos = ray.GetPoint(enter);
                    movetoTile(map.WorldToCell(clickPos));
                }
            }
        }

        var dist = Vector3.Distance(transform.position, pathPoint);

        if (dist > 0.05f)
        {
            var position = transform.position;
            Vector3 direction = (pathPoint - position).normalized;
            position += direction * (movementSpeed * Time.deltaTime);
            transform.position = position;
            //ME.heightMap[(int)position.x, (int)position.y]
            transform.rotation = Quaternion.LookRotation (direction);
            movementSpeed = 5f - (TM.getTileDataByWorldCoords(position).travelCost/2);
            movementSpeed = movementSpeed < 1 ? 1 : movementSpeed;
        }
        else
        {
            if (path.TryPop(out var pather))
            {
                pathPoint = map.CellToWorld(pather);
            }
            else
            {
                npcAnim.SetIsMoving(false);
            }
        }
    }

    public void calculateRange()
    {
        range = getSpecificRange(map.WorldToCell(transform.position), rangeRadius);
    }
    
    public List<Vector3Int> getSpecificRange(Vector3Int gridPos, int radius)
    {
        var newRange = new List<Vector3Int>();
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = Math.Max(-radius, -q-radius); r <= Math.Min(radius, -q+radius); r++)
            {
                var s = -q - r;
                var cubePos = TM.GridToCube(gridPos);
                newRange.Add(TM.CubeToGrid(cubePos.x + q, cubePos.y + r, gridPos.z + s));
            }
        }

        return newRange;
    }

    public void movetoTile(Vector3Int gridPos)
    {
        // get current pos
        var npcGridPos = map.WorldToCell(transform.position);
        //calculate path between current and target pos
        calculateRange();
        path = dijkstra(npcGridPos, gridPos, range);
        
        if (path.TryPop(out var pather))
        {
            // double pop bc the first path point is the current pos
            if (path.TryPop(out pather))
            {
                pathPoint = map.CellToWorld(pather);
                npcAnim.SetIsMoving(true);
            }  
        }
    }
    
    private Stack<Vector3Int> dijkstra(Vector3Int start, Vector3Int destination, List<Vector3Int> SearchRange)
    {
        if (!SearchRange.Contains(destination))
        {
            Debug.LogWarning("DESTINATION BEYOND SEARCH RANGE");
            return new Stack<Vector3Int>();
        }
        
        var Q = new Dictionary<Vector3Int, Node>(); //the unvisited set
        var W = new Dictionary<Vector3Int, Node>(); //the visited set

        foreach (var gridPos in SearchRange)
        {
            var tileData = TM.getTileDataByGridCoords(gridPos);
            if (tileData == null) continue;
            
            var v = new Node(gridPos, tileData.travelCost);
            Q.Add(gridPos, v);
            if (gridPos == start) v.distance = 0;
        }

        while (Q.Count != 0)
        {
            var u = getMinDist(Q);
            if (u == null)
            {
                break;
            }

            Q.Remove(u.gridPos);
            W.Add(u.gridPos, u);

            foreach (var neig in u.neighbors)
            {
                if (Q.ContainsKey(neig))
                {
                    var newDist = Q[neig].cost + u.distance;
                    var v = Q[neig];

                    if (newDist < v.distance)
                    {
                        v.distance = newDist;
                        v.prev = u;
                    }
                }
                
            }
            
            if (u.gridPos == destination)
            {
                break;
            }
            
        }

        var newPath = new Stack<Vector3Int>();

        if (W.ContainsKey(destination))
        {
            var v = W[destination];
            newPath.Push(destination);

            while (v.gridPos != start)
            {
                v = v.prev;
                newPath.Push(v.gridPos);
            }
        }
        else
        {
            Debug.LogError("UNKNOWN PATH ERROR");
        }
        
        return newPath;
    }

    private Node getMinDist(Dictionary<Vector3Int, Node> Q)
    {
        var minDist = float.MaxValue;
        Node minNode = null;
        foreach (var node in Q.Values.Where(node => minDist > node.distance))
        {
            minDist = node.distance;
            minNode = node;
        }
       
        return minNode;
    }
    
    private class Node
    {
        public readonly Vector3Int gridPos;
        public readonly float cost;
        public readonly List<Vector3Int> neighbors; 

        public float distance;
        public Node prev;

        public Node(Vector3Int gridPos, float cost)
        {
            this.gridPos = gridPos;
            this.cost = cost;
            distance = int.MaxValue;
            neighbors = TileManager.Instance.getNeighbors(gridPos);
        }
    }
}
