using System;
using System.Linq;
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

    public void PlayTutorial()
    {
        var tutLoc = mapInput.text.Split("\\").ToList();
        tutLoc.RemoveAt(tutLoc.Count - 1);
        tutLoc.Add("file_new_5.txt");
        
        mapFileLocation.MapLocation = String.Join("\\", tutLoc.ToArray());
        loadingScreen.LoadTutorial();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
