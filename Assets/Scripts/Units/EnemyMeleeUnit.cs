using UnityEngine;

public class EnemyMeleeUnit : EnemyUnit
{
    protected override void Awake()
    {
        maxHealth = 10;
        movePoints = 2;
        attackRange = 1;
        attackStrength = 3;
        defendStrength = 1;

        base.Awake();
    }
}