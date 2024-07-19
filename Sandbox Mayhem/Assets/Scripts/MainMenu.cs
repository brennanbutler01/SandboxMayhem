using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;
    public GameObject optionsScreen;
    public GameObject creditsScreen;
    public GameObject firstIntroScreen;

    // Start is called before the first frame update
    void Start()
    {
        optionsScreen.SetActive(false);
        CloseCredits();
    }

    public void StartGame()
    {
        //SceneManager.LoadScene(firstLevel);
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
