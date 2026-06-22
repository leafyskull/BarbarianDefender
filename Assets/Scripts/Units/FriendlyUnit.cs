using UnityEngine;

public class FriendlyUnit : Unit
{
    protected override void Awake()
    {
        team = UnitTeam.Friendly;
        base.Awake();
    }
}