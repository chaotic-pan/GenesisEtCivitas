using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaviourMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private int rangeRadius = 5;

    private TileManager TM = TileManager.Instance;
    private MapExtractor ME = MapExtractor.Instance;

    private float movementSpeed = 5f;
    private List<Vector3Int> range;

    private void Start()
    {
        transform.position = ME.AdjustCoordsForHeight(transform.position);
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

    public void MovetoTile(Vector3Int gridPos)
    {
        MovetoTileAndExecute(gridPos, null);
    }
    public void MovetoTileInRange(Vector3Int gridPos, List<Vector3Int> range) 
    {
        MovetoTileInRangeAndExecute(gridPos, range, null);
    }
    public void MovetoTileAndExecute(Vector3Int gridPos, Action<GameObject> doOnReached)
    {
        CalculateRange();
        MovetoTileInRangeAndExecute(gridPos, null, doOnReached);
    }
    public async void MovetoTileInRangeAndExecute(Vector3Int gridPos, List<Vector3Int> range, Action<GameObject> doOnReached)
    {
        DEBUG_clearBreadcrumbs();
        StopAllCoroutines();
        
        var npcGridPos = TM.WorldToCell(transform.position);
        await Dijkstra(npcGridPos, gridPos, range ?? this.range);

        var destination = ME.AdjustCoordsForHeight(TM.CellToWorld(gridPos));
        DEBUG_spawnBreadcrumbs(destination,5);
        
        StartCoroutine(FollowPath(destination, doOnReached));
    }
    
    IEnumerator FollowPath(Vector3 destination, Action<GameObject> onReached)
    {
        // one pop to remove first path point which is the current pos
        if (!path.TryPop(out var pather)) yield break;
        
        GameEvents.Civilization.OnStartWalking?.Invoke(gameObject);

        while (path.Count > 0)
        {
            yield return StartCoroutine("MovetoTarget", 
                ME.AdjustCoordsForHeight(TM.CellToWorld(path.Pop())));
        }

        var position = transform.position;
        while (Vector3.Distance(position, destination) > 0.1f)
        {
            Vector3 direction = (destination - position).normalized;
            var newPos = position + direction * (10f * Time.deltaTime);
            transform.position = ME.AdjustCoordsForHeight(newPos);

            // dw about this. this is all totally necessary and efficient . prommie!
            var oldRot = transform.rotation;
            transform.rotation = Quaternion.LookRotation(direction);
            var look = transform.position + transform.forward;
            transform.LookAt(new Vector3(look.x, transform.position.y, look.z));
            var newRot = transform.rotation;
            transform.rotation = Quaternion.RotateTowards(oldRot, newRot, 5);
            position = newPos;
            
            yield return null;
        }
        
        GameEvents.Civilization.OnStopWalking?.Invoke(gameObject);
        DEBUG_clearBreadcrumbs();
        transform.position = destination;

        onReached?.Invoke(gameObject);
    }
    
    IEnumerator MovetoTarget(Vector3 target)
    {
        DEBUG_spawnBreadcrumbs(target,2);
        
        var position = transform.position;
        
        while (Vector3.Distance(position, target) > 1)
        {
            var gridPos = TM.WorldToCell(position);
            var cost = TM.GetTravelCost(gridPos);
            movementSpeed = Math.Max(1, maxSpeed - cost/10);

            
            var direction = (target - position).normalized;
            var newPos = position + direction * (movementSpeed * Time.deltaTime);
            transform.position = ME.AdjustCoordsForHeight(newPos);

            var oldRot = transform.rotation;
            transform.rotation = Quaternion.LookRotation(direction);
            var look = transform.position + transform.forward;
            transform.LookAt(new Vector3(look.x, transform.position.y, look.z));
            var newRot = transform.rotation;
            transform.rotation = Quaternion.RotateTowards(oldRot, newRot, 5);
            position = newPos;
            
            
            yield return null;
        }
    }

    private Stack<Vector3Int> path = new();
    
    #region pathfinding
    public void CalculateRange()
    {
        range = TM.GetSpecificRange(TM.WorldToCell(transform.position), rangeRadius);
    }

    private async Task Dijkstra(Vector3Int start, Vector3Int destination, List<Vector3Int> SearchRange)
    {
        if (!SearchRange.Contains(destination))
        {
            Debug.LogWarning("DESTINATION BEYOND SEARCH RANGE");
            path = new Stack<Vector3Int>();
            return;
        }

        start.z = 0;
        var Q = new Dictionary<Vector3Int, Node>(); //the unvisited set
        var W = new Dictionary<Vector3Int, Node>(); //the visited set

        var result = await Task.Run(() =>
        {
            foreach (var gridPos in SearchRange)
            {
                var tileData = TM.getTileDataByGridCoords(gridPos);
                if (tileData == null) continue;
            
                var v = new Node(gridPos, TM.getTileDataByGridCoords(gridPos).travelCost);
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
        });

        path = result;
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
#endregion
}
