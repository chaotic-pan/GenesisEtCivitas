using Models;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPC : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private NPCInfluenceArea influenceArea;
    
    private NPCModel _npcModel = new NPCModel();
    private Civilization civ;
    
    public void Awake()
    {
        influenceArea.Initialize(this);
        civ = this.transform.GetComponent<Civilization>();
    }

    public void IncreaseInfluence(int influence)
    {
        _npcModel.Faith += influence;   
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Ability"));
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
                //TODO: Make this section less bad
                UpdateValues();
                
            }
        }

    }
    private void UpdateValues()
    {
        _npcModel.Food = civ.Food;
        _npcModel.Water = civ.Water;
        _npcModel.Safety = civ.Safety;
        _npcModel.Shelter = civ.Shelter;
        _npcModel.Energy = civ.Energy;
    }
}
