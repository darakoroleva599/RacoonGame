using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FlowerPotMinigameController : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private RectTransform potImage;
    [SerializeField] private float shakeSpeed = 0.5f;
    [SerializeField] private float maxShakeOffset = 30f;
    [SerializeField] private float maxProgress = 100f;
    [SerializeField] private float decaySpeed = 0.2f;

    [Header("UI")]
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject completeEffect;
    [SerializeField] private GameObject instructionText;
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    [Header("Visual Feedback")]
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color inactiveColor = Color.white;

    private float currentProgress = 0f;
    private float shakeTimer = 0f;
    private float shakeDirection = 1f;
    private bool isComplete = false;
    private bool returningToGame = false;
    private Vector2 potStartPosition;

    void Start()
    {
        Time.timeScale = 0;

        potStartPosition = potImage.anchoredPosition;
        currentProgress = 0f;
        shakeTimer = 0f;
        shakeDirection = 1f;
        isComplete = false;
        returningToGame = false;

        if (progressFill != null)
            progressFill.fillAmount = 0f;

        if (completeEffect != null)
            completeEffect.SetActive(false);

        if (instructionText != null)
            instructionText.SetActive(true);

        Debug.Log("FlowerPotMinigameController Start - ЖМИ A и D БЫСТРО!");
    }

    void Update()
    {
        if (isComplete || returningToGame) return;

        bool pressingA = Input.GetKey(KeyCode.A);
        bool pressingD = Input.GetKey(KeyCode.D);

        // Подсветка клавиш
        if (leftArrow != null)
            leftArrow.color = pressingA ? activeColor : inactiveColor;
        if (rightArrow != null)
            rightArrow.color = pressingD ? activeColor : inactiveColor;

        if (pressingA && pressingD)
        {
            return;
        }

        if (pressingA || pressingD)
        {
            // Скрываем подсказку
            if (instructionText != null && instructionText.activeSelf)
                instructionText.SetActive(false);

            // Увеличиваем прогресс
            currentProgress += shakeSpeed;
            currentProgress = Mathf.Min(currentProgress, maxProgress);

            // ТРЯСКА ГОРШКА (ручная анимация)
            float progressRatio = currentProgress / maxProgress;
            float shakeIntensity = Mathf.Lerp(5f, maxShakeOffset, progressRatio);
            float shakeFrequency = Mathf.Lerp(0.05f, 0.02f, progressRatio); // Быстрее при большем прогрессе

            shakeTimer += Time.unscaledDeltaTime;

            if (shakeTimer >= shakeFrequency)
            {
                shakeTimer = 0f;
                shakeDirection *= -1f; // Меняем направление
            }

            // Плавное движение к краю
            float targetX = potStartPosition.x + shakeDirection * shakeIntensity;
            potImage.anchoredPosition = new Vector2(
                Mathf.Lerp(potImage.anchoredPosition.x, targetX, Time.unscaledDeltaTime * 20f),
                potStartPosition.y
            );

            // Лёгкое вращение
            float targetRotation = shakeDirection * shakeIntensity * 0.15f;
            potImage.rotation = Quaternion.Lerp(
                potImage.rotation,
                Quaternion.Euler(0, 0, targetRotation),
                Time.unscaledDeltaTime * 20f
            );

            // Обновляем шкалу
            if (progressFill != null)
                progressFill.fillAmount = currentProgress / maxProgress;

            // Отладка каждые 25%
            if (Mathf.Abs(currentProgress % 25) < shakeSpeed && currentProgress > 0)
            {
                Debug.Log("Прогресс: " + currentProgress.ToString("F0") + "/" + maxProgress + " | Тряска: " + shakeIntensity.ToString("F1"));
            }

            // Завершение ТОЛЬКО при 100%
            if (currentProgress >= maxProgress - 0.01f && !isComplete)
            {
                Debug.Log("!!! ГОРШОК РАСКАЧАН !!!");
                CompleteMinigame();
            }
        }
        else
        {
            // Ничего не нажато — прогресс медленно падает
            if (currentProgress > 0)
            {
                currentProgress -= decaySpeed;
                currentProgress = Mathf.Max(0, currentProgress);

                float progressRatio = currentProgress / maxProgress;
                float shakeIntensity = Mathf.Lerp(0f, maxShakeOffset, progressRatio);

                shakeTimer += Time.unscaledDeltaTime;
                float shakeFrequency = Mathf.Lerp(0.1f, 0.05f, progressRatio);

                if (shakeTimer >= shakeFrequency)
                {
                    shakeTimer = 0f;
                    shakeDirection *= -1f;
                }

                float targetX = potStartPosition.x + shakeDirection * shakeIntensity * 0.3f;
                potImage.anchoredPosition = new Vector2(
                    Mathf.Lerp(potImage.anchoredPosition.x, targetX, Time.unscaledDeltaTime * 10f),
                    potStartPosition.y
                );

                potImage.rotation = Quaternion.Lerp(
                    potImage.rotation,
                    Quaternion.identity,
                    Time.unscaledDeltaTime * 10f
                );

                if (progressFill != null)
                    progressFill.fillAmount = currentProgress / maxProgress;
            }
            else
            {
                // Горшок возвращается в исходное положение
                potImage.anchoredPosition = Vector2.Lerp(
                    potImage.anchoredPosition,
                    potStartPosition,
                    Time.unscaledDeltaTime * 8f
                );
                potImage.rotation = Quaternion.Lerp(
                    potImage.rotation,
                    Quaternion.identity,
                    Time.unscaledDeltaTime * 8f
                );
            }
        }
    }

    private void CompleteMinigame()
    {
        isComplete = true;
        Debug.Log("=== МИНИ-ИГРА ЗАВЕРШЕНА ===");

        if (completeEffect != null)
            completeEffect.SetActive(true);

        FlowerPotData.wasCompleted = true;

        StartCoroutine(WaitAndReturn());
    }

    private IEnumerator WaitAndReturn()
    {
        Debug.Log("Горшок раскачан! +25% раздражения");
        yield return new WaitForSecondsRealtime(1.5f);
        ReturnToGame();
    }

    private void ReturnToGame()
    {
        if (returningToGame) return;
        returningToGame = true;

        Debug.Log("ReturnToGame - включаем время");
        Time.timeScale = 1;

        Debug.Log("Выгружаем сцену FlowerPotMinigame...");
        SceneManager.UnloadSceneAsync("FlowerPotMinigame");
    }

    public void CloseMinigame()
    {
        if (returningToGame) return;
        returningToGame = true;

        Debug.Log("Закрытие мини-игры без завершения");
        FlowerPotData.wasCompleted = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("FlowerPotMinigame");
    }
}