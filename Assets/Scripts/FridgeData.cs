using UnityEngine;

public class FridgeData : MonoBehaviour
{
    public static FridgeData Instance { get; private set; }

    [HideInInspector] public static bool pepperTaken = false;
    [HideInInspector] public float annoyanceAmount = 25f;
    [HideInInspector] public Vector3 fridgePosition;

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