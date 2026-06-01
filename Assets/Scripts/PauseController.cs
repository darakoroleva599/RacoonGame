using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveProgress();

            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}