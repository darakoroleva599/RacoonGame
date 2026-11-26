using UnityEngine;

public class PhoneFilter : MonoBehaviour
{
    public enum FilterType
    {
        Necklace,
        HatMustache,
        Glasses
    }

    [SerializeField] private FilterType filterType;
    [SerializeField] private PhonePuzzleManager puzzleManager;

    private void OnMouseDown()
    {
        if (puzzleManager != null)
            puzzleManager.SelectFilter(filterType);
    }
}