using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    //Si no se va a cambiar algun parametro de la camara, dejar en 0.
    //Explicación de las variables en el script de CameraController

    public cameraMode camMode;
    [SerializeField] GameObject target;

    [SerializeField] float wantedX;
    [SerializeField] float wantedY;

    [SerializeField] float offsetX;
    [SerializeField] float offsetY;

    [SerializeField] float xLimit;
    [SerializeField] float yLimit;

    [SerializeField] float size;
    [SerializeField] float sizeChangeSpeed;

    CameraController camController;

    void Start()
    {
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            camController.ChangeCameraMode(camMode, wantedX, wantedY, target);

            if (offsetX > 0 || offsetX < 0)
            {
                camController.ChangeOffsetX(offsetX);
            }
            if (offsetY > 0 || offsetY < 0)
            {
                camController.ChangeOffsetY(offsetY);
            }

            if(xLimit > 0 || xLimit < 0)
            {
                camController.xLimit = xLimit;
            }

            if (xLimit > 0 || xLimit < 0)
            {
                camController.yLimit = yLimit;
            }

            if (size > 0)
            {
                camController.ChangeCameraSize(size, sizeChangeSpeed);
            }
        }
    }
}