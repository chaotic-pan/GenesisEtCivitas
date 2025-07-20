using System.Collections;
using Events;
using Models;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Math = System.Math;

public class NPC : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private NPCInfluenceArea influenceArea;
    public static UnityEvent CheckMessiah = new ();
    public GameObject messiahPrefab;
    
    public NPCModel _npcModel = new NPCModel();
    private Civilization civ;
    private Messiah mes;
    private TileManager TM = TileManager.Instance;
    
    public void Awake()
    {
        GameEvents.Civilization.OnCivilizationMerge += MergeCivilisation;
        CheckMessiah.AddListener(CheckForMessiah);
        civ = transform.GetComponent<Civilization>();
        mes = transform.GetComponent<Messiah>();
        if (civ != null)
        {
            civ.Initialize();
            _npcModel.NPCName = civ.Language.GenerateWord();
            _npcModel.NPC = gameObject;
            GameEvents.Civilization.OnCivilizationSpawn?.Invoke(gameObject);
            
            StartCoroutine(StatsDecay(10));
            influenceArea.Initialize(this);
        }
    }
    private void OnDisable()
    {
        GameEvents.Civilization.OnCivilizationMerge -= MergeCivilisation;
    }
    
    private void CheckForMessiah()
    {
        if(_npcModel.IsMessiah)
        {
            var messiah = Instantiate(messiahPrefab, transform.position, Quaternion.identity);
            GameEvents.Civilization.OnMessiahSpawn.Invoke(messiah, gameObject);
            UIEvents.UIVar.saviourExists = true;
            if (_npcModel.City !=null) Destroy(_npcModel.City.gameObject);
            Destroy(gameObject);
        }
    }
  
    IEnumerator StatsDecay(float timer)
    {
        if (civ == null) yield return 0;
        yield return new WaitForSeconds(timer);
        
        var tilePos = TM.map.WorldToCell(civ.transform.position);

        civ.Food = calculateStat(civ.Food, TM.GetFood(tilePos));
        civ.Water = calculateStat(civ.Water, TM.GetWater(tilePos));
        civ.Safety = calculateStat(civ.Safety, TM.GetSafety(tilePos));
        civ.Shelter = calculateStat(civ.Shelter, TM.GetShelter(tilePos));
        civ.Energy = calculateStat(civ.Energy, TM.GetEnergy(tilePos));
        UpdateValues();

        if (civ.Food == 0 && civ.Water == 0)
        {
            GameEvents.Civilization.OnCivilizationDeath.Invoke(gameObject);
            StartCoroutine(Death(5));
        }
        else
        {
            StartCoroutine(StatsDecay(timer));
        }
    }

    IEnumerator Death(float wait)
    {
        yield return new WaitForSeconds(wait);
        Destroy(gameObject);
    }

    private float calculateStat(float stat, float tileStat)
    {
        // TODO BALANCING!!
        stat -= 1 * (float)civ.population/2;
        stat += tileStat*0.5f;
        return Math.Clamp(stat, 0, 99);
    }

    public void IncreaseInfluence()
    {
        _npcModel.Faith = civ.Belief;   
    }
    
    int clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    public void OnPointerClick(PointerEventData eventData)
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Ability"));
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask) 
            && hit.collider.gameObject == gameObject)
        {
            clicked++;
            
            if (clicked > 1)
            {
                if (Time.time - clicktime < clickdelay)
                {
                    clicked = 0;
                    clicktime = 0;
                    
                    // do double click stuff
                    GameEvents.Camera.OnJumpToCiv.Invoke(gameObject);
                }
                else
                {
                    clicked = 1;
                    clicktime = Time.time;
                }
            }
            
            if (clicked == 1)
            {
                clicktime = Time.time;
                
                // do single click stuff
                if (civ != null)
                {
                    UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
                    UpdateValues();
                
                }
                else if (mes != null)
                {
                    UIEvents.UIOpen.OnOpenMessiahMenu.Invoke(_npcModel);
                }
            }
            
        }

    }
    
    public void UpdateValues()
    {
        if (civ == null) return;
        _npcModel.Food = civ.Food;
        _npcModel.Water = civ.Water;
        _npcModel.Safety = civ.Safety;
        _npcModel.Shelter = civ.Shelter;
        _npcModel.Energy = civ.Energy;
        _npcModel.Faith = civ.Belief;
    }

    private void MergeCivilisation(GameObject civAObject, GameObject civBObject)
    {
        if (gameObject != civBObject) return;
        
        var civA = civAObject.GetComponent<Civilization>();
        var popA = civA.population;
        var popB = civ.population;

        civ.Food = averageStat(popA, popB, civA.Food, civ.Food);
        civ.Water = averageStat(popA, popB, civA.Water, civ.Water);
        civ.Safety = averageStat(popA, popB, civA.Safety, civ.Safety);
        civ.Shelter = averageStat(popA, popB, civA.Shelter, civ.Shelter);
        civ.Energy = averageStat(popA, popB, civA.Energy, civ.Energy);

        UpdateValues();
    }

    private float averageStat(float popA, float popB, float statA, float statB)
    {
        return (popA / popB * statB + statA) / (popA/popB + 1);
    }
}
