using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private HumanController humanController;

    void Start()
    {
        // ПРИНУДИТЕЛЬНЫЙ СБРОС ВРЕМЕНИ
        Time.timeScale = 1;

        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);

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
        Debug.Log("РЕСТАРТ ИГРЫ");
        Time.timeScale = 1;

        // Отписываемся перед перезагрузкой
        if (humanController != null)
            humanController.OnCaughtRaccoon -= ShowGameOver;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void OnDestroy()
    {
        if (humanController != null)
            humanController.OnCaughtRaccoon -= ShowGameOver;
    }
}