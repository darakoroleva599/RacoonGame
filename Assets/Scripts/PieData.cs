using UnityEngine;

public class PieData : MonoBehaviour
{
    public static PieData Instance { get; private set; }

    [HideInInspector] public static bool wasCompleted = false;
    [HideInInspector] public float annoyanceAmount = 25f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}