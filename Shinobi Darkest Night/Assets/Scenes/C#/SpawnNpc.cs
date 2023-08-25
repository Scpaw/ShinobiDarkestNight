using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpawnNpc : MonoBehaviour
{
    private bool playerIn;
    [Header("Canvas")]
    [SerializeField] GameObject canvasWorld;
    [SerializeField] GameObject canvas;
    [Header("Button Images")] 
    [SerializeField] Image Doll;
    [SerializeField] Image ScareCrow;
    [SerializeField] Image Archer;
    [SerializeField] Image Resetbutton;
    [SerializeField] Image Spear;
    [SerializeField] Image Mokubi;
    [Tooltip("Room to spawn enemies")]
    [SerializeField] RoomBrain roomToActivate;
    [SerializeField] List<GameObject> enemies;


    private void Start()
    {
        roomToActivate.enemiesToSpaw = new List<GameObject>();
    }
    private void Update()
    {
        canvasWorld.SetActive(playerIn);
        if (playerIn && Input.GetKeyDown(KeyCode.E))
        { 
            canvas.SetActive(!canvas.activeInHierarchy);
        }
        if (!playerIn && canvas.activeInHierarchy)
        {
            canvas.SetActive(false);
        }
        if (roomToActivate.enemies.Count > 0)
        {
            Resetbutton.color = new Color(Resetbutton.color.r, Resetbutton.color.g, Resetbutton.color.b, 1);
        }
        else 
        {
            Resetbutton.color = new Color(Resetbutton.color.r, Resetbutton.color.g, Resetbutton.color.b, 0.5f);
        }
    }

    public void DollButton()
    {
        if (Doll.color.a < 1)
        {
            Doll.color = new Color(Doll.color.r, Doll.color.g, Doll.color.b, 1);
            roomToActivate.enemiesToSpaw.Remove(enemies[0]);
        }
        else
        {
            Doll.color = new Color(Doll.color.r, Doll.color.g, Doll.color.b, 0.5f);
            roomToActivate.enemiesToSpaw.Add(enemies[0]);
        }
    }

    public void ScareCrowButton()
    {
        if (ScareCrow.color.a < 1)
        {
            ScareCrow.color = new Color(ScareCrow.color.r, ScareCrow.color.g, ScareCrow.color.b, 1);
            roomToActivate.enemiesToSpaw.Remove(enemies[1]);
        }
        else
        {
            ScareCrow.color = new Color(ScareCrow.color.r, ScareCrow.color.g, ScareCrow.color.b, 0.5f);
            roomToActivate.enemiesToSpaw.Add(enemies[1]);
        }
    }

    public void ArcherButton()
    {
        if (Archer.color.a < 1)
        {
            Archer.color = new Color(Archer.color.r, Archer.color.g, Archer.color.b, 1);
            roomToActivate.enemiesToSpaw.Remove(enemies[2]);
        }
        else
        {
            Archer.color = new Color(Archer.color.r, Archer.color.g, Archer.color.b, 0.5f);
            roomToActivate.enemiesToSpaw.Add(enemies[2]);
        }
    }

    public void SpearButton()
    {
        if (Spear.color.a < 1)
        {
            Spear.color = new Color(Spear.color.r, Spear.color.g, Spear.color.b, 1);
            roomToActivate.enemiesToSpaw.Remove(enemies[3]);
        }
        else
        {
            Spear.color = new Color(Spear.color.r, Spear.color.g, Spear.color.b, 0.5f);
            roomToActivate.enemiesToSpaw.Add(enemies[3]);
        }
    }

    public void MOkubiButton()
    {
        if (Mokubi.color.a < 1)
        {
            Mokubi.color = new Color(Mokubi.color.r, Mokubi.color.g, Mokubi.color.b, 1);
            roomToActivate.enemiesToSpaw.Remove(enemies[4]);
        }
        else
        {
            Mokubi.color = new Color(Mokubi.color.r, Mokubi.color.g, Mokubi.color.b, 0.5f);
            roomToActivate.enemiesToSpaw.Add(enemies[4]);
        }
    }

    public void ResetSpawn()
    {
        int i = roomToActivate.enemies.Count;
        while (i > 0)
        {
            GameObject enemy = roomToActivate.enemies[i - 1];
            roomToActivate.enemies.Remove(roomToActivate.enemies[i-1]);
            Destroy(enemy);
            i--;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!playerIn)
            {
                playerIn = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (playerIn)
            {
                playerIn = false;
            }
        }
    }
}

