using System;
using UnityEngine;

public class Messiah : MonoBehaviour
{
    private NPCMovement npcMove;
    private Civilization civ;
    
    private void Start()
    {
        npcMove = gameObject.GetComponent<NPCMovement>();
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
