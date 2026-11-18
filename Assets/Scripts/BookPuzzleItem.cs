
using UnityEngine;

public class BookPuzzleItem : MonoBehaviour
{
    [Header("Настройки книги")]
    public int bookId;

    private BookshelfPuzzleManager puzzleManager;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(BookshelfPuzzleManager manager)
    {
        puzzleManager = manager;
    }

    private void OnMouseDown()
    {
        if (puzzleManager != null)
        {
            puzzleManager.SelectBook(this);
        }
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected
            ? originalScale * 1.15f
            : originalScale;
    }

    public int GetBookId()
    {
        return bookId;
    }
}
