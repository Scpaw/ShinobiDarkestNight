using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Player Health")]
    public float playerCourrentHealth;
    public float playerMaxHealth;
    [SerializeField] Slider playerHealthSlider;

    void Awake()
    {
        playerCourrentHealth = 3*playerMaxHealth/4;
        //playerCourrentHealth = playerMaxHealth;
        playerHealthSlider.maxValue = playerMaxHealth;
        playerHealthSlider.value = playerCourrentHealth;
    }

    public void AddHealth(float Health)
    {
        playerCourrentHealth += Health;
        if (playerCourrentHealth > playerMaxHealth) 
        { 
            playerCourrentHealth = playerMaxHealth;

            if (GetComponent<PlayerController>().isHealing)
            {
                GetComponent<PlayerController>().StopHealing();
            }

        }
        playerHealthSlider.value = playerCourrentHealth;
    }

    public void AddDamage(float Damage)
    {
        GetComponent<PlayerController>().StopHealing();
        playerCourrentHealth -= Damage;
        playerHealthSlider.value = playerCourrentHealth;

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
