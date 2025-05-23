using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UICityMenu : UpdateableMenu<CityModel>
    {
        [SerializeField] private TextMeshProUGUI cityNameText;
        [SerializeField] private TextMeshProUGUI populationText;
        
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenCityMenu += OnOpen;
        }
        
        protected override void UpdateData(CityModel cityModel)
        {
            cityNameText.text = cityModel.CityName;
            populationText.text = cityModel.Population.ToString();
        }
    }
}
