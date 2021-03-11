using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint", menuName = "SO/Checkpoint")]
public class Checkpoint : ScriptableObject
{
    public string sceneName;

    //PlayerPosition
    public Vector2 position;

    //PlayerHealth
    public int currentHealth;
    public int maxHealth;

    //PlayerMana
    public int currentMana;
    public int maxMana;

    //PlayerCoins
    public int numCoins;

    //PlayerAbility
    public bool shield;
    public bool grapple;
}
