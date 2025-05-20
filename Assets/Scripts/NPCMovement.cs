using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private TileManager tileManager;
    [SerializeField] private GameObject npc;
    private Tilemap map;

    [SerializeField] private float speed = 5000f;

    void Start()
    {
        map = tileManager.map;
        FindTile();
    }

    void FindTile()
    {
        var centerTileWorldPos = map.CellToWorld(new Vector3Int(0,0,0));
        Vector3 destination = map.CellToWorld(new Vector3Int(0, 1, -1));

        Vector3 direction = (destination - npc.transform.position).normalized;
        //npc.transform.position = direction;
        npc.transform.position += direction * speed * Time.deltaTime;
        Debug.Log(destination);
    }

    void Update()
    {
        
    }
}
