using UnityEngine;

public class PhoneSelfieButton : MonoBehaviour
{
    [SerializeField] private PhonePuzzleManager puzzleManager;

    private void OnMouseDown()
    {
        if (puzzleManager != null)
            puzzleManager.TakeSelfie();
    }
}