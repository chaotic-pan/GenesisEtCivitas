using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIMessiahMenu : UpdateableMenu<NPCModel>
    {
        private NPCModel model;
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenMessiahMenu += OnOpenNpcMenu;     
        }

        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
            model = npcModel;
        }
    
        protected override void UpdateData(NPCModel data)
        {
            // npcName.text = data.NPCName;
        }

        public void ButtonPressMessiah()
        {
            model.IsMessiah = true;
            NPC.CheckMessiah.Invoke();
        }
    }
}
