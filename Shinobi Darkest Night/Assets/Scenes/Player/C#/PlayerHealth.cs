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
    private PlayerController playerController;

    void Awake()
    {
        playerCourrentHealth = playerMaxHealth;
        playerHealthSlider.fillAmount = playerCourrentHealth/playerMaxHealth;
        playerController = GetComponent<PlayerController>();
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
        if (playerController.sakuramochi > 0)
        {
            playerCourrentHealth -= Damage/2;
        }
        else
        {
            playerCourrentHealth -= Damage;

        }
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
