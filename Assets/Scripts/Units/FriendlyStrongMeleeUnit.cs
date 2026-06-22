using UnityEngine;

public class FriendlyStrongMeleeUnit : FriendlyUnit
{
    protected override void Awake()
    {
        maxHealth = 15;
        movePoints = 2;
        attackRange = 1;
        attackStrength = 5;
        defendStrength = 3;

        base.Awake();
    }
}