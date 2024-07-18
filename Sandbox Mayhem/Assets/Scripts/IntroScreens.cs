using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreens : MonoBehaviour
{
    public GameObject nextIntroScreen;

    public void showNextIntroScreen()
    {
        if (nextIntroScreen)
        {
            gameObject.SetActive(false);
            nextIntroScreen.SetActive(true);
        } else
        {
            // No intro screens to show anymore. Transition to start the game.
            SceneManager.LoadScene("Track_1");
        }
    }
}
