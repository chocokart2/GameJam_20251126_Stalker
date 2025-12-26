using UnityEngine;

public class Player : Creature
{
    protected override void Die()
    {
        Debug.Log("Player has died!");
    }
}
