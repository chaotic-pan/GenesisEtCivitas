using Events;
using Models;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject sun2;
    [SerializeField] private float dayLengthInSeconds = 600f; // 10 irl minutes per day
    [SerializeField] private float speed = 1;
    [SerializeField] private bool debugMode;
    [SerializeField] private GameObject GameOverScreen;
    
    private float _elapsedRealTimeInSeconds;
    private float _currentSunAngle;
    
    private DayNightCycleModel _cycleModel;
    private int _lastUpdatedElapsedMinutes = -1;

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
        GameOverScreen.SetActive(true);
    }

    private void Update()
    {
        if (!_cycleModel.AdvanceCycle)
            return;

        var delta = (Time.deltaTime * speed);
        _elapsedRealTimeInSeconds += delta;
        
        CalculateTime();
        RotateSun(delta);
    }

    private void CalculateTime()
    {
        var elapsedInGameMinutes = (int) (_elapsedRealTimeInSeconds / dayLengthInSeconds * 1440);
        
        if (elapsedInGameMinutes == _lastUpdatedElapsedMinutes) 
            return;
        
        _lastUpdatedElapsedMinutes = elapsedInGameMinutes;
        
        _cycleModel.CurrentInGameMinute = elapsedInGameMinutes % 60;
        _cycleModel.CurrentInGameHour = (elapsedInGameMinutes / 60) % 24;
        _cycleModel.CurrentInGameDay = elapsedInGameMinutes / 60 / 24;
        
        _cycleModel.ElapsedInGameMinutes = elapsedInGameMinutes;
    }

    private void RotateSun(float delta)
    {
        if (debugMode) return;
        
        float deltaAngle = (360f / dayLengthInSeconds) * delta;
        _currentSunAngle += deltaAngle;
        
        if (_currentSunAngle >= 360f) _currentSunAngle -= 360f;

        sun.transform.rotation = Quaternion.AngleAxis(_currentSunAngle + 180f, Vector3.right);
        sun2.transform.rotation = Quaternion.AngleAxis(_currentSunAngle - 90f, Vector3.right);
    }
}
