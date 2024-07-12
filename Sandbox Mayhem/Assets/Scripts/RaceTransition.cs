using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RaceTransition : MonoBehaviour
{
    public TMP_Text button_label;
    private Scene scene;

    private static GameManager gameManager;

    public void Awake()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "Track_1")
        {
            button_label.text = "Next Race";
        }
        else if (scene.name == "Track_2")
        {
            button_label.text = "Main Menu";
        }
    }

    public void leaderboardNextButton()
    {
        if (scene.name == "Track_1")
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
                Debug.Log("Found Game Manager");
            }
            else
            {
                Debug.Log("Game Manager Already Exists");
            }

            button_label.text = "Next Race";
            SceneManager.LoadScene("Track_2");
            Time.timeScale = 1f;
        }
        else if (scene.name == "Track_2")
        {
            button_label.text = "Main Menu";
            SceneManager.LoadScene("Main Menu");
            Time.timeScale = 1f;
        }

    }
}
