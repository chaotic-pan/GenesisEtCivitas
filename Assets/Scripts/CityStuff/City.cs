using System.Collections.Generic;
using Events;
using Models;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CityStuff
{
    public class City : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private GameObject housePrefab;
        [SerializeField] private GameObject wellPrefab;
        [SerializeField] private GameObject churchPrefab;
        [SerializeField] private GameObject cityCentrePrefab;
    
        public House _house;
        public Well _well;
        public Church _church;
        private GameObject selectionEffect;

        public Civilization civ;
        private MapExtractor ME = MapExtractor.Instance;

        public string CityName;
        
        private readonly List<Vector2> _buildingPoints = new ()
        {
            new Vector2(4f, 0f),
            new Vector2(4f, 4f),
            new Vector2(0f, 4f),
            new Vector2(-4f, 4f),
            new Vector2(-4f, 0f),
            new Vector2(-4f, -4f),
            new Vector2(0f, -4f),
            new Vector2(4f, -4f)
        };

        private NPCModel _npcModel;

        private void OnEnable()
        {
            GameEvents.Civilization.OnCivilizationDeath += AbandonCity;
            selectionEffect = transform.GetChild(0).gameObject;
            selectionEffect.SetActive(false);
        }

        private void OnDisable()
        { 
            GameEvents.Civilization.OnCivilizationDeath -= AbandonCity;
        }

        public void Initialize(NPCModel model, Civilization civi)
        {
            _npcModel = model;
            _npcModel.City = this;
            civ = civi;

            CityName = civ.Language.GenerateWord();
            
            BuildHouse();
        }

        public void BuildChurch()
        {
            if (_church) return;
            
            _church = InstantiateAtRandomPoint(churchPrefab).GetComponent<Church>();
            _church.Initialize();
        }

        public void BuildWell()
        {
            if (_well) return;
            
            _well = InstantiateAtRandomPoint(wellPrefab).GetComponent<Well>();
        }
    
        private void BuildHouse()
        {
            if (_house) return;
            
            _house = InstantiateAtRandomPoint(housePrefab).GetComponent<House>();
        }

        private GameObject InstantiateAtRandomPoint(GameObject prefab)
        {
            var randomNumber = Random.Range(0, _buildingPoints.Count);
            var randomPoint = _buildingPoints[randomNumber];
            _buildingPoints.RemoveAt(randomNumber);
        
            var instance = Instantiate(prefab, transform);
            instance.transform.localPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);

            instance.transform.position = ME.AdjustCoordsForHeight(instance.transform.position);
            
            instance.transform.LookAt(new Vector3(transform.position.x, instance.transform.position.y, transform.position.z));
            instance.SetActive(true);
        
            GameEvents.Civilization.CreateBuilding?.Invoke(civ.gameObject, instance);
            
            return instance;
        }
        
        int clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
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
                    UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
                    UIEvents.UIOpen.OnSelectCityMessiahAction.Invoke(civ);
                }

            }
        }

        public Vector3 GetHousePos()
        {
            return _house.transform.position;
        }

        private void AbandonCity(GameObject civObject)
        {
            if (civ == null || civObject == null) return;
            if (civ.gameObject != civObject) return;
            
            Destroy(gameObject);
        }
        
        public void OnMouseEnter()
        {
            if (!UIEvents.UIVar.isCastingSaviourAction) return;
            selectionEffect.SetActive(true);
        }

        public void OnMouseExit()
        {
            selectionEffect.SetActive(false);
        }
    }
}
