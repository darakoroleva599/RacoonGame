using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhonePuzzleManager : MonoBehaviour
{
    public static bool WasCompleted = false;

    [Header("Raccoon Filters")]
    [SerializeField] private GameObject necklaceFilter;
    [SerializeField] private GameObject hatMustacheFilter;
    [SerializeField] private GameObject glassesFilter;

    [Header("Selected Filter Indicators")]
    [SerializeField] private GameObject necklaceSelected;
    [SerializeField] private GameObject hatSelected;
    [SerializeField] private GameObject glassesSelected;

    [Header("Selfie Flash")]
    [SerializeField] private GameObject flash;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float returnDelay = 0.7f;

    private bool selfieTaken = false;

    private void Start()
    {
        SetFilterObjects(false, false, false);
        SetIndicators(false, false, false);

        if (flash != null)
            flash.SetActive(false);
    }

    public void SelectFilter(PhoneFilter.FilterType filter)
    {
        if (selfieTaken)
            return;

        switch (filter)
        {
            case PhoneFilter.FilterType.Necklace:
                SetFilterObjects(true, false, false);
                SetIndicators(true, false, false);
                break;

            case PhoneFilter.FilterType.HatMustache:
                SetFilterObjects(false, true, false);
                SetIndicators(false, true, false);
                break;

            case PhoneFilter.FilterType.Glasses:
                SetFilterObjects(false, false, true);
                SetIndicators(false, false, true);
                break;
        }
    }

    private void SetFilterObjects(bool necklace, bool hat, bool glasses)
    {
        if (necklaceFilter != null)
            necklaceFilter.SetActive(necklace);

        if (hatMustacheFilter != null)
            hatMustacheFilter.SetActive(hat);

        if (glassesFilter != null)
            glassesFilter.SetActive(glasses);
    }

    private void SetIndicators(bool necklace, bool hat, bool glasses)
    {
        if (necklaceSelected != null)
            necklaceSelected.SetActive(necklace);

        if (hatSelected != null)
            hatSelected.SetActive(hat);

        if (glassesSelected != null)
            glassesSelected.SetActive(glasses);
    }

    public void TakeSelfie()
    {
        if (selfieTaken)
            return;

        selfieTaken = true;
        StartCoroutine(SelfieRoutine());
    }

    private IEnumerator SelfieRoutine()
    {
        if (flash != null)
            flash.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        if (flash != null)
            flash.SetActive(false);

        yield return new WaitForSeconds(returnDelay);

        WasCompleted = true;

        HumanController.AddAnnoyanceAfterSceneLoad(33f);
        QuestManager.RegisterQuestCompleted("Phone");

        SceneManager.LoadScene("LivingRoom");
    }
}
