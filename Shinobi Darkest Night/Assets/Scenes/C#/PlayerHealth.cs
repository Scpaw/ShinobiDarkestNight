using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Player Health")]
    [SerializeField] float playerCourrentHealth;
    [SerializeField] float playerMaxHealth;
    [SerializeField] Slider playerHealthSlider;

    bool damaged = false;

    void Awake()
    {
        playerCourrentHealth = playerMaxHealth;
        playerHealthSlider.maxValue = playerMaxHealth;
        playerHealthSlider.value = playerCourrentHealth;
    }

    public void AddHealth(float Health)
    {
        playerCourrentHealth += Health;
        if (playerCourrentHealth > playerMaxHealth) 
        { 
            playerCourrentHealth = playerMaxHealth; 
        }
        playerHealthSlider.value = playerCourrentHealth;
    }

    public void AddDamage(float Damage)
    {
        playerCourrentHealth -= Damage;
        playerHealthSlider.value = playerCourrentHealth;
        damaged = true;

        if (playerCourrentHealth <= 0)
        {
            MakeDead();
        }
    }

     void MakeDead()
    {
        Destroy(gameObject, 0);
    }
}
