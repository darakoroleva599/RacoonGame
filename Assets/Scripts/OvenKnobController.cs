using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OvenKnobController : MonoBehaviour
{
    [SerializeField] private RectTransform knob;
    [SerializeField] private float maxRotation = 180f;
    [SerializeField] private float scrollSensitivity = 3f;
    [SerializeField] private float startRotation = 0f;
    [SerializeField] private bool clockwise = false;

    private float currentRotation = 0f;
    private bool isComplete = false;

    void Start()
    {
        currentRotation = 0f;
        isComplete = false;
        knob.rotation = Quaternion.Euler(0, 0, startRotation);
    }

    void Update()
    {
        if (isComplete) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float direction = clockwise ? 1f : -1f;
            currentRotation += scroll * scrollSensitivity * 100f * direction;
            currentRotation = Mathf.Clamp(currentRotation, 0f, maxRotation);

            float angle = startRotation + currentRotation * direction;
            knob.rotation = Quaternion.Euler(0, 0, angle);

            if (currentRotation >= maxRotation - 0.01f && !isComplete)
            {
                CompleteMinigame();
            }
        }
    }

    private void CompleteMinigame()
    {
        isComplete = true;
        OvenData.wasCompleted = true;
        StartCoroutine(WaitAndReturn());
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.UnloadSceneAsync("OvenKnobminigame");
    }
}