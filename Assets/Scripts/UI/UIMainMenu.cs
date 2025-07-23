using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private UILoadingScreen loadingScreen;
    [SerializeField] private TMP_InputField mapInput;
    [SerializeField] private TextMeshProUGUI placeholderText;
    private string path;
    [SerializeField] private MapFileLocation mapFileLocation;

    public void OnStart()
    {
        if (path == null)
        {
            placeholderText.fontStyle = FontStyles.Bold;
            placeholderText.color = Color.red;
            return;
        }
        mapFileLocation.MapLocation = path;
        mapFileLocation.isBuild = true;
        mapInput.text = "";
        loadingScreen.LoadLevel();
    }

    public void PlayTutorial()
    {
        // TODO fix
        var tutLoc = mapInput.text.Split("\\").ToList();
        tutLoc.RemoveAt(tutLoc.Count - 1);
        tutLoc.Add("file_new_5.txt");
        
        loadingScreen.LoadTutorial();
    }

    public void onOpenFile()
    {
        var filePath = EditorUtility.OpenFilePanel("Select World", "Desktop", "world");
        path = filePath;
        var filename = filePath.Split("/");
        mapInput.text = filename[^1];
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
