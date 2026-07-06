using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    public bool isInitialized { get; private set; }


    // Awake(): Sets instance reference
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize()
    {
        isInitialized = true;
    }

    public void OnFriendlyUnitMoved()
    {
        VisionManager.Instance.RecalculateVision();
    }






}
