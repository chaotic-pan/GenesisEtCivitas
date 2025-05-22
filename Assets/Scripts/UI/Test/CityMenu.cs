using TMPro;
using UnityEngine;

namespace UI.Test
{
    public class CityMenu : UpdateableMenu<CityData>
    {
        [SerializeField] private TextMeshProUGUI cityNameText;
        [SerializeField] private TextMeshProUGUI populationText;
        [SerializeField] private TextMeshProUGUI otherInfoText;
        
        public override void Initialize()
        {
            GameEvents.UI.OnOpenCityMenu += OnOpen;
        }
        
        protected override void UpdateData(CityData cityData)
        {
            cityNameText.text = cityData.CityName;
            populationText.text = cityData.Population.ToString();
            otherInfoText.text = cityData.OtherInfo.ToString();
        }
    }
}
