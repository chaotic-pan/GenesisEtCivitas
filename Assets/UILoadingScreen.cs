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
     
     private bool _isLoading;
     
     public void LoadLevel()
     {
          Destroy(eventSystem);
          
          animManager.SetIsDancing(true);
          _isLoading = true;
          StartCoroutine(LoadLevelAsync());
     }

     private void Update()
     {
          if (_isLoading)
               cameraContainer.transform.Rotate(new Vector3(0f, 0.1f, 0f));
     }

     IEnumerator LoadLevelAsync()
     {
          // fade in loading screen
          yield return StartCoroutine(FadeIn());
          
          // load level additively
          var asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
          
          while (asyncLoad.progress < 0.9f)
          {
               loadingSlider.value = asyncLoad.progress / 0.9f;
               yield return null;
          }

          loadingSlider.value = 1f;
          
          yield return new WaitUntil(() => asyncLoad.isDone);
          
          // destroy main menu and camera
          Destroy(mainMenu);
          Destroy(mainMenuCamera);
          
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
