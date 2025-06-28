using System;
using System.Collections;
using Events;
using TMPro;
using UnityEngine;

namespace UI.EventLog
{

    public class UIEventLogEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        private GameObject _eventObject;

        public void Initialize(string message, GameObject eventObject)
        {
            _eventObject = eventObject;
            text.text = message;

            StartCoroutine(RemoveMessage());
        }

        public void OnClick()
        {
            GameEvents.Camera.OnJumpToCiv.Invoke(_eventObject);
        }

        public void DestroyMessage()
        {
            StopCoroutine(RemoveMessage());
            Destroy(gameObject);
        }
    
        IEnumerator RemoveMessage()
        {
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }

    }
}
