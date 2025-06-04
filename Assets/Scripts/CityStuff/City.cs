using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CityStuff
{
    public class City : MonoBehaviour
    {

        [SerializeField] private GameObject houseGameObject;
        [SerializeField] private GameObject wellGameObject;
        [SerializeField] private GameObject churchGameObject;
    
        private readonly List<Vector2> _buildingPoints = new ()
        {
            new Vector2(0.25f, 0f),
            new Vector2(0.25f, 0.25f),
            new Vector2(0f, 0.25f),
            new Vector2(-0.25f, 0.25f),
            new Vector2(-0.25f, 0f),
            new Vector2(-0.25f, -0.25f),
            new Vector2(0f, -0.25f),
            new Vector2(0.25f, -0.25f)
        };
    
        private House _house;
        private Well _well;
        private Church _church;

        private CityModel _cityModel;

        private void Awake()
        {
            _cityModel = new CityModel()
            {
                City = this,
                CityName = "City"
            };
        }

        public void BuildChurch()
        {
            _church = InstantiateAtRandomPoint(churchGameObject).GetComponent<Church>();
        }

        public void BuildWell()
        {
            _well = InstantiateAtRandomPoint(wellGameObject).GetComponent<Well>();
        }
    
        public void BuildHouse()
        {
            _house = InstantiateAtRandomPoint(houseGameObject).GetComponent<House>();
        }

        private GameObject InstantiateAtRandomPoint(GameObject prefab)
        {
            var randomNumber = Random.Range(0, _buildingPoints.Count);
            var randomPoint = _buildingPoints[randomNumber];
            _buildingPoints.RemoveAt(randomNumber);
        
            var instance = Instantiate(prefab, transform);
            instance.transform.localPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
            instance.transform.LookAt(transform);
            instance.SetActive(true);
        
            return instance;
        }
    }
}
