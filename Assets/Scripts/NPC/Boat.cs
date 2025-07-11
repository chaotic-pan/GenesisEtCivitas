using System;
using System.Collections.Generic;
using UnityEngine;
public class Boat : MonoBehaviour
{
    [SerializeField] private List<Vector3> boatPos;
    [SerializeField] private GameObject bSmall;
    [SerializeField] private GameObject bMedium;
    [SerializeField] private GameObject bLarge;

    public void spawnBoats(int people)
    {
        var boatCount = 0;
        while (people > 0)
        {
            var boatSize = people switch
            {
                > 5 => 8,
                > 3 => 5,
                _ => 3
            };
            var boat = boatSize switch
            {
                8 => bLarge,
                5 => bMedium,
                _ => bSmall
            };

            boat = Instantiate(boat, Vector3.zero, Quaternion.identity, transform);
            boat.transform.localPosition = boatPos[boatCount];
            boat.transform.localRotation = Quaternion.identity;
            
            var boatPeeps = Math.Min(boatSize, people);
            for (int i = 0; i < boatPeeps; i++)
            {
                boat.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);                
            }
            people -= boatPeeps;
            boatCount++;
        }
        
    }
}