using Events;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CivMenuRow : MonoBehaviour
{
    [SerializeField] private TMP_Text civName;
    [SerializeField] private TMP_Text faith;
    [SerializeField] private TMP_Text water;
    [SerializeField] private TMP_Text food;
    [SerializeField] private TMP_Text shelter;
    [SerializeField] private TMP_Text safety;
    [SerializeField] private TMP_Text energy;
    
    private NPCModel _npcModel;
    
    public void Initialize(NPCModel npcModel)
    {
        npcModel.OnUpdateData.AddListener(UpdateData);
        
        _npcModel = npcModel;
        UpdateData(npcModel);
    }

    private void UpdateData(NPCModel npcModel)
    {
        civName.text = npcModel.NPCName;
        faith.text = ((int)Mathf.Round(npcModel.Faith)).ToString();
        water.text = ((int)Mathf.Round(npcModel.Water)).ToString();
        food.text = ((int)Mathf.Round(npcModel.Food)).ToString();
        shelter.text = ((int)Mathf.Round(npcModel.Shelter)).ToString();
        safety.text = ((int)Mathf.Round(npcModel.Safety)).ToString();
        energy.text = ((int)Mathf.Round(npcModel.Energy)).ToString();
    }

    public void OnClickOnCiv()
    {
        GameEvents.Camera.OnJumpToCiv(_npcModel.NPC);
    }
}
