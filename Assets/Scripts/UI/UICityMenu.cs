using CityStuff;
using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UICityMenu : UpdateableMenu<CityModel>
    {
        [SerializeField] private TextMeshProUGUI cityNameText;
        [SerializeField] private TextMeshProUGUI populationText;
        
        private City city;
        
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenCityMenu += OnOpenGetCity;
            
        }

        private void OnOpenGetCity(CityModel cityModel)
        {
            Debug.Log(cityModel);
            OnOpen(cityModel);
            
            Debug.Log(cityModel.CityName);
            city = cityModel.City;
        }

        protected override void UpdateData(CityModel cityModel)
        {
            cityNameText.text = cityModel.CityName;
            populationText.text = cityModel.Population.ToString();
        }

        public void OnBuildChurch()
        {
            if (!city) return;
            Debug.Log("BUild ");
            city.BuildChurch();
        }
    }
}
