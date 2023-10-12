using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    //dialogue
    private Text text;
    private Button buttonToPress;
    private GameObject worldCanvas;
    [SerializeField] private Image image_player;
    [SerializeField] private Image image_npc;
    [SerializeField] private List<AnimateThis> playerAnimations;
    [SerializeField] private List<AnimateThis> npcAnimations;
    private int animationIndex;
    private AnimateThis currentAnimation;
    private float timebtwAnimations;

    [SerializeField] private List<OneDialogue> textToDisplay;
    [SerializeField] private List<OneDialogue> textByPlayer;
    private int textIndex;
    private int playerIndex;
    private int npcIndex;
    private bool npcTalking;
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
        Transform canvasGame = transform.Find("Canvas Game");
        text = canvasGame.GetComponentInChildren<Text>();
        startTimer = timer;
        startTextTimer = timerToNextText;
        timerToNextText = 0;
        point = transform.Find("PlaceForPlayer").transform.position;
        buttonToPress = canvasGame.GetComponentInChildren<Button>();
        if (button && !buttonToPress.gameObject.activeInHierarchy)
        {
            buttonToPress.transform.parent.gameObject.SetActive(true);
        }
        else if (!button && buttonToPress.gameObject.activeInHierarchy)
        {
            buttonToPress.transform.parent.gameObject.SetActive(false);
        }
        worldCanvas = transform.Find("World Canvas").gameObject;
        if ( worldCanvas.activeInHierarchy)
        { 
            worldCanvas.SetActive(false);
        }
    }


    void Update()
    {
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
            PlayerController.Instance.StartHealing();
            if (cutScene && !worldCanvas.activeInHierarchy)
            {
                worldCanvas.SetActive(true);
            }
        }


        if (isWriting && timebtwAnimations <= 0)
        {
            if (npcTalking)
            {
                image_npc.sprite = currentAnimation.sprites[animationIndex];
            }
            else
            {
                image_player.sprite = currentAnimation.sprites[animationIndex];
            }

            if (animationIndex+1 < currentAnimation.sprites.Count )
            {
                animationIndex++;
            }
            else
            {
                animationIndex = 0;
            }
            timebtwAnimations = 0.1f;
        }
        else if (isWriting)
        { 
            timebtwAnimations -=Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((trigger || cutScene )&& collision.tag == "Player")
        {
            if (cutScene && ((npcIndex == 0 && playerIndex ==0) || reDialogue))
            {
                StartDialogue();
            }
            if (!playerIn && !cutScene)
            {
                playerIn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((trigger || cutScene )&& collision.tag == "Player")
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
        if (cutScene)
        { 
            npcTalking=true;
            if (reDialogue && (playerIndex > 0 || npcIndex > 0))
            { 
                playerIndex = 0;
                npcIndex = 0;
                image_player.GetComponentInChildren<Text>().text = string.Empty;
                image_npc.GetComponentInChildren<Text>().text = string.Empty;
            }
        }
    }

    private void NextDialogue()
    {
        if (!isWriting)
        {
            if (cutScene)
            {
                if (npcTalking && npcIndex < textToDisplay.Count)
                {
                    StartCoroutine(WriteText(textToDisplay[npcIndex].text, image_npc.GetComponentInChildren<Text>()));
                    npcIndex++;
                }
                else if(!npcTalking && playerIndex < textByPlayer.Count)
                {
                    StartCoroutine(WriteText(textByPlayer[playerIndex].text, image_player.GetComponentInChildren<Text>()));
                    playerIndex++;
                }
            }
            else if (textIndex < textToDisplay.Count)
            {
                StartCoroutine(WriteText(textToDisplay[textIndex].text, text));
                textIndex++;
            }
            else if (reDialogue && trigger)
            {
                StartCoroutine(WriteText(textToDisplay[0].text,text));
                textIndex = 1;
            }
        }
    }

    private IEnumerator WriteText(string textToWrite,Text text)
    {
        if (textToWrite != text.text)
        {
            timer = startTimer;
            if (text.text != string.Empty)
            {
                text.text = string.Empty;
            }
            if (cutScene)
            {
                if (npcTalking)
                {
                    currentAnimation = npcAnimations[Random.Range(0, npcAnimations.Count)];
                }
                else
                {
                    currentAnimation = playerAnimations[Random.Range(0, playerAnimations.Count)];
                }
                animationIndex = 0;
            }
            isWriting = true;
            foreach (char letter in textToWrite.ToCharArray())
            {
                text.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
            timer = startTimer;
            isWriting = false;
            if ((button || cutScene) && textIndex >= textToDisplay.Count)
            {
                PlayerController.Instance.Dialogue = false;
                if ((button) && reDialogue)
                {
                    buttonToPress.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            timer = startTimer;
        }

        if (cutScene)
        {
            if (npcTalking)
            {
                if (textToDisplay[npcIndex - 1].anotherDialogue)
                {
                    npcTalking = false;
                }
                else if (textToDisplay[npcIndex - 1].endDialogue)
                {
                    yield return new WaitForSeconds(2f);
                    PlayerController.Instance.Dialogue = false;
                    worldCanvas.SetActive(false);
                }
            }
            else
            {
                if (textByPlayer[playerIndex - 1].anotherDialogue)
                {
                    npcTalking = true;
                }
                else if (textByPlayer[playerIndex - 1].endDialogue)
                {
                    yield return new WaitForSeconds(2f);
                    PlayerController.Instance.Dialogue = false;
                    worldCanvas.SetActive(false);
                }
            }
        }
    }
}



[System.Serializable]
public class OneDialogue
{
    public string text;
    public bool anotherDialogue;
    public bool endDialogue;
}

[System.Serializable]
public class AnimateThis
{ 
    public List<Sprite> sprites = new List<Sprite>();
}