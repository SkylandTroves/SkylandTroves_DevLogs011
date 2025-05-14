using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    private SceneController sc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

    public void OnButtonClick()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.StopLoopingSound("LevelWind");
        }
        
        Application.Quit();
        
    }
}
