using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private TileManager tileManager;
    [SerializeField] private GameObject npc;
    private Tilemap map;

    [SerializeField] private float speed = 5f;
    private Vector3 destination;
    private Plane clickPlane;
    private AnimManager npcAnim;
    
    void Start()
    {
        map = tileManager.map;
        // FindTile();
        destination = npc.transform.position;
        clickPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        npcAnim = npc.GetComponent<AnimManager>();
    }

    void FindTile()
    {
        destination = map.CellToWorld(new Vector3Int(0,0,0));
        // destination = map.CellToWorld(new Vector3Int(0, 1, -1));
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
                destination = map.CellToWorld(gridPos);
                
               tileManager.printTileData(gridPos);
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
            npcAnim.SetIsMoving(false);
        }
    }
}
