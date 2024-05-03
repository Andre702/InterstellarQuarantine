using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int iD;

    public Sprite[] infectedSprites;
    public int infectionStage = 0;

    public int incommingInfectionStage = 0;
    public bool infecting;

    private int x;
    private int y;

    [SerializeField]
    private Transform highlight;
    [SerializeField]
    private Image infectionSprite;
    [SerializeField]
    private Transform infectingMarker;
    private CanvasGroup highlightCG;

    void Start()
    {
        highlightCG = highlight.GetComponent<CanvasGroup>();
        UpdateTile();
    }

    public void UpdateTile()
    {
        if (incommingInfectionStage > 0)
        {
            infectionStage += incommingInfectionStage;
            infecting = true;
            infectingMarker.gameObject.SetActive(true);

            incommingInfectionStage = 0;
        }

        if (infectionStage > 3) { infectionStage = 3; }

        if (infectionStage <= 0)
        {
            // healthy
            infectionSprite.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            // sick
            infectionSprite.GetComponent<CanvasGroup>().alpha = 1;
        }
        infectionSprite.sprite = infectedSprites[infectionStage];
    }

    public void NewTurn()
    {
        if (infecting)
        {
            infecting = false;
            infectingMarker.gameObject.SetActive(false);
        }
    }

    public void Infect()
    {
        this.incommingInfectionStage += 1;
    }

    public void ChangeStage(int infectionStage)
    {
        this.infectionStage = infectionStage;
    }


    public void SetCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void SetHighlight(float alpha)
    {
        highlightCG.alpha = alpha;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlight(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"UI Tile clicked on coordinates: ({x}, {y})");
        Infect();
        UpdateTile();

    }
}
