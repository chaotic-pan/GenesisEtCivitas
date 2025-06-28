using System.Collections.Generic;
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
    
        private House _house;
        private Well _well;
        private Church _church;

        private Civilization civ;

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

            var desiredHeight = MapExtractor.Instance.GetHeightByWorldCoord(instance.transform.position);
            
            instance.transform.position = new Vector3(instance.transform.position.x, desiredHeight, instance.transform.position.z);
            
            instance.transform.LookAt(transform);
            instance.SetActive(true);
        
            return instance;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
                UIEvents.UIOpen.OnSelectCityMessiahAction.Invoke(civ);
            }
        }
    }
}
