using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Vector2 direction;

    public int difficulty;

    public int medicsMaxNumber = 2;
    public int medicsNumber = 2;
    public TextMeshProUGUI medicsNumberText;
    public Button medicsDispatchButton;
    public bool medicDispatching;
    public int medicsRangeOfInfluence = 2;

    // Actions section:
    public List <Vector2> blockingDirections;
    [SerializeField] TextAsset doctorText;

    [SerializeField] TextAsset captainText;

    public List<Vector2> possibleDirections = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right, (Vector2.up + Vector2.left), (Vector2.up + Vector2.right), (Vector2.down + Vector2.left), (Vector2.down + Vector2.right) };
    public List<Vector2> chosenDirections = new List<Vector2>();

    public DialogueManager dialogueManager;

    public delegate void NextTurnHandler();
    public static event NextTurnHandler OnNextTurn;

    public delegate void PostSpreadUpdateHandler();
    public static event PostSpreadUpdateHandler OnPostSprteadUpdate;

    public static GameManager instance { get; private set; }

    public float riotMeter = 0f;
    public Transform riotBar;

    public float vaccine = 0f;
    public Transform vaccineBar;

    public MedicsDeploySquare doctorSquare;

    public GameObject doctorImage;
    public GameObject captainImage;

    public int dead = 0;
    public GameObject endScreen;



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

    void Start()
    {
        OnPostSprteadUpdate += ProgrssVaccine;
        dialogueManager.InitializeDialogue();

        GridManager.instance.StartGrid();

        if (GridManager.instance.StartInfection())
        {
            OnNextTurn();
            OnPostSprteadUpdate();
        }
        else
        {
            // END THE GAME?
        }


        
    }

    void Update()
    {
        // Update direction based on arrow key inputs
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //if (horizontalInput != 0 || verticalInput != 0)
        //{
        //    direction = new Vector2(horizontalInput, verticalInput);
        //}

        // Log the direction upon pressing the spacebar
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Debug.Log("Current Direction: " + GetDirectionAsString());
        //    GridManager.instance.SpreadDisease1(direction);
        //}

        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            for (int i = 0; i < difficulty; i++)
            {
                int rDirection = Random.Range(0, 8);
                chosenDirections.Add(possibleDirections[rDirection]);
            }

            GridManager.instance.SpreadDisease1(chosenDirections);

            dead = 0;

            OnNextTurn();

            if (dead >= (GridManager.instance.width * GridManager.instance.height * 0.6))
            {
                endScreen.SetActive(true);
                endScreen.GetComponentInChildren<TextMeshProUGUI>().text = "You Lost\n\nThe casualties exceeded 60 % of the crew. Your mission to colonize other planets is doomed to fail";
            }

            OnPostSprteadUpdate();

            if (vaccine >= 100)
            {
                endScreen.SetActive(true);
                endScreen.GetComponentInChildren<TextMeshProUGUI>().text = "You Win\n\nVaccine for the disease has been successfully developed and distributed among the remaining crew members";
            }

            if (riotMeter >= 100)
            {
                endScreen.SetActive(true);
                endScreen.GetComponentInChildren<TextMeshProUGUI>().text = "You Lost\n\nThe crew did not appreciate your efforts and fueled by ignorance and frustration they rebelled against you";
            }

            chosenDirections = new List<Vector2>();
        }

        UpdateMeters();


    }

    public void UpdateMeters()
    {
        Vector3 scaleR = riotBar.localScale;
        scaleR.x = riotMeter / 100;
        riotBar.localScale = scaleR;

        Vector3 scaleV = vaccineBar.localScale;
        scaleV.x = vaccine / 100;
        vaccineBar.localScale = scaleV;
    }

    string GetDirectionAsString()
    {
        if (direction == Vector2.up)
            return "Up";
        else if (direction == Vector2.down)
            return "Down";
        else if (direction == Vector2.left)
            return "Left";
        else if (direction == Vector2.right)
            return "Right";
        else if (direction == (Vector2.up + Vector2.left))
            return "UpLeft";
        else if (direction == (Vector2.up + Vector2.right))
            return "UpRight";
        else if (direction == (Vector2.down + Vector2.left))
            return "DownLeft";
        else if (direction == (Vector2.down + Vector2.right))
            return "DownRight";
        else
            return "No specific direction";
    }

    public void ResetBlocking()
    {
        blockingDirections = new List<Vector2>();


    }

    public void ProgrssVaccine()
    {
        if (doctorSquare.hasMedic)
        {
            vaccine += (float)Random.Range(4, 7);
        }

    }

    public void TalkToDoctor()
    {
        dialogueManager.InitializeNPCDialogue(doctorText);
        doctorImage.SetActive(true);

    }

    public void TalkToCaptain()
    {
        dialogueManager.InitializeNPCDialogue(captainText);
        captainImage.SetActive(true);
    }


    public void OnMedicsDispatchButtonClick() 
    { 
        if (medicsNumber > 0)
        {
            medicDispatching = true;
        }
        
    }

    public void OnMedicsReturnButtonClick()
    {
        medicDispatching = false;
        GridManager.instance.ReturnAllMedics();
        medicsNumber = medicsMaxNumber;
        medicsNumberText.text = medicsNumber.ToString();
        medicsDispatchButton.interactable = true;
    }

    public bool MedicsAdd(int n)
    {
        if (medicsNumber > 0)
        {
            medicsNumber += n;
            medicsNumberText.text = medicsNumber.ToString();

            if (medicsNumber <= 0)
            {
                medicsDispatchButton.interactable = false;
            }
            medicDispatching = false;
            return true;
        }
        else
        {
            medicsDispatchButton.interactable = false;
            medicDispatching = false;
            return false;
        }
        
    }

    public void CloseAllImages()
    {
        doctorImage.SetActive(false);
        captainImage.SetActive(false);
    }
}
