using UnityEngine;

public class FriendlyRangedUnit : FriendlyUnit
{
    protected override void Awake()
    {
        maxHealth = 8;
        movePoints = 2;
        attackRange = 2;
        attackStrength = 3;
        defendStrength = 1;

        base.Awake();
    }
}