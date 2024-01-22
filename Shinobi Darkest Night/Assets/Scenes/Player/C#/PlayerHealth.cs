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
    [SerializeField] float regenSpeed = 13;
    [SerializeField] Image playerHealthSlider;
    [SerializeField] Image addHpSlider;
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
            playerCourrentHealth += regenSpeed*Time.deltaTime;
            addHp -= regenSpeed*Time.deltaTime;
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
        if (playerCourrentHealth+ addHp > playerMaxHealth) 
        { 
            addHp = playerMaxHealth - playerCourrentHealth;

            if (GetComponent<PlayerController>().isHealing)
            {
                GetComponent<PlayerController>().StopHealing();
            }
        }
        addHpSlider.fillAmount = (playerCourrentHealth + addHp) / playerMaxHealth;
        AkSoundEngine.PostEvent("Player_Healing_Drinking", gameObject);
    }

    public void AddDamage(float Damage)
    {
        if (playerController.godMode || playerCourrentHealth < 0)
        {
            return;
        }
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
        addHpSlider.fillAmount = (playerCourrentHealth + addHp) / playerMaxHealth;
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

    void MakeDead()
    {
        GetComponent<PlayerController>().MakeDeath();
    }
}
