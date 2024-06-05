using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Response
{
    public string keySentence;
    public string destinationNode;
    

    public Response(string text)
    {
        int arrow = text.IndexOf("->");
        if (arrow != -1)
        {
            keySentence = text.Substring(0, arrow);
            destinationNode = text.Substring(arrow + 2, text.Length - (arrow + 2));
        }
        else
        {
            keySentence = text;
            destinationNode = text;
        }
    }
}

public class DialogueObject : MonoBehaviour
{
    public Dictionary<string, DialogueNode> nodes;
    public string titleOfStartNode = "";

    public DialogueObject(TextAsset twineText)
    {
        nodes = new Dictionary<string, DialogueNode>();
        ParseTwineText(twineText.ToString());
    }

    public DialogueNode GetNode(string nodeTitle)
    {
        return nodes[nodeTitle];
    }

    public DialogueNode GetStartNode()
    {
        return nodes[titleOfStartNode];
    }

    public void ParseTwineText(string twineText)
    {
        string[] nodeData = twineText.Split(new string[] { "::" }, StringSplitOptions.None);

        bool passedHeader = false;
        bool firstnode = true;

        for (int i = 0; i < nodeData.Length; i++)
        {

            // The first node comes after the UserStylesheet node
            if (!passedHeader)
            {
                if (nodeData[i].StartsWith(" UserStylesheet"))
                    passedHeader = true;

                continue;
            }

            // Note: tags are optional
            // Normal Format: "NodeTitle [Tags, comma, seperated] \r\n Message Text \r\n [[Response One]] \r\n [[Response Two]]"
            // No-Tag Format: "NodeTitle \r\n Message Text \r\n [[Response One]] \r\n [[Response Two]]"
            string currLineText = nodeData[i];

            // Remove position data
            int posBegin = currLineText.IndexOf("{\"position");
            if (posBegin != -1)
            {
                int posEnd = currLineText.IndexOf("}", posBegin);
                currLineText = currLineText.Substring(0, posBegin) + currLineText.Substring(posEnd + 1);
            }

            int titleStart = 0;
            int titleEnd = currLineText.IndexOf("\r\n");

            string title = "";
            if (titleEnd > 0)
            {
                title = currLineText.Substring(titleStart, titleEnd).Trim();
                currLineText = currLineText.Substring(titleEnd + 2, currLineText.Length - (titleEnd + 2));
            }

            if (firstnode)
            {
                titleOfStartNode = title;
                firstnode = false;
            }
            


            DialogueNode curNode = new DialogueNode
            {
                title = title,
                text = currLineText
            };

            string responseIncludingText = currLineText;


            List<string> responseData = new List<string>(responseIncludingText.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            for (int k = responseData.Count - 1; k >= 0; k--)
            {
                string curResponseData = responseData[k];

                if (string.IsNullOrEmpty(curResponseData))
                {
                    responseData.RemoveAt(k);
                    continue;
                }

                int destinationStart = curResponseData.IndexOf("[[");

                if (destinationStart == -1)
                {
                    responseData.RemoveAt(k);
                    Debug.Log("No destination around in node titled, '" + curNode.title + "'");
                    continue;
                }

                int destinationEnd = curResponseData.IndexOf("]]");

                if (destinationEnd == -1)
                {
                    responseData.RemoveAt(k);
                    Debug.Log("No destination around in node titled, '" + curNode.title + "'");
                    continue;
                }

                do
                {
                    string responseText = curResponseData.Substring(destinationStart + 2, (destinationEnd - destinationStart) - 2);

                    curNode.responses.Add(new Response(responseText));

                    if (destinationEnd >= curResponseData.Length - 4)
                    {
                        break;
                    }

                    curResponseData = curResponseData.Substring(destinationEnd + 2, curResponseData.Length - (destinationEnd + 2));

                    destinationStart = curResponseData.IndexOf("[[");
                    destinationEnd = curResponseData.IndexOf("]]");
                }
                while (destinationStart != -1 || destinationEnd != -1);
            }

            nodes[curNode.title] = curNode;
        }
    }

    public void PrintNode()
    {

    }
}
