using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float yPos = 15;
    public float zoomSpeed = 2;
    public float lerpSpeed = 1;
    private Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = new Vector3(0, yPos, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        if(vert != 0 || hor != 0)
        {
            targetPos += (Vector3.forward * vert + hor * Vector3.right).normalized * moveSpeed;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            yPos += -Mathf.Sign(scroll) * zoomSpeed;
            yPos = Mathf.Clamp(yPos, 1, 100);
            targetPos = new Vector3(targetPos.x, yPos, targetPos.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
