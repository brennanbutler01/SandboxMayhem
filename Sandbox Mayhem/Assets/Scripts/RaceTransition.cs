using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceTransition : MonoBehaviour
{

    public void leaderboardNextButton()
    {
        print("Test");
        SceneManager.LoadScene("Track_1");
        Time.timeScale = 1f;
    }
}
