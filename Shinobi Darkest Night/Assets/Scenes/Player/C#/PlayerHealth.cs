using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Player Health")]
    public float playerCourrentHealth;
    public float playerMaxHealth;
    [SerializeField] Image playerHealthSlider;

    void Awake()
    {
        playerCourrentHealth = playerMaxHealth;
        playerHealthSlider.fillAmount = playerCourrentHealth/playerMaxHealth;
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
        playerHealthSlider.fillAmount = playerCourrentHealth / playerMaxHealth;
    }

    public void AddDamage(float Damage)
    {
        GetComponent<PlayerController>().StopHealing();
        playerCourrentHealth -= Damage;
        playerHealthSlider.fillAmount = playerCourrentHealth / playerMaxHealth;

        if (playerCourrentHealth <= 0)
        {
            MakeDead();
        }
    }

    void MakeDead()
    {
        GetComponent<PlayerController>().MakeDeath();
    }
}
