using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]

public class PauseMenuScript : MonoBehaviour
{
    private string currentScene;
    private UnityEngine.SceneManagement.Scene scene;
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    public GameObject optionsScreen;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        scene = SceneManager.GetActiveScene();
        optionsScreen.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();

        if (canvasGroup == null)
            Debug.LogError("PauseMenuScript must be associated with a CanvasGroup");
    }

    public void Continue()
    {
        if (canvasGroup.interactable)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
            Time.timeScale = 1f;
        }
    }

    public void ResetCar()
    {
        gameManager.resetCarPositionToWaypoint();
        Continue();
    }

    public void RestartRace()
    {
        SceneManager.LoadScene(scene.name);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
