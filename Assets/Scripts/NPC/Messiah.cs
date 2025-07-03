using System;
using Events;
using UnityEngine;
using UnityEngine.Events;

public class Messiah : MonoBehaviour
{
    private NPCMovement npcMove;
    private Civilization civ;
    
    public static UnityEvent<Civilization> UseMessiah = new ();
    public static UnityEvent<Vector3Int> SendMessiah = new ();

    
    private void Start()
    {
        npcMove = gameObject.GetComponent<NPCMovement>();
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
        Vector3Int tilePos = npcMove.map.WorldToCell(civ.transform.position);
        npcMove.MovetoTileInRangeAndExecute(tilePos, TileManager.Instance.GetFullRange(), OnCityReached);
    }

    private void OnCityReached(int npcId)
    {
        if (npcId == npcMove.GetInstanceID() && civ != null)
        { 
            GameEvents.Civilization.OnPreach.Invoke(gameObject, 10);
            ChangeCiviScores();
        }
    }

    private void ChangeCiviScores()
    {
        civ.Food += 5;
        civ.Water += 5;
        civ.Safety += 5;
        civ.Shelter += 5;
        civ.Energy += 5;

        civ = null;
    }
}
