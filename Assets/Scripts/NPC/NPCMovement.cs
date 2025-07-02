using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private int rangeRadius = 5;

    public Tilemap map;
    private TileManager TM = TileManager.Instance;
    private MapExtractor ME = MapExtractor.Instance;

    private float movementSpeed = 5f;
    private List<Vector3Int> range;

    private void Start()
    {
        map = TM?.map;
        transform.position = AdjustCoordsForHeight(transform.position);
        StartCoroutine(WaitAndSettle());
    }
    
    IEnumerator WaitAndSettle()
    {
        // suspend execution for 2 seconds
        yield return new WaitForSeconds(2);
        if (transform.GetComponent<Civilization>() != null) FindSettlingLocation(20);
    }

    private void FindSettlingLocation(int range)
    {
        Vector3Int gridPos = map.WorldToCell(transform.position);
        List<Vector3Int> locations = TM.GetSpecificRange(gridPos, range);

        Vector3Int settlingPos = new Vector3Int();
        float winValue = 0;
        foreach(Vector3Int loc in locations)
        {
            float value = (TM.GetFood(loc) + TM.GetWater(loc) + TM.GetSafety(loc) + TM.GetShelter(loc) + TM.GetEnergy(loc)) / 5;
            if(winValue < value)
            {
                winValue = value;
                settlingPos = loc;
            }
        }
        MovetoTile(settlingPos);
        transform.GetComponent<Civilization>().hasSettlingLoc = true;
        transform.GetComponent<Civilization>().GetSettlingValues(settlingPos);
    }
    
# region DEBUG
    [Tooltip("enable to have Civs spawn visual point along their walking path")]
    [SerializeField] private bool DEBUG_PathBreadcrumbs;
    private List<GameObject> DEBUG_breadcrumbs = new();

    private void DEBUG_spawnBreadcrumbs(Vector3 pos, int size)
    {
        if (!DEBUG_PathBreadcrumbs) return;
        var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.localScale = new Vector3(size,size,size);
        s.transform.position = pos;
        DEBUG_breadcrumbs.Add(s);
    }
    
    private void DEBUG_clearBreadcrumbs()
    {
        if (!DEBUG_PathBreadcrumbs) return;
        foreach (var crumb in DEBUG_breadcrumbs)
        {
            Destroy(crumb);
        }
        DEBUG_breadcrumbs.Clear();
    }
#endregion

    public void CalculateRange()
    {
        var p = transform.position;
        p.y = -1;
        range = TM.GetSpecificRange(map.WorldToCell(p), rangeRadius);
    }

    private Vector3 AdjustCoordsForHeight(Vector3 coord)
    {
        var height = ME.GetHeightByWorldCoord(coord);
        return new Vector3(coord.x,height , coord.z);
    }
    
    public void MovetoTile(Vector3Int gridPos)
    {
        MovetoTileAndExecute(gridPos, null);
    }
    public void MovetoTileInRange(Vector3Int gridPos, List<Vector3Int> range) 
    {
        MovetoTileInRangeAndExecute(gridPos, range, null);
    }
    public void MovetoTileAndExecute(Vector3Int gridPos, Action<int> doOnReached)
    {
        CalculateRange();
        MovetoTileInRangeAndExecute(gridPos, null, doOnReached);
    }
    public void MovetoTileInRangeAndExecute(Vector3Int gridPos, List<Vector3Int> range, Action<int> doOnReached)
    {
        DEBUG_clearBreadcrumbs();
        StopAllCoroutines();
        
        var npcGridPos = map.WorldToCell(transform.position);
        var path = Dijkstra(npcGridPos, gridPos, range ?? this.range);
        
        DEBUG_spawnBreadcrumbs(AdjustCoordsForHeight(map.CellToWorld(gridPos)),5);
        
        StartCoroutine(FollowPath(path, doOnReached));
    }
    
    IEnumerator FollowPath(Stack<Vector3Int> path, Action<int> onReached)
    {
        // one pop to remove first path point which is the current pos
        if (!path.TryPop(out var pather)) yield break;
        
        GameEvents.Civilization.OnStartWalking.Invoke(gameObject);

        while (path.Count > 0)
        {
            yield return StartCoroutine("MovetoTarget", 
                AdjustCoordsForHeight(map.CellToWorld(path.Pop())));
        }
        
        GameEvents.Civilization.OnStopWalking.Invoke(gameObject);
        DEBUG_clearBreadcrumbs();
        
        // Build city if no city is existent at location after movement
        if (transform.TryGetComponent<Civilization>(out var civie))
        {
            if (civie.city == null && civie.hasSettlingLoc)
            {
                civie.city = CityBuilder.Instance.BuildCity(transform.position, transform.GetComponent<NPC>()._npcModel, civie);
                GameEvents.Civilization.OnCityFounded.Invoke(gameObject);
            }
        }

        onReached?.Invoke(GetInstanceID());
    }
    
    IEnumerator MovetoTarget(Vector3 target)
    {
        DEBUG_spawnBreadcrumbs(target,2);
        var position = transform.position;
        
        while (Vector3.Distance(position, target) > 10f)
        {
            var direction = (target - position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            var rotation = Quaternion.LerpUnclamped(transform.rotation, lookRotation, 
                Time.deltaTime);

            transform.rotation = rotation;
            position += transform.forward * (movementSpeed * Time.deltaTime);
            transform.position = AdjustCoordsForHeight(position);
            
            // TODO speed
            // var p = ME.CoordsToPoints(position);
            // movementSpeed = maxSpeed - ME.travelcost[p.x, p.y]/2;
            // movementSpeed = movementSpeed < 1 ? 1 : movementSpeed;
           
            yield return null;
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
