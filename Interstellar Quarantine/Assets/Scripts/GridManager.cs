using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{ 
    [Header("Adjustable")]
    public int width;
    public int height;

    public int tileSpacing;
    private float tileSize;

    [TextArea(1, 20)]
    public string tileMapString;
    private int[,] tileMap;

    [Header("Links")]
    public Tile tileTemplate;
    public Transform mainCamera;
    public Transform mainCanvasGroup;

    public Dictionary<(int x, int y), Tile> tileSet;

    public delegate void PostSpreadUpdateHandler();
    public static event PostSpreadUpdateHandler PostSprteadUpdate;


    private void Start()
    {
        tileSize = tileTemplate.GetComponent<RectTransform>().sizeDelta.x;
        if (tileMapString.Length > 0)
        {
            tileMap = ParseTileMap(tileMapString);
            width = tileMap.GetLength(1);
            height = tileMap.GetLength(0);

            CreateGridFromMap(tileMap);
            
        }
        else
        {
            CreateGridGeneric(width, height);
        }

        CenterGrid();
    }


    public void CreateGridGeneric(int _width, int _height)
    {
        tileSet = new Dictionary<(int x, int y), Tile>();

        // create a field of width x height tiles, each named after its coordinates
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var newTile = Instantiate(tileTemplate, mainCanvasGroup);
                newTile.name = $"Tile {x}, {y}";
                newTile.transform.localPosition = new Vector3
                    (
                    x * (tileSize + tileSpacing), 
                    ((_height-1) - y) * (tileSize + tileSpacing)
                    );
                // y position is inverted because in Unity Y numbers grow upwards and I hate it.

                newTile.GetComponent<Tile>().SetCoordinates(x, y);

                PostSprteadUpdate += newTile.UpdateSpriteMarker;

                tileSet[new (x, y)] = newTile;
            }
        }

    }
    // Creates a square tileset from point 0,0 of the tileSet group
    // based on width and height provided in public variables

    private int[,] ParseTileMap(string map)
    {
        // Split the tile map string into rows
        string[] rows = map.Trim().Split('\n');

        // Get the number of rows and columns
        int numRows = rows.Length;
        int numCols = rows[0].Length;

        // Initialize the tile map array
        int[,] tileMap = new int[numRows, numCols];

        // Parse each row
        for (int i = 0; i < numRows; i++)
        {
            // Parse each character in the row
            for (int j = 0; j < numCols; j++)
            {
                // Convert character to integer (assuming '1' represents tile, '0' represents disabled tile)
                tileMap[i, j] = (rows[i][j] == '1') ? 1 : 0;
            }
        }

        return tileMap;
    }
    // Converts tileMapString into array of numbers that will be used in creation of a custom map

    public void CreateGridFromMap(int[,] _tileMap)
    {
        tileSet = new Dictionary<(int x, int y), Tile>();
        height = _tileMap.GetLength(0);
        width = _tileMap.GetLength(1);

        // create a field of width x height tiles, each named after its coordinates
        for (int y = 0; y < _tileMap.GetLength(0); y++)
        {
            for (int x = 0; x < _tileMap.GetLength(1); x++)
            {
                if (_tileMap[y, x] == 0)
                {
                    continue;
                }
                var newTile = Instantiate(tileTemplate, mainCanvasGroup);
                newTile.name = $"Tile {x}, {y}";
                newTile.transform.localPosition = new Vector3
                    (
                    x * (tileSize + tileSpacing),
                    ((_tileMap.GetLength(1) - 1) - y) * (tileSize + tileSpacing)
                    );
                // y position is inverted because in Unity Y numbers grow upwards and I hate it.

                newTile.GetComponent<Tile>().SetCoordinates(x, y);

                PostSprteadUpdate += newTile.UpdateSpriteMarker;

                tileSet[new(x, y)] = newTile;
            }
        }

        Debug.Log(tileSet.Count);
    }
    // Creates a custom tileset from point 0,0 of the tileSet group
    // based on a map provided in a tileMapString area (0 - empty tiles, 1 - full tiles)
    // provided map should be square to achieve desired result

    private void CenterGrid()
    {
        float fWidth = (float)width;
        float fHeight = (float)height;
        Vector3 shift = new Vector3
            (
            (fWidth / 2 * (tileSize + tileSpacing) - tileSize / 2),
            (fHeight / 2 * (tileSize + tileSpacing) - tileSize / 2)
            );

        mainCanvasGroup.transform.localPosition -= shift;
    }
    // Centers grid on a canvas depending on the shape of the grid

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

    public void SpreadDisease1(Vector2 direction)
    {
        if (direction == null)
        {
            return;
        }

        direction.y *= -1;
        // y position is inverted because in Unity Y numbers grow upwards and I hate it.

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile targetTile = GetTile(x, y);

                if (targetTile != null && targetTile.infectionStage > 0)
                {
                    InfectTile(x + (int)direction.x, y + (int)direction.y);
                }
            }
        }

        PostSprteadUpdate();
    }

    public void InfectTile(int x, int y)
    {
        Tile selectedTile = GetTile(x, y);
        if (selectedTile != null)
        {
            selectedTile.ProgressStage();
        }
        
    }

    private void OnDestroy()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile targetTile = GetTile(x, y);
                if (targetTile != null)
                {
                    PostSprteadUpdate -= GetTile(x, y).UpdateSpriteMarker;
                }
                
            }
        }
    }


}
