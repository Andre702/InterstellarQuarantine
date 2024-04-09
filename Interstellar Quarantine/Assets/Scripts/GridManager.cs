using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public Tile tileTemplate;
    public Transform mainCamera;
    public Dictionary<(int x, int y), Tile> tileSet;

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        tileSet = new Dictionary<(int x, int y), Tile>();

        // create a field of width x height tiles, each named after its coordinates
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var newTile = Instantiate(tileTemplate, new Vector3(x, height - y), Quaternion.identity);
                newTile.name = $"Tile {x}, {y}";
                // y position is inverted because in Unity Y numbers grow upwards and I hate it.

                tileSet[new (x, y)] = newTile;
            }
        }

        //move the camera so that it will be centered on the newly created tile grid
        mainCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

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

    public void InfectTile(int x, int y)
    {
        var selectedTile = GetTile(x, y);
        selectedTile.infected = true;
        selectedTile.UpdateSprite();

    }
}
