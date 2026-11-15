using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        PlayerMove player = FindObjectOfType<PlayerMove>();

        if (player != null && player.IsCrawling)
            return;

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveProgress();

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
