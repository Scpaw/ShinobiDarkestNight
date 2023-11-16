using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public GameObject menu;
    public bool pause;
    private GameObject gameMenu;
    private GameObject findsMenu;

    private void Start()
    {
        gameMenu = menu.transform.GetChild(1).gameObject;
        findsMenu = menu.transform.GetChild(2).gameObject;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
        }

        if (pause && !menu.activeInHierarchy)
        {
            menu.SetActive(true);
            gameMenu.SetActive(true);
            findsMenu.SetActive(false);
            Time.timeScale = 0;
            PlayerController.Instance.canAttack = false;
        }
        else if (!pause && menu.activeInHierarchy)
        {
            menu.SetActive(false);
            Time.timeScale = 1;
            PlayerController.Instance.canAttack = true;
        }
    }

    public void Resume()
    {
        pause = false;
    }

    public void Finds()
    { 
        gameMenu.SetActive(false);
        findsMenu.SetActive(true);
    }

    public void Back()
    {
        gameMenu.SetActive(true);
        findsMenu.SetActive(false);
    }
}
