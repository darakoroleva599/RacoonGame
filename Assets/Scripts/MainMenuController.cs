using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        Time.timeScale = 1;

        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);

        bool canContinue = SaveManager.Instance != null && SaveManager.Instance.gameStarted;
        continueButton.interactable = canContinue;
        continueButton.gameObject.SetActive(canContinue);

        if (canContinue)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }
    }

    private void StartGame()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.ResetProgress();
        Time.timeScale = 1;
        SceneManager.LoadScene("Kitchen");
    }

    private void ContinueGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Kitchen");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}