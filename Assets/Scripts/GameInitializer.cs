using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance;

    [SerializeField] GridManager gridManager;
    [SerializeField] GameFlowManager gameFlowManager;
    [SerializeField] UnitSpawner unitSpawner;
    [SerializeField] VisionManager visionManager;
    [SerializeField] UnitSelectionManager unitSelectionManager;
    [SerializeField] UnitManager unitManager;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // First, initialize everything
        gameFlowManager.Initialize();
        gridManager.Initialize();
        unitManager.Initialize();
        unitSelectionManager.Initialize();
        unitSpawner.Initialize();
        visionManager.Initialize();

        // Then, run functions to start game setup
        
        // GameFlowManager - nothing ATM
        gridManager.DoSetup();
        // UnitManager - nothing ATM
        unitSelectionManager.DoSetup();
        // UnitSpawner - nothing ATM
        visionManager.RecalculateVision();
    }
}
