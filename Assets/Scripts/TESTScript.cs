using System;
using CityStuff;
using UnityEngine;

public class TESTScript : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void Start()
    {
        var go = Instantiate(prefab);
        go.transform.position = Vector3.zero;

        var city = go.GetComponent<City>();
        
        city.Initialize("TestCity");
        city.BuildWell();
    }
}
