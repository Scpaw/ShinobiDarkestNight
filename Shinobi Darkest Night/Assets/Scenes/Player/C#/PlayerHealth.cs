using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Player Health")]
    public float playerCourrentHealth;
    public float addHp;
    public float playerMaxHealth;
    [SerializeField] Image playerHealthSlider;
    private Text healthText;
    private PlayerController playerController;

    void Awake()
    {
        playerCourrentHealth = playerMaxHealth;
        playerHealthSlider.fillAmount = playerCourrentHealth/playerMaxHealth;
        playerController = GetComponent<PlayerController>();
    }

    public void DoHealth()
    {
        if (playerCourrentHealth < playerMaxHealth && addHp > 0)
        {
            playerCourrentHealth += 13*Time.deltaTime;
            addHp -= 13*Time.deltaTime;
            playerHealthSlider.fillAmount = playerCourrentHealth / playerMaxHealth; healthText.text = ((int)playerCourrentHealth).ToString();
        }
        else if (playerCourrentHealth > playerMaxHealth)
        {
            playerCourrentHealth = playerMaxHealth;
            addHp = 0;
        }
    }

    public void AddHealth(float Health)
    {
        addHp += Health;
        healthText.text = ((int)playerCourrentHealth).ToString();
        if (playerCourrentHealth > playerMaxHealth) 
        { 
            playerCourrentHealth = playerMaxHealth;

            if (GetComponent<PlayerController>().isHealing)
            {
                GetComponent<PlayerController>().StopHealing();
            }
        }
        playerHealthSlider.fillAmount = playerCourrentHealth / playerMaxHealth;
        AkSoundEngine.PostEvent("Player_Healing_Drinking", gameObject);
    }

    public void AddDamage(float Damage)
    {
        if (!playerController.godMode)
        {
            GetComponent<PlayerController>().StopHealing();
            if (playerController.sakuramochi > 0)
            {
                playerCourrentHealth -= Damage / 2;
            }
            else
            {
                playerCourrentHealth -= Damage;

            }
            playerHealthSlider.fillAmount = playerCourrentHealth / playerMaxHealth;
            if (healthText == null)
            {
                healthText = playerHealthSlider.transform.parent.GetComponentInChildren<Text>();
            }
            healthText.text = ((int)playerCourrentHealth).ToString();
            if (playerCourrentHealth <= 0)
            {
                MakeDead();
            }

            AkSoundEngine.PostEvent("Player_Damage", gameObject);
        }
    }

    void MakeDead()
    {
        GetComponent<PlayerController>().MakeDeath();
    }
}
