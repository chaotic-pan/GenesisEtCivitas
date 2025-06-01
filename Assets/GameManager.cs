using System;
using UnityEngine;

[RequireComponent(typeof(DayNightCycle))]
public class GameManager : MonoBehaviour
{
    private DayNightCycle _dayNightCycle;

    private void Awake()
    {
        _dayNightCycle = GetComponent<DayNightCycle>();
    }

    // Entry point
    void Start()
    {
        _dayNightCycle.Initialize();
    }
}
