using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSpeed = 5f; // Adjust the scrolling speed as needed
    public float scrollEdgeSize = 25f; // Adjust the edge size where scrolling starts

    void Update()
    {
        // Get the position of the mouse cursor
        Vector3 mousePosition = Input.mousePosition;

        // Get the size of the screen
        Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0);

        // Check if mouse cursor is near the left edge of the screen
        if (mousePosition.x < scrollEdgeSize)
        {
            // Move the camera to the left
            transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        }

        // Check if mouse cursor is near the right edge of the screen
        if (mousePosition.x > screenSize.x - scrollEdgeSize)
        {
            // Move the camera to the right
            transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);
        }
    }
}
