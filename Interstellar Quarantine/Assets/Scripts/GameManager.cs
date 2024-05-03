using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager grid;
    private Vector2 direction;

    public delegate void NextTurnHandler();
    public static event NextTurnHandler OnNextTurn;

    public delegate void PostSpreadUpdateHandler();
    public static event PostSpreadUpdateHandler OnPostSprteadUpdate;

    void Start()
    {
        
    }

    void Update()
    {
        // Update direction based on arrow key inputs
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            direction = new Vector2(horizontalInput, verticalInput);
        }

        // Log the direction upon pressing the spacebar
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Current Direction: " + GetDirectionAsString());
            grid.SpreadDisease1(direction);
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            OnNextTurn();
            OnPostSprteadUpdate();
        }
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
}
