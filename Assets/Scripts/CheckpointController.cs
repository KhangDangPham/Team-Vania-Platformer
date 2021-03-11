using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public Checkpoint checkpoint;
    public PlayerHealth playerHealth;
    public PlayerMana playerMana;
    public PlayerCoins playerCoins;
    public PlayerAbility playerAbility;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            checkpoint.sceneName = SceneManager.GetActiveScene().name;

            checkpoint.position = transform.position;

            checkpoint.currentHealth = playerHealth.currentHealth;
            checkpoint.maxHealth = playerHealth.maxHealth;

            checkpoint.currentMana = playerMana.currentMana;
            checkpoint.maxMana = playerMana.maxMana;

            checkpoint.numCoins = playerCoins.numCoins;

            checkpoint.shield = playerAbility.shield;
            checkpoint.grapple = playerAbility.grapple;
        }
    }
}
