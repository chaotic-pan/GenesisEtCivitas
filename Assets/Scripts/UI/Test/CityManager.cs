using System;
using Models;
using UnityEngine;

namespace UI.Test
{
    public class CityManager : MonoBehaviour
    {
        [SerializeField] private CityTest city1;
        [SerializeField] private CityTest city2;

        [SerializeField] private int city1PopulationTest = 65;
        [SerializeField] private int city2PopulationTest = 12;

        private void Start()
        {
            var cityData1 = new CityModel()
            {
                CityName = "Bravil",
                Population = city1PopulationTest
            };
            
            var cityData2 = new CityModel()
            {
                CityName = "City2",
                Population = city2PopulationTest
            };
            
            city1.Initialize(cityData1);
            city2.Initialize(cityData2);
        }
    }
}
