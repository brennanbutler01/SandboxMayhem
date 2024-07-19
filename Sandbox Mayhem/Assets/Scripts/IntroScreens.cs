using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IntroScreens : MonoBehaviour
{
    public GameObject nextIntroScreen;

    public void showNextIntroScreen()
    {
        if (nextIntroScreen)
        {
            gameObject.SetActive(false);
            nextIntroScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(nextIntroScreen.transform.Find("Button").gameObject);
        } else
        {
            // No intro screens to show anymore. Transition to start the game.
            SceneManager.LoadScene("Track_1");
        }
    }
}
