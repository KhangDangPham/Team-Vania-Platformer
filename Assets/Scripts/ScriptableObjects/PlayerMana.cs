using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Mana", menuName = "Player/PlayerMana")]
public class PlayerMana : ScriptableObject
{
    public int currentMana;
    public int maxMana;
}
