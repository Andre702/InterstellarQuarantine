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

    public static GridManager instance { get; private set; }

    public MedicsDeploySquare doctorSquare;
    public MedicsDeploySquare captainSquare;

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

    public void StartGrid()
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
                newTile.canBeInfected = true;
                newTile.name = $"Tile {x}, {y}";
                Vector3 coordinates = new Vector3
                    (
                    x * (tileSize + tileSpacing),
                    ((_height - 1) - y) * (tileSize + tileSpacing)
                    );
                newTile.transform.localPosition = coordinates;
                // y position is inverted because in Unity Y numbers grow upwards and I hate it.

                newTile.GetComponent<Tile>().SetCoordinates(x, y);

                GameManager.OnPostSprteadUpdate += newTile.UpdateTile;
                GameManager.OnNextTurn += newTile.NewTurn;

                tileSet[new (x, y)] = newTile;

                //var tileDuuble = Instantiate(tileTemplate, mainCanvasGroup);
                //tileDuuble.canBeInfected = true;
                //tileDuuble.name = $"Tile {x}, {y} - double";
                //tileDuuble.transform.localPosition = coordinates;
            }
        }

        var extraTile = Instantiate(tileTemplate, mainCanvasGroup);

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

                GameManager.OnPostSprteadUpdate += newTile.UpdateTile;

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

    public void SpreadDisease1(List<Vector2> directions)
    {
        if (directions.Count == 0)
        {
            return;
        }

        for (int i = 0; i < directions.Count; i++)
        {
            Vector2 direction = directions[i];
            direction.y *= -1;
            directions[i] = direction;
        }

        
        // y position is inverted because in Unity Y numbers grow upwards and I hate it.

        bool diseaseHasSpread = false;
        for (int i = 0; i < directions.Count; i++)
        {
            Vector2 direction = directions[i];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tile targetTile = GetTile(x, y);

                    if (targetTile != null && targetTile.infecting) // change infection stage to infecting)
                    {
                        if (targetTile.medicPresent)
                        {
                            if (DirectionIsBeingBlocked(direction))
                            {
                                continue;
                            }
                        }

                        if (InfectTile(x + (int)direction.x, y + (int)direction.y))
                        {
                            diseaseHasSpread = true;
                        }
                    }
                }
            }
        }
        

        if (!diseaseHasSpread)
        {
            StartInfection();
            StartInfection();
        }
    }

    public bool DirectionIsBeingBlocked(Vector2 direction)
    {
        foreach (Vector2 compareDirection in GameManager.instance.blockingDirections)
        {
            //here the compareDirections would be inverted but since the blocking is done in groups of adjacend or neighbouring it will be fine
            if (direction == compareDirection)
            {
                if (GameManager.instance.blockingDirections.Count > 7)
                {
                    GameManager.instance.riotMeter -= 0.2f;
                }
                return true;
            }
        }
        return false;
    }

    public bool StartInfection()
    {
        // For this to work with string map it needs a check of a null for a tile or a list of existing infectable tile
        int allowedAttemptsToFindSpot = 10;

        int targetX = Random.Range(0, width);
        int targetY = Random.Range(0, height);


        for (int i = 0; i < allowedAttemptsToFindSpot; i++)
        {
            if (!NeighbourhoodAlreadyInfected(targetX, targetY))
            {
                break;
            }

            // else reshuffle coordinates
            targetX = Random.Range(0, width);
            targetY = Random.Range(0, height);
        }

        if (!GetTile(targetX, targetY).canBeInfected)
        {
            (targetX, targetY) = FirstInfectableTileCoordinates();
            if (targetX < 0 && targetY < 0)
            {
                // GAME HAS ENDED EVERYONE IS IMMUNE ------------------------------------------
                return false;
            }
        }

        InfectTile(targetX, targetY);

        return true;
    }

    public (int, int) FirstInfectableTileCoordinates()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (GetTile(x, y).canBeInfected)
                {
                    return (x, y);
                }
            }
        }
        return (-1, -1);
    }

    public bool NeighbourhoodAlreadyInfected(int targetX, int targetY)
    {
        for (int x = (targetX - 1); x <= (targetX + 1); x++)
        {
            for (int y = (targetY - 1); y <= (targetY + 1); y++)
            {
                Tile neighborTile = GetTile(x, y);
                if (neighborTile != null)
                {
                    if (neighborTile.infectionStage > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool InfectTile(int x, int y)
    {
        Tile selectedTile = GetTile(x, y);
        if (selectedTile != null)
        {
            selectedTile.Infect();
            return true;
            //if (!selectedTile.medicPresent)
            //{
                
            //}
            
        }
        return false;
    }

    public bool DispatchMedics(int targetX, int targetY)
    {
        if (GameManager.instance.MedicsAdd(-1))
        {
            if (targetX == -1 && targetY == -1)
            {
                return true;
            }

            int distance = GameManager.instance.medicsRangeOfInfluence;
            for (int x = (targetX - distance); x <= (targetX + distance); x++)
            {
                for (int y = (targetY - distance); y <= (targetY + distance); y++)
                {
                    Tile affectedTile = GetTile(x, y);
                    if (affectedTile != null)
                    {
                        affectedTile.MedicDispatched(true);
                    }
                }
            }

            return true;
        }

        return false;
    }

    public void ReturnAllMedics()
    {
        doctorSquare.hasMedic = false;
        doctorSquare.UpdateImage();

        captainSquare.hasMedic = false;
        captainSquare.UpdateImage();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tileTarget = GetTile(x, y);
                if (tileTarget != null)
                {
                    if (tileTarget.medicPresent)
                    {
                        tileTarget.MedicDispatched(false);
                    }
                }
            }
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
                    GameManager.OnPostSprteadUpdate -= GetTile(x, y).UpdateTile;
                    GameManager.OnNextTurn -= GetTile(x, y).NewTurn;
                }
                
            }
        }
    }


}
