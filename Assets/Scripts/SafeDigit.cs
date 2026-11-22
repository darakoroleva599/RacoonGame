
using UnityEngine;

public class SafeDigit : MonoBehaviour
{
    [SerializeField] private int digit;
    [SerializeField] private SafePuzzleManager safeManager;

    private void OnMouseDown()
    {
        if (safeManager != null)
            safeManager.EnterDigit(digit);
    }
}
