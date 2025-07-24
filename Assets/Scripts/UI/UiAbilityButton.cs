using Models;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UiAbilityButton : MonoBehaviour
{
    public int cost;
    public TextMeshProUGUI costsText;
    public Image costBG;
    public Image icon;

    private void Awake()
    {
        UIEvents.UIUpdate.OnUpdatePlayerData += CostCheck;
    }

    private void OnDisable()
    { 
       UIEvents.UIUpdate.OnUpdatePlayerData -= CostCheck;
    }
    
    public void CostCheck(PlayerModel pm)
    {
        if (cost > pm.InfluencePoints)
        {
            // too expensive
            costBG.color = Color.gray;
            icon.color = Color.gray;
            costsText.color = Color.gray;
        }
        else
        {
            costBG.color = Color.white;
            icon.color = Color.white;
            costsText.color = Color.white;
        }
    }
}
