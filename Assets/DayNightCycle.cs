using Events;
using Models;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private float dayLengthInSeconds = 600f; // 10 irl minutes per day
    [SerializeField] private float speed = 1;
    [SerializeField] private bool debugMode = false;
    
    private int _minutesPerDay = 1440; 
    private float _elapsedRealTimeInSeconds;
    private float _currentSunAngle;
    
    private DayNightCycleModel _cycleModel;
    
    
    public void Initialize()
    {
        GameEvents.Lifecycle.OnGameEnd += StopCycle;
        _cycleModel = new DayNightCycleModel();
        StartCycle();
    }

    public void StartCycle()
    {
        _cycleModel.AdvanceCycle = true;
    }

    public void StopCycle()
    {
        _cycleModel.AdvanceCycle = false;
    }

    private void Update()
    {
        if (!_cycleModel.AdvanceCycle || debugMode)
            return;

        var delta = (Time.deltaTime * speed);
        _elapsedRealTimeInSeconds += delta;
        
        CalculateTime();
        RotateSun(delta);
    }

    private void CalculateTime()
    {
        var elapsedInGameMinutes = (int) (_elapsedRealTimeInSeconds / dayLengthInSeconds * _minutesPerDay);

        if (elapsedInGameMinutes <= _cycleModel.CurrentInGameMinute) 
            return;
        
        _cycleModel.CurrentInGameMinute = elapsedInGameMinutes % 60;
        _cycleModel.CurrentInGameHour = (elapsedInGameMinutes / 60) % 24;
        _cycleModel.CurrentInGameDay = elapsedInGameMinutes / 60 / 24;
    }

    private void RotateSun(float delta)
    {
        float deltaAngle = (360f / dayLengthInSeconds) * delta;
        _currentSunAngle += deltaAngle;

        // Keep angle within 0-360 for clarity
        if (_currentSunAngle >= 360f) _currentSunAngle -= 360f;

        light.transform.rotation = Quaternion.AngleAxis(_currentSunAngle, Vector3.right);
    }
}
