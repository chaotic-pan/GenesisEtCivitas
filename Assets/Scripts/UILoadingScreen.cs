using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
     [Header("Level To Load")]
     [SerializeField] private string levelName;
     [SerializeField] private string tutorialName;
     
     [Header("Creature feature")]
     [SerializeField] private AnimManager animManager;
     [SerializeField] private GameObject cameraContainer;
     
     [Header("MainMenu")]
     [SerializeField] private GameObject eventSystem;
     [SerializeField] private GameObject mainMenuCamera;
     [SerializeField] private GameObject mainMenu;
     
     [Header("UI")]
     [SerializeField] private Slider loadingSlider;
     [SerializeField] private CanvasGroup loadingPanel;
     [SerializeField] private GameObject creatureFeature;
     
     [SerializeField] private MapFileLocation mapFileLocation;
     
     private bool _isLoading;
     
     public void LoadLevel()
     {
          Destroy(eventSystem);
          
          animManager.SetIsDancing(true);
          _isLoading = true;
          StartCoroutine(LoadLevelAsync(levelName));
     }

     public void LoadTutorial()
     {
          Destroy(eventSystem);
          
          animManager.SetIsDancing(true);
          _isLoading = true;
          StartCoroutine(LoadLevelAsync(tutorialName));
     }
     
     private void Update()
     {
          if (_isLoading)
               cameraContainer.transform.Rotate(new Vector3(0f, 0.1f, 0f));
     }

     IEnumerator LoadLevelAsync(string level)
     {
          // fade in loading screen
          yield return StartCoroutine(FadeIn());
          
          // fake start loading 
          var fadeTimer = 1f;
          while (fadeTimer > 0)
          {
               fadeTimer -= Time.deltaTime;
               loadingSlider.value = .25f + fadeTimer*-.25f;
               yield return null;
          }
          
          // load level additively
          var asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
          
          while (!asyncLoad.isDone)
          {
               loadingSlider.value = .25f + asyncLoad.progress * .5f;
               yield return null;
          }

          loadingSlider.value = .5f;

          // destroy main menu and camera
          Destroy(mainMenu);
          Destroy(mainMenuCamera);

          // fake end timer + gives Civs a sec to initialize fully
          fadeTimer = 2f;
          while (fadeTimer > 0)
          {
               fadeTimer -= Time.deltaTime;
               loadingSlider.value = .75f + fadeTimer*-(.25f/2)+.25f;
               yield return null;
          }
          
          
          // fade out loading screen
          yield return StartCoroutine(FadeOut());
          yield return null;
          
          var asyncUnloadMainMenu = SceneManager.UnloadSceneAsync("MainMenu");
          yield return new WaitUntil(() => asyncUnloadMainMenu.isDone);
     }
     
     IEnumerator FadeIn()
     {
          loadingPanel.gameObject.SetActive(true);
          
          float time = 0f;
          while (time < 1)
          {
               loadingPanel.alpha = time / 1;
               time += Time.deltaTime;
               yield return null;
          }
          loadingPanel.alpha = 1f;
     }
     
     IEnumerator FadeOut()
     {
          float time = 0f;
          while (time < 1)
          {
               loadingPanel.alpha = 1f - (time / 1);
               time += Time.deltaTime;
               yield return null;
          }
          loadingPanel.alpha = 0f;
          _isLoading = false;
     }
}
