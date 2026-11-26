
using UnityEngine;

public class PhoneDust : MonoBehaviour
{
    [Header("Маршрут")]
    [SerializeField] private Transform returnPoint;
    [SerializeField] private Transform middlePoint;
    [SerializeField] private Transform checkPoint;

    [Header("Пыль")]
    [SerializeField] private GameObject dustCloud;

    private HumanController humanController;

    private static bool prankStarted;

    private bool secondStageStarted;

    private void Start()
    {
        humanController = FindObjectOfType<HumanController>();

        if (dustCloud != null)
        {
            Animator anim = dustCloud.GetComponent<Animator>();

            if (anim != null)
                anim.SetBool("IsActive", false);

            dustCloud.SetActive(false);
        }

        if (!PhonePuzzleManager.WasCompleted || prankStarted)
            return;

        if (humanController == null ||
            returnPoint == null ||
            middlePoint == null ||
            checkPoint == null)
        {
            Debug.LogWarning("[PhoneDust] Не назначены точки маршрута или HumanController.");
            return;
        }

        prankStarted = true;


        humanController.OnCheckPrankFinished += StartSecondStage;

        humanController.CheckPrank(middlePoint, checkPoint, 1f);
    }

    private void StartSecondStage()
    {
        if (secondStageStarted)
            return;

        secondStageStarted = true;

        humanController.OnCheckPrankFinished -= StartSecondStage;


        humanController.OnCheckPrankStopped += ShowDust;
        humanController.OnCheckPrankLeaving += HideDust;
        humanController.OnCheckPrankFinished += HideDust;

        humanController.CheckPrank(checkPoint, returnPoint, 3f);
    }

    private void ShowDust()
    {
        if (dustCloud == null)
            return;

        dustCloud.SetActive(true);

        Animator anim = dustCloud.GetComponent<Animator>();

        if (anim != null)
        {
            anim.SetBool("IsActive", true);
            anim.speed = 0.7f;
        }
    }

    private void HideDust()
    {
        if (dustCloud != null)
        {
            Animator anim = dustCloud.GetComponent<Animator>();

            if (anim != null)
                anim.SetBool("IsActive", false);

            dustCloud.SetActive(false);
        }

        if (humanController != null)
        {
            humanController.OnCheckPrankStopped -= ShowDust;
            humanController.OnCheckPrankLeaving -= HideDust;
            humanController.OnCheckPrankFinished -= HideDust;
        }
    }
}
