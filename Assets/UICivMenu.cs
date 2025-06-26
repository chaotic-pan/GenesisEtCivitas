using System;
using Events;
using Models;
using UnityEngine;

public class UICivMenu : MonoBehaviour
{
    [SerializeField] private GameObject civMenuPrefab;
    
    private void Awake()
    {
        GameEvents.Civilization.OnCivilizationSpawn += OnCivilizationSpawn;
    }

    private void OnCivilizationSpawn(NPCModel npcModel)
    {
        var civModel = Instantiate(civMenuPrefab, transform);
        civModel.GetComponent<CivMenuRow>().Initialize(npcModel);
    }
}
