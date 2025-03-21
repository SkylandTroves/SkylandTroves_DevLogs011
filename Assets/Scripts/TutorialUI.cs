using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform podium; // Assign the podium in the Inspector
    public Vector3 offset;   // Adjust in Inspector for positioning
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (podium != null)
        {
            // Convert world position to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(podium.position + offset);
            rectTransform.position = screenPos;
        }
    }
}
