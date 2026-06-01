using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class FridgeMinigameController : MonoBehaviour
{
    [SerializeField] private DragHandler item1;
    [SerializeField] private DragHandler item2;
    [SerializeField] private GameObject pepper;
    [SerializeField] private float dragThreshold = 50f;

    private bool pepperRevealed = false;
    private bool pepperClicked = false;
    private bool returningToGame = false;

    void Start()
    {
        Time.timeScale = 0;

        if (pepper != null)
        {
            Button pepperButton = pepper.GetComponent<Button>();
            if (pepperButton == null)
            {
                pepperButton = pepper.AddComponent<Button>();
                pepperButton.targetGraphic = pepper.GetComponent<Image>();
            }
            pepperButton.onClick.AddListener(OnPepperClicked);
        }

        item1.OnMoved += CheckItemsPosition;
        item2.OnMoved += CheckItemsPosition;
    }

    void Update()
    {
        if (returningToGame) return;

        if (pepperRevealed && !pepperClicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;
                RectTransform pepperRect = pepper.GetComponent<RectTransform>();

                if (RectTransformUtility.RectangleContainsScreenPoint(pepperRect, mousePos))
                {
                    OnPepperClicked();
                }
            }
        }
    }

    private void CheckItemsPosition()
    {
        if (pepperRevealed) return;

        float dist1 = Vector2.Distance(item1.GetComponent<RectTransform>().anchoredPosition, item1.GetStartPosition());
        float dist2 = Vector2.Distance(item2.GetComponent<RectTransform>().anchoredPosition, item2.GetStartPosition());

        if (dist1 > dragThreshold && dist2 > dragThreshold)
        {
            pepperRevealed = true;
        }
    }

    public void OnPepperClicked()
    {
        if (pepperClicked) return;
        pepperClicked = true;

        FridgeData.pepperTaken = true;

        StartCoroutine(WaitAndReturn());
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ReturnToGame();
    }

    private void ReturnToGame()
    {
        if (returningToGame) return;
        returningToGame = true;

        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("FridgeMinigame");
    }

    public void CloseMinigame()
    {
        if (returningToGame) return;
        returningToGame = true;

        FridgeData.pepperTaken = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("FridgeMinigame");
    }
}