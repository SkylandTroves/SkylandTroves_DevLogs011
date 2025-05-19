using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    private SceneController sc;
    public void OnButtonClick()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.StopLoopingSound("LevelWind");
        }
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
        
    }
}
