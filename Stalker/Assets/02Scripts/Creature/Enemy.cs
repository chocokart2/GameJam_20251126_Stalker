using UnityEngine;

public class Enemy : Creature
{
    protected override void Die()
    {
        Debug.Log("Enemy Dead");
    }
}
