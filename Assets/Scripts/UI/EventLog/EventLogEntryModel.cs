using UnityEngine;

namespace UI.EventLog
{
    public class EventLogEntryModel
    {
        public string Message;
        public GameObject EventObject;
        
        public EventLogEntryModel(string message, GameObject eventObject)
        {
            Message = message;
            EventObject = eventObject;
        }
    }
}