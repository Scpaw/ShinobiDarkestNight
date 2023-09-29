using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    //dialogue
    private Text text;
    [SerializeField] private List<string> textToDisplay;
    private int textIndex;
    [SerializeField] private bool reDialogue;
    [Header("CHOOSE ONLY ONE!")]
    [SerializeField] private bool trigger;
    [SerializeField] private bool button;
    [SerializeField] private bool cutScene;
    private bool isWriting;

    void Start()
    {
        text = GetComponentInChildren<Text>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            NextDialogue();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trigger&& collision.tag == "Player")
        {
            NextDialogue();
        }
    }

    public void StartDialogue()
    { 
         
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
            else if (reDialogue)
            {
                StartCoroutine(WriteText(textToDisplay[0]));
                textIndex = 1;
            }
        }
    }

    private IEnumerator WriteText(string textToWrite)
    {
        text.text = "";
        isWriting = true;
        foreach (char letter in textToWrite.ToCharArray())
        { 
            text.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isWriting = false;
    }
}
