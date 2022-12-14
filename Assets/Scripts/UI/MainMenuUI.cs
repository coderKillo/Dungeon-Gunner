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

    private bool isInstructionLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuTrack, 0f, 2f);

        LoadCharacterSelectorScene();
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

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelectorScene()
    {
        quitButton.SetActive(true);
        playButton.SetActive(true);
        instructionsButton.SetActive(true);
        returnToMainMenuButton.SetActive(false);

        if (isInstructionLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionLoaded = false;
        }

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
