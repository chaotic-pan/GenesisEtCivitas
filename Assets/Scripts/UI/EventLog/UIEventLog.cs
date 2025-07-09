using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace UI.EventLog
{
    public class UIEventLog : MonoBehaviour
    {
        [SerializeField] private GameObject eventLogEntryPrefab;
    
        private readonly Queue<EventLogEntryModel> _eventQueue = new();
    
        private void Awake()
        {
            GameEvents.Civilization.OnCivilizationSpawn += OnCivilizationSpawn;
            GameEvents.Civilization.OnMessiahSpawn += OnMessiahSpawn;
            GameEvents.Civilization.OnCivilizationDeath += OnCivilizationDeath;
            GameEvents.Civilization.OnCivilizationSplit += OnCivilizationSplit;
            GameEvents.Civilization.OnCivilizationMerge += OnCivilizationMerge;
            GameEvents.Civilization.OnCityFounded += OnCityFounded;
            GameEvents.Civilization.OnCivilizationLowOnStats += OnCivlizationLowOnStats;
        }

        private void OnCivlizationLowOnStats(GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModel._npcModel.NPCName)} is struggling!";
            var eventLogEntry = new EventLogEntryModel(message, npcModelObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }

        private void Start()
        {
            StartCoroutine(ShowLogs());
        }
    
        IEnumerator ShowLogs()
        {
            while (true)
            {
                if (_eventQueue.Count > 0)
                {
                    ShowEventLog(_eventQueue.Dequeue());
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void ShowEventLog(EventLogEntryModel eventLogEntry)
        {
            MakeSpaceForLogs();
            
            var messageInstance = Instantiate(eventLogEntryPrefab, transform);
            messageInstance.GetComponent<UIEventLogEntry>().Initialize(eventLogEntry.Message, eventLogEntry.EventObject);
        }

        private void MakeSpaceForLogs()
        {
            if (transform.childCount < 5) return;
        
            transform.GetChild(0).GetComponent<UIEventLogEntry>().DestroyMessage();
        }

        private string Capitalize(string s)
        {
            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

        private void OnCivilizationSplit(GameObject npcModelAObject, GameObject npcModelBObject)
        {
            var npcModelA = npcModelAObject.GetComponent<NPC>();
            var npcModelB = npcModelBObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModelA._npcModel.NPCName)} split from {Capitalize(npcModelB._npcModel.NPCName)}!";
            var eventLogEntry = new EventLogEntryModel(message, npcModelAObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }

        private void OnCivilizationMerge(GameObject npcModelAObject, GameObject npcModelBObject)
        {
            var npcModelA = npcModelAObject.GetComponent<NPC>();
            var npcModelB = npcModelBObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModelA._npcModel.NPCName)} has joined {Capitalize(npcModelB._npcModel.NPCName)}!";
            var eventLogEntry = new EventLogEntryModel(message, npcModelBObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }

        private void OnCivilizationDeath(GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModel._npcModel.NPCName)} died!";
            var eventLogEntry = new EventLogEntryModel(message, npcModelObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }

        private void OnMessiahSpawn(GameObject messiahObject, GameObject npcModelObject)
        {
            var message = $"A new saviour has risen!";
            var eventLogEntry = new EventLogEntryModel(message, messiahObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }

        private void OnCivilizationSpawn(GameObject npcModelObject)
        {
            var npcModel = npcModelObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModel._npcModel.NPCName)} spawned!";
            var eventLogEntry = new EventLogEntryModel(message, npcModelObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }
        
        private void OnCityFounded(GameObject npcObject)
        {
            var npcModel = npcObject.GetComponent<NPC>();
            
            var message = $"{Capitalize(npcModel._npcModel.NPCName)} founded a new city: {npcModel._npcModel.City.CityName}";
            var eventLogEntry = new EventLogEntryModel(message, npcObject);
            
            _eventQueue.Enqueue(eventLogEntry);
        }
    }
}
