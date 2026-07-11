using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Data", menuName = "Game/Terrain Data")]

public class TerrainData : ScriptableObject
{
    public TerrainType terrainType;

    [Header("Movement")]
    public int movementCost = 1;

    [Header("Combat")]
    public int defenseBonus = 0;
    public bool canAttackFrom = true;

    [Header("Vision")]
    public bool blocksVision = false;
    public int visionHeight = 0;

    [Header("Visuals")]
    public Sprite sprite;
}