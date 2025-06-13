using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{

    public void OnStart()
    {
        SceneManager.LoadScene("WorldMap");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
