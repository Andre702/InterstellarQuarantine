using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int iD;

    public Sprite ogSprite;
    public Sprite[] infectedStages;
    public bool infected = false;
    public int infectionStage = 0;

    private int x;
    private int y;

    [SerializeField]
    private Transform highlight;
    [SerializeField]
    private Transform marker;
    private CanvasGroup highlightCG;

    void Start()
    {
        highlightCG = highlight.GetComponent<CanvasGroup>();
    }

    //public void UpdateSprite()
    //{
    //    if (infected)
    //    {
    //        GetComponent<SpriteRenderer>().sprite = infectedSprite;
    //    }
    //    else
    //    {
    //        GetComponent<SpriteRenderer>().sprite = ogSprite;
    //    }
    //}

    public void ChangeStage(int infectionStage)
    {
        this.infectionStage = infectionStage;
    }

    public void ProgressStage()
    {
        this.infectionStage += 1;
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
    }
}
