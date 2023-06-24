using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject instructionsButton;
    [SerializeField] private GameObject returnToMainMenuButton;
    [SerializeField] private GameObject backgroundModel;
    [SerializeField] private GameObject backgroundTitle;
    [SerializeField] private GameObject highscore;

    private bool isInstructionLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuTrack, 0f, 2f);

        LoadMainMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadInstructions()
    {
        isInstructionLoaded = true;

        quitButton.SetActive(false);
        playButton.SetActive(false);
        instructionsButton.SetActive(false);
        returnToMainMenuButton.SetActive(true);
        backgroundModel.SetActive(false);
        backgroundTitle.SetActive(false);
        highscore.SetActive(false);

        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    public void LoadMainMenu()
    {
        quitButton.SetActive(true);
        playButton.SetActive(true);
        instructionsButton.SetActive(true);
        returnToMainMenuButton.SetActive(false);
        backgroundModel.SetActive(true);
        backgroundTitle.SetActive(true);
        highscore.SetActive(true);

        if (isInstructionLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionLoaded = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
