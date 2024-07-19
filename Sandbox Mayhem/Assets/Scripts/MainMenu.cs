using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    public GameObject menuStart;
    public GameObject optionsStart;
    public GameObject creditsStart;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI.SetActive(true);
        optionsScreen.SetActive(false);
        storyCanvas.SetActive(false);
        creditsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(menuStart);
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
        EventSystem.current.SetSelectedGameObject(firstIntroScreen.transform.Find("Button").gameObject);
    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsStart);
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(menuStart);
    }

    public void OpenCredits()
    {
        creditsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditsStart);
    }

    public void CloseCredits()
    {
        creditsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(menuStart);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}