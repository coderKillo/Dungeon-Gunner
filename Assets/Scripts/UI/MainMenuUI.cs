using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuButtons;
    [SerializeField] private GameObject returnToMainMenuButton;
    [SerializeField] private GameObject backgroundModel;
    [SerializeField] private GameObject backgroundTitle;
    [SerializeField] private GameObject highscore;

    private bool isInstructionLoaded = false;
    private bool isCardCollectionLoaded = false;

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

        ShowExtras(false);

        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    public void LoadCardCollections()
    {
        isCardCollectionLoaded = true;

        ShowExtras(false);

        SceneManager.LoadScene("CardCollectionScene", LoadSceneMode.Additive);
    }

    public void LoadMainMenu()
    {
        ShowExtras(true);

        if (isInstructionLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionLoaded = false;
        }

        if (isCardCollectionLoaded)
        {
            SceneManager.UnloadSceneAsync("CardCollectionScene");
            isCardCollectionLoaded = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ShowExtras(bool show)
    {
        menuButtons.SetActive(show);
        returnToMainMenuButton.SetActive(!show);
        backgroundModel.SetActive(show);
        backgroundTitle.SetActive(show);
        highscore.SetActive(show);
    }
}
