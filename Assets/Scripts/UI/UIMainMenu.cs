using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private UILoadingScreen loadingScreen;
    [SerializeField] private TMP_InputField mapInput;
    [SerializeField] private MapFileLocation mapFileLocation;

    public void OnStart()
    {
        mapFileLocation.MapLocation = "";
        loadingScreen.LoadLevel();
    }
    
    public void UpdateMapLoc()
    {
        mapFileLocation.MapLocation = mapInput.text;
        mapInput.text = "";
        loadingScreen.LoadLevel();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
