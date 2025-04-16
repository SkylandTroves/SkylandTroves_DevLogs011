/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    MAIN,
    INSTRUCTIONS,
    WIN,
    LOSE,
    COUNTDOWN,
} 

public class PanelSwapping : MonoBehaviour
{
    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup InstructionsCanvasGroup;
    
    private Dictionary<PanelType, CanvasGroup> panels = new Dictionary<PanelType, CanvasGroup>();
    
    public void Awake()
    {
        panels.Add(PanelType.MAIN, MainMenuCanvasGroup);
        panels.Add(PanelType.INSTRUCTIONS, InstructionsCanvasGroup);
    }
    
    // makes sure that the main menu is visible when the game is started 
    private void Start()
    {
        OnClickReturnToMainMenu();
    }
    
    // show a panel 
    public void Show(PanelType panelType)
    {
        CanvasGroupDisplayer.Show(panels[panelType]);
    }
    
    // hide all panels, then show the one we want
    public void ShowOnly(PanelType panelType)
    {
        HideAll();
        Show(panelType);
    }
    
    private void HideAll()
    {
        foreach (KeyValuePair<PanelType,CanvasGroup> keyValuePair in panels)
        {
            CanvasGroupDisplayer.Hide(keyValuePair.Value);
        }
    }
    
    public void OnClickReturnToMainMenu()
    {
        ShowOnly(PanelType.MAIN);
    }
    
    public void OnClickShowInstructions()
    {
        ShowOnly(PanelType.INSTRUCTIONS);
    }
    
}