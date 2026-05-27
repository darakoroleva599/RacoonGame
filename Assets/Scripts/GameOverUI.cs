using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private HumanController humanController;

    void Start()
    {
        Time.timeScale = 1;

        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (humanController == null)
            humanController = FindObjectOfType<HumanController>();

        if (humanController != null)
            humanController.OnCaughtRaccoon += ShowGameOver;
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1;

        if (humanController != null)
            humanController.OnCaughtRaccoon -= ShowGameOver;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (humanController != null)
            humanController.OnCaughtRaccoon -= ShowGameOver;

        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitGame);
    }
}