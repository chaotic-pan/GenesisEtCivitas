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
        
        private City _city;
        
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenCityMenu += OnOpenGetCity;
            
        }

        private void OnOpenGetCity(CityModel cityModel)
        {
            OnOpen(cityModel);
            _city = cityModel.City;
        }

        protected override void UpdateData(CityModel cityModel)
        {
            cityNameText.text = cityModel.CityName;
            populationText.text = cityModel.Population.ToString();
        }

        public void OnBuildChurch()
        {
            if (!_city) return;
            _city.BuildChurch();
        }
    }
}
