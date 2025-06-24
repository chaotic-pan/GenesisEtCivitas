using System.Threading.Tasks;
using UnityEngine;

public class Messiah : MonoBehaviour
{
    private NPCMovement npcMove;
    private Civilization civ;
    
    private void Start()
    {
        npcMove = this.gameObject.GetComponent<NPCMovement>();
        npcMove.reachedDestination.AddListener(OnDestinationReached);
    }

    public void GrantScoreImprovements(Civilization civ)
    {
        Vector3Int tilePos = npcMove.map.WorldToCell(civ.transform.position);
        npcMove.MovetoTile(tilePos);
        this.civ = civ;
    }

    private void OnDestinationReached(int npcId)
    {
        if (npcId == npcMove.GetInstanceID() &&
            civ != null)
        {
             ChangeCiviScores(civ);
        }
    }

    private void ChangeCiviScores(Civilization civ)
    {
        civ.Food += 5;
        civ.Water += 5;
        civ.Safety += 5;
        civ.Shelter += 5;
        civ.Energy += 5;

        civ = null;
    }
}
