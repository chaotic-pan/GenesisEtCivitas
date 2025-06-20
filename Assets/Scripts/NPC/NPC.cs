using System.Collections;
using Models;
using UI;
using Unity.Mathematics.Geometry;
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
        StartCoroutine(StatsDecay(10));
    }

    IEnumerator StatsDecay(float timer)
    {
        yield return new WaitForSeconds(timer);
        civ.Food -= 1;
        civ.Food = civ.Food <= 0 ? 0 : civ.Food;
        civ.Water -= 1;
        civ.Water = civ.Water <= 0 ? 0 : civ.Water;
        civ.Safety -= 1; 
        civ.Safety = civ.Safety <= 0 ? 0 : civ.Safety;
        civ.Shelter -= 1;
        civ.Shelter = civ.Shelter <= 0 ? 0 : civ.Shelter;
        civ.Energy -= 1;
        civ.Energy = civ.Energy <= 0 ? 0 : civ.Energy;
        UpdateValues();
        StartCoroutine(StatsDecay(timer));
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
