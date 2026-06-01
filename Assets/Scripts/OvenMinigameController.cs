using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OvenMinigameController : MonoBehaviour
{
    [SerializeField] private Image knobHighlight;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private Color pulseColor1 = Color.yellow;
    [SerializeField] private Color pulseColor2 = Color.white;

    private bool waitingForInput = true;
    private bool isTransitioning = false;

    void Start()
    {
        Time.timeScale = 0;
        waitingForInput = true;
        isTransitioning = false;
    }

    void Update()
    {
        if (!waitingForInput || isTransitioning) return;

        if (knobHighlight != null)
        {
            float t = Mathf.Sin(Time.realtimeSinceStartup * pulseSpeed) * 0.5f + 0.5f;
            knobHighlight.color = Color.Lerp(pulseColor1, pulseColor2, t);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            waitingForInput = false;
            isTransitioning = true;
            SceneManager.LoadScene("OvenKnobminiGame", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("OvenMiniGame");
        }
    }
}