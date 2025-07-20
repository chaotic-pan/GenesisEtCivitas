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

        [SerializeField] private GameObject houseGameObject;
        [SerializeField] private GameObject wellGameObject;
        [SerializeField] private GameObject churchGameObject;
        [SerializeField] private GameObject citycentre;
    
        public House _house;
        private Well _well;
        private Church _church;

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

        public void Initialize(NPCModel model, Civilization civi)
        {
            _npcModel = model;
            _npcModel.City = this;
            civ = civi;

            CityName = civ.Language.GenerateWord();
            
            BuildHouse();
            
            // build city centre hex
            var instance = Instantiate(citycentre, transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = ME.AdjustCoordsForHeight(instance.transform.position);
        }

        public void BuildChurch()
        {
            if (_church) return;
            
            _church = InstantiateAtRandomPoint(churchGameObject).GetComponent<Church>();
            _church.Initialize();
        }

        public void BuildWell()
        {
            if (_well) return;
            
            _well = InstantiateAtRandomPoint(wellGameObject).GetComponent<Well>();
        }
    
        private void BuildHouse()
        {
            if (_house) return;
            
            _house = InstantiateAtRandomPoint(houseGameObject).GetComponent<House>();
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
    }
}
