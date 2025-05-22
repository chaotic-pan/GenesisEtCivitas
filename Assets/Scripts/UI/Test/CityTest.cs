using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Test
{
    public class CityTest : MonoBehaviour, IPointerClickHandler
    {
        private CityData _cityData;

        public void Initialize(CityData cityData)
        {
            _cityData = cityData;
            
            var coroutine = SpawnNewPop();
            StartCoroutine(coroutine);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                GameEvents.UI.OnOpenCityMenu.Invoke(_cityData);
            }
        }

        IEnumerator SpawnNewPop()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                _cityData.Population += 1;
            }
        }
    }
}
