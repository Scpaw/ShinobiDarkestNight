using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    //dialogue
    private Text text;
    private Button buttonToPress;
    [SerializeField] private List<string> textToDisplay;
    private int textIndex;
    [SerializeField] private bool reDialogue;
    [Header("CHOOSE ONLY ONE!")]
    [SerializeField] private bool trigger;
    [SerializeField] private bool button;
    [SerializeField] private bool cutScene;

    private bool isWriting;
    private float startTimer;
    [Tooltip("how long text lasts")]
    [SerializeField] private float timer;
    [Tooltip("how long untill nex text")]
    [SerializeField] float timerToNextText;
    private float startTextTimer;
    private bool playerIn;
    private Vector3 point;
    void Start()
    {
        text = GetComponentInChildren<Text>();
        startTimer = timer;
        startTextTimer = timerToNextText;
        timerToNextText = 0;
        point = transform.Find("PlaceForPlayer").transform.position;
        buttonToPress = GetComponentInChildren<Button>();
        if (button && !buttonToPress.gameObject.activeInHierarchy)
        {
            buttonToPress.transform.parent.gameObject.SetActive(true);
        }
        else if (!button && buttonToPress.gameObject.activeInHierarchy)
        {
            buttonToPress.transform.parent.gameObject.SetActive(false);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartDialogue();
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (text.text != string.Empty)
        { 
            text.text = string.Empty;
        }

        if (playerIn)
        {
            if (timerToNextText > 0)
            {
                timerToNextText -= Time.deltaTime;
            }
            else
            {
                NextDialogue();
                timerToNextText = startTextTimer;
            }
        }


        if ((button || cutScene) && (PlayerController.Instance.transform.position - point).magnitude <=0.1f && !playerIn)
        { 
            playerIn = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trigger&& collision.tag == "Player")
        {
            if (!playerIn)
            {
                playerIn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (trigger && collision.tag == "Player")
        {
            if (playerIn)
            {
                playerIn = false;
                timerToNextText = 0;
            }
        }
    }

    public void StartDialogue()
    {
        PlayerController.Instance.MoveToPlace(point);
        if (reDialogue)
        {
            textIndex = 0;
        }
        buttonToPress.transform.parent.gameObject.SetActive(false);
    }

    private void NextDialogue()
    {
        if (!isWriting)
        {
            if (textIndex < textToDisplay.Count)
            {
                StartCoroutine(WriteText(textToDisplay[textIndex]));
                textIndex++;
            }
            else if (reDialogue && trigger)
            {
                StartCoroutine(WriteText(textToDisplay[0]));
                textIndex = 1;
            }

        }
    }

    private IEnumerator WriteText(string textToWrite)
    {
        if (textToWrite != text.text)
        {
            timer = startTimer;
            if (text.text != string.Empty)
            {
                text.text = string.Empty;
            }
            isWriting = true;
            foreach (char letter in textToWrite.ToCharArray())
            {
                text.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
            timer = startTimer;
            isWriting = false;
            if (button && textIndex >= textToDisplay.Count)
            {
                PlayerController.Instance.Dialogue = false;
                if (button && reDialogue)
                {
                    buttonToPress.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            timer = startTimer;
        }
    }
}
