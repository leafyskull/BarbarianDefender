using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;


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

    public void OnFriendlyUnitMoved()
    {
        VisionManager.Instance.RecalculateVision();
    }






}
