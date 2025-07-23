using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIGameOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject overlay;
        [SerializeField] private GameObject close;
        [SerializeField] private TextMeshProUGUI title;
        
        private void OnEnable()
        {
            GameEvents.Lifecycle.OnGameEnd += OnGameEnd;
            GameEvents.Lifecycle.OnGamePause += OnGamePause;
        }
        private void OnDisable()
        {
            GameEvents.Lifecycle.OnGameEnd -= OnGameEnd;
            GameEvents.Lifecycle.OnGamePause -= OnGamePause;
        }

        private void OnGamePause()
        {
            Time.timeScale = 0;
            overlay.SetActive(true);
            close.SetActive(true);
            title.text = "PAUSE";
        }
        
        public void ContinueGame()
        {
            Time.timeScale = 1;
            overlay.SetActive(false);
            close.SetActive(false);
        }
        
        private void OnGameEnd()
        {
            Time.timeScale = 0;
            overlay.SetActive(true);
            close.SetActive(false);
            title.text = "GAME OVER";
        }

        public void LeaveGame()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}