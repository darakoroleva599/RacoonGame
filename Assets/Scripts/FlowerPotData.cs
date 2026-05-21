using UnityEngine;

public class FlowerPotData : MonoBehaviour
{
    public static FlowerPotData Instance { get; private set; }

    [HideInInspector] public float annoyanceAmount = 25f;
    [HideInInspector] public Vector3 potPosition;
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