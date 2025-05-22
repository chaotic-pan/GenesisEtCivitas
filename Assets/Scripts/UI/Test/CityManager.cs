using System;
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
            var cityData1 = new CityData()
            {
                CityName = "Bravil",
                OtherInfo = 123,
                Population = city1PopulationTest
            };
            
            var cityData2 = new CityData()
            {
                CityName = "City2",
                OtherInfo = 9756,
                Population = city2PopulationTest
            };
            
            city1.Initialize(cityData1);
            city2.Initialize(cityData2);
        }
    }
}
