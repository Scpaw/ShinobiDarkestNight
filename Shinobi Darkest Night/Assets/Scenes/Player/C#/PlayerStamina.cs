using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Player Stamina")]
    [SerializeField] float playerCourrentStamina;
    [SerializeField] float playerMaxStamina;
    [SerializeField] Slider playerStaminaSlider;

    bool used = false;

    void Awake()
    {
        playerCourrentStamina = playerMaxStamina;
        playerStaminaSlider.maxValue = playerMaxStamina;
        playerStaminaSlider.value = playerCourrentStamina;
    }

    public void AddStamina(float Stamina)
    {
        playerCourrentStamina += Stamina;
        if (playerCourrentStamina > playerMaxStamina)
        {
            playerCourrentStamina = playerMaxStamina;
        }
        playerStaminaSlider.value = playerCourrentStamina;
    }

    public void DecreseStamina(float Stamina)
    {
        playerCourrentStamina -= Stamina;
        playerStaminaSlider.value = playerCourrentStamina;
        used = true;
    }
}
