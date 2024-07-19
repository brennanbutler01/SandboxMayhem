using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Xml;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;
    public GameObject optionsScreen;
    public GameObject mainMenuUI;
    public GameObject storyCanvas;
    public GameObject creditsScreen;
    public GameObject firstIntroScreen;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI.SetActive(true);
        optionsScreen.SetActive(false);
        storyCanvas.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(firstLevel);
        CloseCredits();
    }

    public void StartGame()
    {

        storyCanvas.SetActive(true);
        mainMenuUI.SetActive(false);
        firstIntroScreen.SetActive(true);
        

    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsScreen.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}