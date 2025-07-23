using System.Collections.Generic;
using Events;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UICivMenu : MonoBehaviour
    {
        [SerializeField] private GameObject civMenuPrefab;
        [SerializeField] private TMP_Text buttonText;

        private readonly Dictionary<NPCModel, CivMenuRow> _civRows = new();

        private bool _isOpen;
    
        private void Awake()
        {
            GameEvents.Civilization.OnCivilizationSpawn += OnCivilizationSpawn;
            GameEvents.Civilization.OnMessiahSpawn += RemoveCivilization;
            GameEvents.Civilization.OnCityFounded += OnCivilizationSettled;
        }
        
        private void OnDestroy()
        {
            GameEvents.Civilization.OnCivilizationSpawn -= OnCivilizationSpawn;
            GameEvents.Civilization.OnMessiahSpawn -= RemoveCivilization;
            GameEvents.Civilization.OnCityFounded -= OnCivilizationSettled;
        }

        private void RemoveCivilization(GameObject messiah, GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>()._npcModel;
            var civRow = _civRows[npcModel].gameObject;
            
            Destroy(civRow);
            _civRows.Remove(npcModel);
        }

        private void OnCivilizationSpawn(GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>()._npcModel;
            var civModel = Instantiate(civMenuPrefab, transform);
            
            civModel.GetComponent<CivMenuRow>().Initialize(npcModel);
            _civRows.Add(npcModel,  civModel.GetComponent<CivMenuRow>());
        }
        
        private void OnCivilizationSettled(GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>()._npcModel;
            var civRow = _civRows[npcModel].gameObject;
            civRow.transform.GetChild(0).GetComponent<Image>().enabled = true;
        }

        public void OnToggleMenu()
        {
            _isOpen = !_isOpen;

            if (_isOpen)
            {
                transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -128, 0);
                buttonText.text = ">";
            }


            if (!_isOpen)
            {
                transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(300, -128, 0);
                buttonText.text = "<";
            }
            
        }
    }
}
