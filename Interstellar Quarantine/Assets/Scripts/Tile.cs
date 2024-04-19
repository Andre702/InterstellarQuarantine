using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int iD;

    public Sprite ogSprite;
    public Sprite[] infectedSprites;
    public int infectionStage = 0;

    public int incommingInfectionStage = 0;

    private int x;
    private int y;

    [SerializeField]
    private Transform highlight;
    [SerializeField]
    private Image marker;
    private CanvasGroup highlightCG;

    void Start()
    {
        highlightCG = highlight.GetComponent<CanvasGroup>();
        UpdateSpriteMarker();
    }

    public void UpdateSpriteMarker()
    {
        infectionStage += incommingInfectionStage;
        if (infectionStage > 3) { infectionStage = 3; }

        incommingInfectionStage = 0;

        if (infectionStage <= 0)
        {
            marker.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            marker.GetComponent<CanvasGroup>().alpha = 1;
        }
        marker.sprite = infectedSprites[infectionStage];
    }

    public void ChangeStage(int infectionStage)
    {
        this.infectionStage = infectionStage;
    }

    public void ProgressStage()
    {
        this.incommingInfectionStage += 1;
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
        ProgressStage();
        UpdateSpriteMarker();

    }
}
