using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{

    public void OnStart()
    {
        SceneManager.LoadScene("DJTestScene2");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
