using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MedicsDeploySquare : MonoBehaviour, IPointerClickHandler
{
    public bool hasMedic = false;
    public bool isCaptainSquare = false;
    public GameObject medic;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GridManager.instance.DispatchMedics(-1, -1))
        {
            hasMedic = true;
        }

        UpdateImage();
    }

    public void UpdateImage()
    {
        medic.SetActive(hasMedic);

    }

}
