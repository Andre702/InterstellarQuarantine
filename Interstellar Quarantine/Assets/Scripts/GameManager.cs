using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager grid;
    private Vector2 direction;

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
            direction = new Vector2(horizontalInput, verticalInput).normalized;
        }

        // Log the direction upon pressing the spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Current Direction: " + GetDirectionAsString());
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
        else if (direction == (Vector2.up + Vector2.left).normalized)
            return "UpLeft";
        else if (direction == (Vector2.up + Vector2.right).normalized)
            return "UpRight";
        else if (direction == (Vector2.down + Vector2.left).normalized)
            return "DownLeft";
        else if (direction == (Vector2.down + Vector2.right).normalized)
            return "DownRight";
        else
            return "No specific direction";
    }
}
