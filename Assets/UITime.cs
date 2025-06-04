using Events;
using Models;
using TMPro;
using UnityEngine;

public class UITime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI minuteText;
    [SerializeField] private TextMeshProUGUI hourText;
    [SerializeField] private TextMeshProUGUI dayText;

    private void Awake()
    {
        GameEvents.DayNightCycle.OnDayNightCycleUpdate += UpdateData;
    }

    private void UpdateData(DayNightCycleModel data)
    {
        minuteText.text = data.CurrentInGameMinute.ToString();
        hourText.text = data.CurrentInGameHour.ToString();
        dayText.text = data.CurrentInGameDay.ToString();
    }
}
