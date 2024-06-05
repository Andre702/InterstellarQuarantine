using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    public string title;
    public string text;
    public List<Response> responses;

    public DialogueNode()
    {
        title = "";
        text = "";
        responses = new List<Response>();
    }
    
}
