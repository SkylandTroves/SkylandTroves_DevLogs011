using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public TMP_Text StateText;
    public TMP_Text MessageText;

    private static TMP_Text stateText;
    private static TMP_Text messageText;

    private static string totalMessage;

    private static bool isActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        stateText = StateText;
        messageText = MessageText;
        
        StateText.text = String.Empty;
        MessageText.text = String.Empty;
        totalMessage = String.Empty;
    }

    public static void Enable()
    {
        isActive = true;
    }

    public static void Disable()
    {
        isActive = false;
    }

    public static void UpdateState(string message)
    {
        if (isActive)
        {
            stateText.text = message;
        }
    }
    
    public static void UpdateMessage(string message)
    {
        totalMessage = totalMessage + "\n" + message;
        if (isActive)
        {
            messageText.text = totalMessage;
        }
    }
}
