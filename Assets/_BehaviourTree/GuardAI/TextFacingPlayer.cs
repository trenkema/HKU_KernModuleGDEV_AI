using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFacingPlayer : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam != null)
        {
            transform.LookAt(cam.transform);
            transform.Rotate(Vector3.up * 180);
        }
        else
        {
            cam = Camera.main;
        }
    }
}
