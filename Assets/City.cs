using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class City : MonoBehaviour
{
    // City hall, Church, House, Predigt, Brunnen

    [SerializeField] private GameObject houseGameObject;
    [SerializeField] private GameObject wellGameObject;
    [SerializeField] private GameObject churchGameObject;
    
    private readonly List<Vector2> _buildingPoints = new ()
    {
        new Vector2(0.25f, 0f),
        new Vector2(0.25f, 0.25f),
        new Vector2(0f, 0.25f),
        new Vector2(-0.25f, 0.25f),
        new Vector2(-0.25f, 0f),
        new Vector2(-0.25f, -0.25f),
        new Vector2(0f, -0.25f),
        new Vector2(0.25f, -0.25f)
    };
    
    private House _house;
    private Well _well;
    private Church _church;

    private void Awake()
    {
        _house = InstantiateAtRandomPoint(houseGameObject).GetComponent<House>();
        _well = InstantiateAtRandomPoint(wellGameObject).GetComponent<Well>();
        _church = InstantiateAtRandomPoint(churchGameObject).GetComponent<Church>();
    }

    private GameObject InstantiateAtRandomPoint(GameObject prefab)
    {
        var randomNumber = Random.Range(0, _buildingPoints.Count);
        var randomPoint = _buildingPoints[randomNumber];
        _buildingPoints.RemoveAt(randomNumber);
        
        var instance = Instantiate(prefab, transform);
        instance.transform.localPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
        instance.transform.LookAt(transform);
        instance.SetActive(true);
        
        return instance;
    }
}
