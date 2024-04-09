using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite ogSprite;
    public Sprite infectedSprite;
    public bool infected = false;

    public void UpdateSprite()
    {
        if (infected)
        {
            GetComponent<SpriteRenderer>().sprite = infectedSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = ogSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
