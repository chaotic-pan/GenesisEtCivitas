using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
     [SerializeField] private AnimManager animManager;
     [SerializeField] private GameObject cameraContainer;
     
     [SerializeField] private GameObject mainMenu;
     [SerializeField] private Slider loadingSlider;

     public void LoadLevel(string levelName) 
     {
          mainMenu.SetActive(false);
          gameObject.SetActive(true);
          
          animManager.SetIsDancing(true);

          StartCoroutine(LoadLevelAsync(levelName));
     }
     
     void Update()
     {
          cameraContainer.transform.Rotate(new Vector3(0f, 0.1f, 0f));
     }

     IEnumerator LoadLevelAsync(string levelName)
     {
          var asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

          while (asyncLoad != null && !asyncLoad.isDone)
          {
               var progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);
               loadingSlider.value = progressValue;
               yield return null;
          }
     }
}
