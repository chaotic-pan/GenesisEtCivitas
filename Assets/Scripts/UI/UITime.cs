using Events;
using Models;
using TMPro;
using UnityEngine;

namespace UI
{
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
            minuteText.text = data.CurrentInGameMinute.ToString("00");
            hourText.text = data.CurrentInGameHour.ToString("00");
            dayText.text = data.CurrentInGameDay.ToString();
        }
    }
}
