using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Test
{
    public class CityTest : MonoBehaviour, IPointerClickHandler
    {
        private CityModel _cityModel;

        public void Initialize(CityModel cityModel)
        {
            _cityModel = cityModel;
            
            var coroutine = SpawnNewPop();
            StartCoroutine(coroutine);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                UIEvents.UIOpen.OnOpenCityMenu.Invoke(_cityModel);
            }
        }

        IEnumerator SpawnNewPop()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                _cityModel.Population += 1;
            }
        }
    }
}
