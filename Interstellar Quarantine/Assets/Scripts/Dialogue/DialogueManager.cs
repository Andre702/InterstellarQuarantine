using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextAsset twineText;
    DialogueObject curDialogue;
    DialogueObject curNPCDialogue;

    DialogueNode lastStoryNode;
    DialogueNode curNode;

    bool isNPCDialogue = false;



    public delegate void NodeEnteredHandler(DialogueNode node);
    public event NodeEnteredHandler onEnteredNode;

    public TextMeshProUGUI title;
    public TextMeshProUGUI textDisplay;
    public List<Button> answers;

    public Button openStory;
    public Button closeStory;

    public GameObject dialogueWindow;

    public DialogueNode GetCurrentNode()
    {
        return curNode;
    }

    public static DialogueManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void InitializeDialogue()
    {
        if (curDialogue == null) 
        { 
            curDialogue = new DialogueObject(twineText); 
        }
        isNPCDialogue = false;

        if (lastStoryNode != null)
        {
            curNode = lastStoryNode;
        }
        else
        {
            curNode = curDialogue.GetStartNode();
        }
        
        dialogueWindow.SetActive(true);

        DisplayCurrentNode();
    }

    public void InitializeNPCDialogue(TextAsset twineText)
    {
        curNPCDialogue = new DialogueObject(twineText);
        isNPCDialogue = true;

        curNode = curNPCDialogue.GetStartNode();

        dialogueWindow.SetActive(true);

        DisplayCurrentNode();
    }

    public void DisplayCurrentNode()
    {
        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].GetComponentInChildren<TMP_Text>().text = "";
        }

        if (answers.Count >= curNode.responses.Count)
        {
            for (int i = 0; i < curNode.responses.Count; i++)
            {
                answers[i].GetComponentInChildren<TMP_Text>().text = curNode.responses[i].keySentence;
            }
        }

        textDisplay.text = curNode.text;
        title.text = curNode.title;
        
    }

    public void Answer(TextMeshProUGUI answer)
    {
        string destinationNode = answer.text;
        if (destinationNode == "")
        {
            return;
        }
        if (isNPCDialogue)
        {
            curNode = curNPCDialogue.GetNode(destinationNode);
        }
        else
        {
            curNode = curDialogue.GetNode(destinationNode);
        }
        

        if (curNode.title == "masks")
        {
            GameManager.instance.blockingDirections = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        }
        else if (curNode.title == "hygiene")
        {
            GameManager.instance.blockingDirections = new List<Vector2> { (Vector2.up + Vector2.left), (Vector2.up + Vector2.right), (Vector2.down + Vector2.left), (Vector2.down + Vector2.right) };
        }

        else if (curNode.title == "quarantine")
        {
            GameManager.instance.blockingDirections = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right, (Vector2.up + Vector2.left), (Vector2.up + Vector2.right), (Vector2.down + Vector2.left), (Vector2.down + Vector2.right) };
        }

        DisplayCurrentNode();
    }

    public void OpenDialogue()
    {

        dialogueWindow.SetActive(true);
    }

    public void CloseDialogue()
    {
        dialogueWindow.SetActive(false);

        GameManager.instance.CloseAllImages();
    }
}
