using Events;
using UnityEngine;
using UnityEngine.Events;

public class Messiah : MonoBehaviour
{
    private SaviourMovement npcMove;
    private Civilization civ;
    
    public static UnityEvent<Civilization> UseMessiah = new ();
    public static UnityEvent<Vector3Int> SendMessiah = new ();

    
    private void Start()
    {
        npcMove = gameObject.GetComponent<SaviourMovement>();
        GetComponentInChildren<Animator>().SetFloat("isSaviour", 1);
        UseMessiah.AddListener(GrantScoreImprovements);
        SendMessiah.AddListener(OnSendMessiah);
    }

    public void OnSendMessiah(Vector3Int gridPos)
    {
        npcMove.MovetoTileInRange(gridPos, TileManager.Instance.GetFullRange());
    }

    public void GrantScoreImprovements(Civilization civi)
    {
        civ = civi;
        var civPos = civ.transform.position;
        civPos.y = 0;
        Vector3Int tilePos = npcMove.map.WorldToCell(civPos);
        npcMove.MovetoTileInRangeAndExecute(tilePos, TileManager.Instance.GetFullRange(), OnCityReached);
    }

    private void OnCityReached(GameObject npc)
    {
        if (npc == npcMove.gameObject && civ != null)
        { 
            GameEvents.Civilization.OnPreach.Invoke(gameObject);
            ChangeCiviScores();
        }
    }

    private void ChangeCiviScores()
    {
        civ.Food += 15;
        civ.Water += 15;
        civ.Safety += 15;
        civ.Shelter += 15;
        civ.Energy += 15;

        civ = null;
    }
}
