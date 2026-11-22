using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SafePuzzleManager : MonoBehaviour
{
    [Header("Safe Sprites")]
    [SerializeField] private SpriteRenderer safeBackground;
    [SerializeField] private Sprite safeNormalSprite;
    [SerializeField] private Sprite safeWrongSprite;
    [SerializeField] private Sprite safeCorrectSprite;

    [Header("Password")]
    [SerializeField] private string correctPassword = "7923512";
    [SerializeField] private int maxDigits = 7;
    [SerializeField] private Text codeText;


    [Header("Document")]
    [SerializeField] private GameObject document;
    [SerializeField] private Sprite documentNormalSprite;
    [SerializeField] private Sprite documentCorruptedSprite;
    [SerializeField] private Button corruptDocumentButton;


    public static bool WasCompleted { get; private set; }


    private string enteredCode = "";
    private bool safeUnlocked = false;
    private bool documentCorrupted = false;
    private bool wrongCodeShown = false;
    private SpriteRenderer documentRenderer;


    private void Start()
    {
        WasCompleted = false;
        if (document != null)
            documentRenderer = document.GetComponent<SpriteRenderer>();
        ResetSafe();
    }


    public void EnterDigit(int digit)
    {
        if (safeUnlocked || documentCorrupted)
            return;

        if (wrongCodeShown)
        {
            enteredCode = "";
            wrongCodeShown = false;
            if (safeBackground != null)
                safeBackground.sprite = safeNormalSprite;
        }

        if (enteredCode.Length >= maxDigits)
            return;

        enteredCode += digit.ToString();
        UpdateCodeText();

        if (enteredCode.Length >= correctPassword.Length)
            CheckPassword();
    }


    private void CheckPassword()
    {
        if (enteredCode == correctPassword)
            UnlockSafe();
        else
            ShowWrongCode();
    }


    private void ShowWrongCode()
    {
        wrongCodeShown = true;
        if (safeBackground != null)
            safeBackground.sprite = safeWrongSprite;
    }


    private void UnlockSafe()
    {
        safeUnlocked = true;
        if (safeBackground != null)
            safeBackground.sprite = safeCorrectSprite;

        if (document != null)
        {
            Invoke(nameof(ActivateDocument), 1f);
            if (documentRenderer != null)
                documentRenderer.sprite = documentNormalSprite;
        }

        if (corruptDocumentButton != null)
            Invoke(nameof(ShowCorruptButton), 1f);
    }


    private void ShowCorruptButton()
    {
        corruptDocumentButton.gameObject.SetActive(true);
    }


    private void ActivateDocument()
    {
        document.SetActive(true);
    }


    public void CorruptDocument()
    {
        if (!safeUnlocked || documentCorrupted)
            return;

        documentCorrupted = true;
        WasCompleted = true;

        if (documentRenderer != null)
            documentRenderer.sprite = documentCorruptedSprite;

        if (corruptDocumentButton != null)
            corruptDocumentButton.gameObject.SetActive(false);

        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("LivingRoom");
    }


    private void UpdateCodeText()
    {
        if (codeText != null)
            codeText.text = enteredCode;
    }


    private void ResetSafe()
    {
        enteredCode = "";
        safeUnlocked = false;
        documentCorrupted = false;
        wrongCodeShown = false;
        if (safeBackground != null)
            safeBackground.sprite = safeNormalSprite;
        if (codeText != null)
            codeText.text = "";
        if (document != null)
        {
            document.SetActive(false);
            if (documentRenderer != null)
                documentRenderer.sprite = documentNormalSprite;
        }
        if (corruptDocumentButton != null)
            corruptDocumentButton.gameObject.SetActive(false);
    }
}
