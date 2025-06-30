using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private UILoadingScreen loadingScreen;

    public void OnStart()
    {
        loadingScreen.LoadLevel("WorldMap");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
