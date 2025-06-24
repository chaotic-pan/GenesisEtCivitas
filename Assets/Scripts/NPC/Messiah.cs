using System.Threading.Tasks;
using UnityEngine;

public class Messiah : MonoBehaviour
{
    private NPCMovement npcMove;
    private void Start()
    {
        npcMove = this.gameObject.GetComponent<NPCMovement>();
    }

    public void GrantScoreImprovements(Civilization civ)
    {
        Vector3Int tilePos = npcMove.map.WorldToCell(civ.transform.position);
        npcMove.MovetoTile(tilePos);
        ChangeCiviScores(civ);
    }

    private void ChangeCiviScores(Civilization civ)
    {
        civ.Food += 5;
        civ.Water += 5;
        civ.Safety += 5;
        civ.Shelter += 5;
        civ.Energy += 5;
    }
}
