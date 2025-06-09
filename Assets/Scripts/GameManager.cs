using System;
using Events;
using Models;
using UnityEngine;

[RequireComponent(typeof(DayNightCycle))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private int maxDays = 7;
    
    private DayNightCycle _dayNightCycle;

    private void Awake()
    {
        GameEvents.DayNightCycle.OnDayNightCycleUpdate += OnGameEnd;
        _dayNightCycle = GetComponent<DayNightCycle>();
    }

    // Entry point
    void Start()
    {
        _dayNightCycle.Initialize();
    }

    // Game ended
    void OnGameEnd(DayNightCycleModel dayNightCycleModel)
    {
        if (dayNightCycleModel.CurrentInGameDay >= maxDays)
        {
            GameEvents.Lifecycle.OnGameEnd.Invoke();
            
        }
    }
}
