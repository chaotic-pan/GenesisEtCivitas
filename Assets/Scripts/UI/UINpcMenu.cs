using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UINpcMenu : UpdateableMenu<NPCModel>
    {
        [SerializeField] private TextMeshProUGUI npcName;
        [SerializeField] private TextMeshProUGUI influenceText;
    
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
        }
    }
}
