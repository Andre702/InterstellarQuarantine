using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoctorsOptions : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();

        AddOptions();
    }

    void AddOptions()
    {
        // Create a list of options
        List<string> options = new List<string>();

        // Add options to the list
        options.Add("Masks");
        options.Add("Hand Washing");
        options.Add("Quarantine");

        // Add the options list to the dropdown
        dropdown.AddOptions(options);
    }
}
