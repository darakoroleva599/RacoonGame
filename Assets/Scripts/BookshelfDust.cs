
using UnityEngine;

public class BookshelfDust : MonoBehaviour
{
    [SerializeField] private Transform returnPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private GameObject dustCloud;

    private HumanController humanController;

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

        // Если головоломка со стеллажом решена
        if (BookshelfPuzzleManager.WasCompleted)
        {
            BookshelfPuzzleManager.WasCompleted = false;

            if (humanController != null)
            {
                humanController.OnCheckPrankStopped += ShowDust;
                humanController.OnCheckPrankLeaving += HideDust;
                humanController.OnCheckPrankFinished += HideDust;

                humanController.CheckPrank(checkPoint, returnPoint, 3f);
            }
        }
    }

    private void ShowDust()
    {
        if (dustCloud == null) return;

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
