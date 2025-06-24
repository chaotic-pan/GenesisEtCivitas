using System;
using System.Collections;
using Models;
using UI;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UI;

public class NPC : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private NPCInfluenceArea influenceArea;
    public static UnityEvent CheckMessiah = new UnityEvent();
    public GameObject messiahPrefab;
    
    private NPCModel _npcModel = new NPCModel();
    private Civilization civ;
    private Messiah mes;
    
    public void Awake()
    {
        CheckMessiah.AddListener(CheckForMessiah);
        civ = this.transform.GetComponent<Civilization>();
        mes = this.transform.GetComponent<Messiah>();
        if (civ != null)
        {
            StartCoroutine(StatsDecay(10));
            influenceArea.Initialize(this);
        }
    }

    private void CheckForMessiah()
    {
        if(_npcModel.IsMessiah == true)
        {
            Instantiate(messiahPrefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
    IEnumerator StatsDecay(float timer)
    {
        if (civ == null) yield return 0;
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
            if (hit.collider.gameObject == gameObject && civ != null)
            {
                UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
                //TODO: Make this section less bad
                UpdateValues();
                
            }
            else if (hit.collider.gameObject == gameObject && mes != null)
            {
                UIEvents.UIOpen.OnOpenMessiahMenu.Invoke(_npcModel);
            }
            
        }

    }
    private void UpdateValues()
    {
        if (civ == null) return;
        _npcModel.Food = civ.Food;
        _npcModel.Water = civ.Water;
        _npcModel.Safety = civ.Safety;
        _npcModel.Shelter = civ.Shelter;
        _npcModel.Energy = civ.Energy;
    }
}
