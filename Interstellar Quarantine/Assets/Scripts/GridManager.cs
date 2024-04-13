using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{ 
    [Header("Adjustable")]
    public int width;
    public int height;

    [Header("Links")]
    public Tile tileTemplate;
    public Transform mainCamera;
    public Transform mainCanvasGroup;

    public Dictionary<(int x, int y), Tile> tileSet;

    private float tileSize;
    private float tileSpacing = 5;

    private void Start()
    {
        tileSize = tileTemplate.GetComponent<RectTransform>().sizeDelta.x;
        CreateGrid();
        CenterGrid();
    }

    public void CreateGrid()
    {
        tileSet = new Dictionary<(int x, int y), Tile>();

        // create a field of width x height tiles, each named after its coordinates
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var newTile = Instantiate(tileTemplate, mainCanvasGroup);
                newTile.name = $"Tile {x}, {y}";
                newTile.transform.localPosition = new Vector3
                    (
                    x * (tileSize + tileSpacing), 
                    (height - y) * (tileSize + tileSpacing)
                    );
                // y position is inverted because in Unity Y numbers grow upwards and I hate it.

                newTile.GetComponent<Tile>().SetCoordinates(x, y);

                tileSet[new (x, y)] = newTile;
            }
        }

    }
    // Creates a full tileset from point 0,0 of the tileSet group
    // based on width and height provided in public variables
    private void CenterGrid()
    {

        mainCanvasGroup.transform.localPosition -= new Vector3
            (
            width/2 * (tileSize + tileSpacing) - tileSize/2, 
            height / 2 * (tileSize + tileSpacing) + tileSize/2
            );
    }
    // Moves tile set half its width and length left and down as it is created from point 0,0
    // +- tileSize/2 is compensating for the anchor of the tile being in its center so half of the tile inwards

    public Tile GetTile(int x, int y)
    {
        // if there exists tile on given x and y, return it
        if(tileSet.TryGetValue((x, y), out var tile))
        {
            return tile;
        }
        else
        {
            return null;
        }
    }
    // Returns a tile from given position x and y

    public void InfectTile(int x, int y)
    {
        var selectedTile = GetTile(x, y);
        selectedTile.infected = true;
        //selectedTile.UpdateSprite();

    }


}
