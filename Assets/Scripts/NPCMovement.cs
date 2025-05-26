using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Tilemaps;
using Math = System.Math;
using Plane = UnityEngine.Plane;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private TileManager tileManager;
    [SerializeField] private GameObject npc;
    private Tilemap map;
    private Dictionary<Vector2Int, Tile> tiles;

    [SerializeField] private float speed = 5f;
    private Vector3 destination;
    private Stack<Tile> path = new Stack<Tile>();
    private Plane clickPlane;
    private AnimManager npcAnim;
    
    void Start()
    {
        map = tileManager.map;
        tiles = tileManager.tiles;
        
        destination = npc.transform.position;
        clickPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        npcAnim = npc.GetComponent<AnimManager>();
    }

    void Update()
    {
        // Click on tile moves NPC there
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (clickPlane.Raycast(ray, out float enter))
            {
                var clickPos = ray.GetPoint(enter);
                var gridPos = map.WorldToCell(clickPos);
                var npcGridPos = map.WorldToCell(npc.transform.position);
                path = dijkstra(new Vector2Int(npcGridPos.x, npcGridPos.y), new Vector2Int(gridPos.x, gridPos.y));
                if (path.TryPop(out var pather))
                {
                    // double po bc the first path point is the current pos
                    if (path.TryPop(out pather))
                    {
                        destination = map.CellToWorld(new Vector3Int(pather.pos.x, pather.pos.y, 0));;
                        print(destination);
                    }  
                }
                npcAnim.SetIsMoving(true);
            }
        }

        var dist = Vector3.Distance(npc.transform.position, destination);

        if (dist > 0.05f)
        {
            var position = npc.transform.position;
            Vector3 direction = (destination - position).normalized;
            position += direction * speed * Time.deltaTime;
            npc.transform.position = position;
            npc.transform.rotation = Quaternion.LookRotation (direction);
        }
        else
        {
            if (path.TryPop(out var pather))
            {
                destination = map.CellToWorld(new Vector3Int(pather.pos.x, pather.pos.y, 0));;
                print(destination);
            }
            else
            {
                npcAnim.SetIsMoving(false);
            }
        }
    }



    private Stack<Tile> dijkstra(Vector2Int start, Vector2Int destination)
    {
        var Q = new Dictionary<Vector2Int, Node>(); //the unvisited set
        var W = new Dictionary<Vector2Int, Node>(); //the visited set
        

        foreach (var gridPos in tiles.Keys)
        {
            var v = new Node(gridPos, tiles[gridPos]);
            v.distance = int.MaxValue;
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

            foreach (var neig in u.tile.neighbors)
            {
                if (Q.ContainsKey(neig.Key.pos))
                {
                    var newDist = neig.Value + u.distance;
                    var v = Q[neig.Key.pos];

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

        Stack<Tile> path = new Stack<Tile>();

        if (W.ContainsKey(destination))
        {
            var v = W[destination];
            path.Push(v.tile);

            while (v.gridPos != start)
            {
                v = v.prev;
                path.Push(v.tile);
            }
        }
        else
        {
            print("PATH ERROR???");
        }
        
        return path;
    }

    private Node getMinDist(Dictionary<Vector2Int, Node> Q)
    {
        var minDist = float.MaxValue;
        Node minNode = null;
        foreach (var node in Q.Values)
        {
            if (minDist > node.distance)
            {
                minDist = node.distance;
                minNode = node;
            }
            
        }
       
        return minNode;
    }
    
    private class Node
    {
        public Vector2Int gridPos;
        public Tile tile;
        
        public float distance;
        public Node prev;

        public Node(Vector2Int gridPos, Tile tile)
        {
            this.gridPos = gridPos;
            this.tile = tile;
        }
    }
}
