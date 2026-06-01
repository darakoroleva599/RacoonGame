using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PieMinigameController : MonoBehaviour
{
    [SerializeField] private GameObject normalPie;
    [SerializeField] private GameObject spoiledPie;
    [SerializeField] private Button spoilButton;

    private bool isSpoiled = false;
    private bool returningToGame = false;

    void Start()
    {
        Time.timeScale = 0;

        if (normalPie != null)
            normalPie.SetActive(true);
        if (spoiledPie != null)
            spoiledPie.SetActive(false);
        if (spoilButton != null)
            spoilButton.onClick.AddListener(OnSpoilButtonClicked);
    }

    private void OnSpoilButtonClicked()
    {
        if (isSpoiled) return;
        isSpoiled = true;

        if (normalPie != null)
            normalPie.SetActive(false);
        if (spoiledPie != null)
            spoiledPie.SetActive(true);

        if (spoilButton != null)
            spoilButton.gameObject.SetActive(false);

        PieData.wasCompleted = true;

        StartCoroutine(WaitAndReturn());
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSecondsRealtime(1f);
        ReturnToGame();
    }

    private void ReturnToGame()
    {
        if (returningToGame) return;
        returningToGame = true;

        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PieMinigame");
    }

    public void CloseMinigame()
    {
        if (returningToGame) return;
        returningToGame = true;

        PieData.wasCompleted = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PieMinigame");
    }
}