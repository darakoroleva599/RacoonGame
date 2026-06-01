using UnityEngine;

public class PepperDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private static PepperDisplay instance;
    private static bool isVisible = false;

    void Awake()
    {
        instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void OnEnable()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = isVisible;
    }

    public static void Show()
    {
        isVisible = true;
        if (instance != null && instance.spriteRenderer != null)
            instance.spriteRenderer.enabled = true;
    }

    public static void Hide()
    {
        isVisible = false;
        if (instance != null && instance.spriteRenderer != null)
            instance.spriteRenderer.enabled = false;
    }

    public static bool IsVisible()
    {
        return isVisible;
    }
}