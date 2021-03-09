﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Health", menuName = "Player/PlayerHealth")]
public class PlayerHealth : ScriptableObject
{
    public int currentHealth;
    public int maxHealth;
}