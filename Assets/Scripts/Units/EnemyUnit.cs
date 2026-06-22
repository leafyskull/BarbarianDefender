using UnityEngine;

public class EnemyUnit : Unit
{
    protected override void Awake()
    {
        team = UnitTeam.Enemy;
        base.Awake();
    }
}