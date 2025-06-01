using System;
using System.Collections;
using Models;
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private float cycleSpeed;
    
    private DayNightCycleModel _cycleModel;
    private const float DayLengthInSeconds = 600f; // 10 irl minutes per day
    private float _timePerMinute;
    
    public void Initialize()
    {
        // day length / hours / minutes = every in game minute
        _timePerMinute = DayLengthInSeconds / 24 / 60;

        _cycleModel = new DayNightCycleModel();

        StartCycle();
    }

    public void StartCycle()
    {
        _cycleModel.AdvanceCycle = true;
        StartCoroutine(CycleInGameMinutes());
    }

    public void StopCycle()
    {
        _cycleModel.AdvanceCycle = false;
        StopCoroutine(CycleInGameMinutes());
    }

    private void Update()
    {
        if (!_cycleModel.AdvanceCycle)
            return;
        
        _cycleModel.ElapsedRealTimeInSeconds += Time.deltaTime;
    }

    private IEnumerator CycleInGameMinutes()
    {
        while (_cycleModel.AdvanceCycle)
        {
            yield return new WaitForSeconds(_timePerMinute);
            _cycleModel.CurrentInGameTimeInSeconds++;
        }
    }
}
