using UnityEngine;

public class FriendlyWeakMeleeUnit : FriendlyUnit
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