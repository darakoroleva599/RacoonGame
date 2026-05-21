using UnityEngine;

public class OvenData : MonoBehaviour
{
    public static OvenData Instance { get; private set; }

    [HideInInspector] public float annoyanceAmount = 25f;
    [HideInInspector] public Vector3 ovenPosition;
    [HideInInspector] public static bool wasCompleted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}