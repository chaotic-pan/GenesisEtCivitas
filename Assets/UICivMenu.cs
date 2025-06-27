using System;
using Events;
using Models;
using TMPro;
using UnityEngine;

public class UICivMenu : MonoBehaviour
{
    [SerializeField] private GameObject civMenuPrefab;
    [SerializeField] private TMP_Text buttonText;

    private bool isOpen = true;
    
    private void Awake()
    {
        GameEvents.Civilization.OnCivilizationSpawn += OnCivilizationSpawn;
    }

    private void OnCivilizationSpawn(NPCModel npcModel)
    {
        var civModel = Instantiate(civMenuPrefab, transform);
        civModel.GetComponent<CivMenuRow>().Initialize(npcModel);
    }

    public void OnToggleMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(-392f, 0, 0);
            buttonText.text = ">";
        }


        if (!isOpen)
        {
            transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            buttonText.text = "<";
        }
            
    }
}
