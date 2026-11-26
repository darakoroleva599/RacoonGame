using UnityEngine;
using UnityEngine.SceneManagement;

public class BookshelfPuzzleManager : MonoBehaviour
{
    [Header("Книги в порядке расположения по местам")]
    public BookPuzzleItem[] booksInSlots = new BookPuzzleItem[6];

    [Header("Правильная последовательность ID")]
    public int[] correctOrder = new int[6];

    [Header("Кнопка выхода")]
    public GameObject exitButton;

    public static bool WasCompleted { get; set; }

    private BookPuzzleItem selectedBook;
    private int selectedSlot = -1;
    private bool puzzleCompleted;

    private void Start()
    {
        for (int i = 0; i < booksInSlots.Length; i++)
        {
            if (booksInSlots[i] != null)
                booksInSlots[i].Initialize(this);
        }

        if (exitButton != null)
            exitButton.SetActive(false);

        CheckSolution();
    }

    public void SelectBook(BookPuzzleItem clickedBook)
    {
        if (puzzleCompleted || clickedBook == null)
            return;

        int clickedSlot = System.Array.IndexOf(booksInSlots, clickedBook);

        if (clickedSlot < 0)
            return;

        if (selectedBook == null)
        {
            selectedBook = clickedBook;
            selectedSlot = clickedSlot;
            selectedBook.SetSelected(true);
            return;
        }

        if (selectedBook == clickedBook)
        {
            selectedBook.SetSelected(false);
            selectedBook = null;
            selectedSlot = -1;
            return;
        }

        BookPuzzleItem secondBook = clickedBook;

        Vector3 firstPosition = selectedBook.transform.position;
        Vector3 secondPosition = secondBook.transform.position;

        selectedBook.transform.position = secondPosition;
        secondBook.transform.position = firstPosition;

        booksInSlots[selectedSlot] = secondBook;
        booksInSlots[clickedSlot] = selectedBook;

        selectedBook.SetSelected(false);
        selectedBook = null;
        selectedSlot = -1;

        CheckSolution();
    }

    private void CheckSolution()
    {
        if (booksInSlots.Length != correctOrder.Length)
            return;

        for (int i = 0; i < booksInSlots.Length; i++)
        {
            if (booksInSlots[i] == null ||
                booksInSlots[i].GetBookId() != correctOrder[i])
            {
                return;
            }
        }

        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (puzzleCompleted)
            return;

        puzzleCompleted = true;
        WasCompleted = true;

        HumanController.AddAnnoyanceAfterSceneLoad(33f);
        QuestManager.RegisterQuestCompleted("Bookshelf");

        if (exitButton != null)
            exitButton.SetActive(true);
    }

    public void ExitPuzzle()
    {
        if (!puzzleCompleted)
            return;

        SceneManager.LoadScene("LivingRoom");
    }
}
