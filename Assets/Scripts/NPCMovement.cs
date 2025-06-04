using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Plane = UnityEngine.Plane;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private int rangeRadius = 5;
    
    private Tilemap map;
    private AnimManager npcAnim;
    private TileManager TM = TileManager.Instance;
    private MapExtractor ME = MapExtractor.Instance;

    private float movementSpeed = 5f;
    private List<Vector3Int> range;
    private Stack<Vector3Int> path = new Stack<Vector3Int>();
    private Vector3 pathPoint;
    private Plane clickPlane;

    private void Start()
    {
        map = TM.map;

        npcAnim = transform.GetComponentInChildren<AnimManager>();
        
        //DEBUG
        clickPlane = new Plane(Vector3.up, new Vector3(0, -1, 0));

        pathPoint = AdjustCoordsForHeight(transform.position);
        transform.position = pathPoint;
    }

    private void Update()
    {
        //DEBUG Click on tile moves NPC there
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main is not null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (clickPlane.Raycast(ray, out float enter))
                {
                    var clickPos = ray.GetPoint(enter);
                    MovetoTile(map.WorldToCell(clickPos));
                }
            }
        }

        var position = transform.position;
        var dist = Vector3.Distance(position, pathPoint);

        if (dist > 0.05f)
        {
            var direction = (pathPoint - position).normalized;
            var rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.LookRotation(direction), 
                Time.deltaTime*movementSpeed);

            transform.rotation = rotation;
            position += transform.forward * (movementSpeed * Time.deltaTime);
            transform.position = AdjustCoordsForHeight(position);
            
            var p = ME.CoordsToPoints(position);
            movementSpeed = maxSpeed - ME.travelcost[p.x, p.y]/2;
            movementSpeed = movementSpeed < 1 ? 1 : movementSpeed;
        }
        else
        {
            if (path.TryPop(out var pather))
            {
                pathPoint = AdjustCoordsForHeight(map.CellToWorld(pather));
            }
            else
            {
                npcAnim.SetIsMoving(false);
            }
        }
    }

    public void CalculateRange()
    {
        var p = transform.position;
        p.y = -1;
        range = GetSpecificRange(map.WorldToCell(p), rangeRadius);
    }
    
    public List<Vector3Int> GetSpecificRange(Vector3Int gridPos, int radius)
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

    private Vector3 AdjustCoordsForHeight(Vector3 coord)
    {
        var height = ME.GetHeightByWorldCoord(coord);
        return new Vector3(coord.x,height , coord.z);
    }

    public void MovetoTile(Vector3Int gridPos)
    {
        // get current pos
        var npcGridPos = map.WorldToCell(transform.position);
        //calculate path between current and target pos
        CalculateRange();
        path = Dijkstra(npcGridPos, gridPos, range);
        
        if (path.TryPop(out var pather))
        {
            // double pop bc the first path point is the current pos
            if (path.TryPop(out pather))
            {
                pathPoint = AdjustCoordsForHeight(map.CellToWorld(pather));
                npcAnim.SetIsMoving(true);
            }  
        }
    }
    
    private Stack<Vector3Int> Dijkstra(Vector3Int start, Vector3Int destination, List<Vector3Int> SearchRange)
    {
        if (!SearchRange.Contains(destination))
        {
            Debug.LogWarning("DESTINATION BEYOND SEARCH RANGE");
            return new Stack<Vector3Int>();
        }

        start.z = 0;
        var Q = new Dictionary<Vector3Int, Node>(); //the unvisited set
        var W = new Dictionary<Vector3Int, Node>(); //the visited set

        foreach (var gridPos in SearchRange)
        {
            var tileData = TM.getTileDataByGridCoords(gridPos);
            if (tileData == null) continue;

            var p = ME.CoordsToPoints(map.CellToWorld(gridPos));
            var v = new Node(gridPos, ME.travelcost[p.x, p.y]);
            Q.Add(gridPos, v);
            if (gridPos == start) v.distance = 0;
        }

        while (Q.Count != 0)
        {
            var u = GetMinDist(Q);
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

    private Node GetMinDist(Dictionary<Vector3Int, Node> Q)
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
