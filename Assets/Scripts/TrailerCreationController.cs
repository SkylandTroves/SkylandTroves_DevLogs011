using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TrailerCreationController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Vector3 dragOrigin;

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleMouseDrag();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down
        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    void HandleZoom()
    {
        Camera cam = Camera.main;

        if (Input.GetKey(KeyCode.Q))
        {
            AdjustZoom(zoomSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            AdjustZoom(-zoomSpeed * Time.deltaTime);
        }

        void AdjustZoom(float delta)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta, minZoom, maxZoom);
            }
            else
            {
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - delta, minZoom, maxZoom);
            }
        }
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-difference.x * moveSpeed, 0, -difference.y * moveSpeed);

            transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }
    }
}

