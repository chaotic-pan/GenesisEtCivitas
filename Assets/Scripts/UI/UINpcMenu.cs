using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UINpcMenu : UpdateableMenu<NPCModel>
    {
        [SerializeField] private TextMeshProUGUI npcName;
        [SerializeField] private TextMeshProUGUI influenceText;
        [SerializeField] private TextMeshProUGUI foodText;
        [SerializeField] private TextMeshProUGUI waterText;
        [SerializeField] private TextMeshProUGUI safetyText;
        [SerializeField] private TextMeshProUGUI shelterText;
        [SerializeField] private TextMeshProUGUI energyText;

        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenNpcMenu += OnOpenNpcMenu;
        }

        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
        }
    
        protected override void UpdateData(NPCModel data)
        {
            npcName.text = data.NPCName;
            influenceText.text = data.Faith.ToString();
            foodText.text = data.Food.ToString();
            waterText.text = data.Water.ToString();
            safetyText.text = data.Safety.ToString();
            shelterText.text = data.Shelter.ToString();
            energyText.text = data.Energy.ToString();
        }
    }
}
