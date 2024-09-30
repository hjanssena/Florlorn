using UnityEngine;

public enum cameraMode { xScroll, yScroll, free, locked }

public class CameraController : MonoBehaviour
{
    public cameraMode camMode;
    Camera cam;
    private bool moving;
    public float speed; //Velocidad de movimiento de la camara
    //Que tanto se puede mover el target antes de que la camara se mueva en ambos ejes
    //Es basicamente la deadzone
    public float xLimit; 
    public float yLimit; 
    //Distancia que se le incrementa o decrementa a la posicion del target
    //Con estos podemos posicionar la camara de maneras mas cinematicas lol
    public float xOffset; 
    public float yOffset; 
    //Se utilizan en los modos de camara en donde un eje es estatico
    //Es el punto en el eje en el que queremos que la camara se bloquee
    public float wantedX;
    public float wantedY;

    private Vector3 wantedPosition;
    [SerializeField] GameObject target;

    public float shakeMagnitude; //Cuanto quieres sacudirla
    public float shakeDuration; //Cuanto tiempo la vas a sacudir
    public float dampingSpeed; //Que tan rapido se evapora la sacudida

    //Variables de cambio de tamaño de la camara
    private bool changingSize;
    private float sizeChangeSpeed;
    private float size;

    cameraMode ocamMode;
    GameObject otarget;

    float owantedX;
    float owantedY;

    float ooffsetX;
    float ooffsetY;

    float oxLimit;
    float oyLimit;

    float osize;
    float osizeChangeSpeed;

    void Start()
    {
        cam = GetComponent<Camera>();
        GetStartingProperties();
    }

    void GetStartingProperties()
    {
        ocamMode = camMode;
        otarget = target;

        owantedX = wantedX;
        owantedY = wantedY;

        ooffsetX = xOffset;
        ooffsetY = yOffset;

        oxLimit = xLimit;
        oyLimit = yLimit;

        osize = size;
        osizeChangeSpeed = sizeChangeSpeed;
    }

    void Update()
    {
        GetPosition();
        //AdjustToPlayerView();
        CameraShake();
    }

    void LateUpdate()
    {
        MovementLimits();

        if (changingSize)
        {
            ChangingSize();
        }
    }

    /*void AdjustToPlayerView()
    {
        if(target.name == lib.player && target.GetComponent<SpriteRenderer>().flipX)
        {
            xOffset = -1.5f;
        }
        else if (target.name == lib.player && !target.GetComponent<SpriteRenderer>().flipX)
        {
            xOffset = 1.5f;
        }
        else 
        {
            xOffset = 0;
        }
    }*/

    void MovementLimits() //Aqui se revisa si el bato esta dentro de los limites del deadzone
    {
        if (transform.position.x > wantedPosition.x + xLimit && !moving)
        {
            moving = true;
            CameraMovement();
        }

        if (transform.position.x < wantedPosition.x - xLimit && !moving)
        {
            moving = true;
            CameraMovement();
        }

        if (transform.position.y > wantedPosition.y + yLimit && !moving)
        {
            moving = true;
            CameraMovement();
        }

        if (transform.position.y < wantedPosition.y - yLimit && !moving)
        {
            moving = true;
            CameraMovement();
        }

        if (moving) //El booleano es para que la camara siga moviendose un poco despues de que inicie su movimiento, de lo contrario su movimiento era choppy.
        {
            if (transform.position.x > wantedPosition.x + (xLimit * .25f))
            {
                CameraMovement();
            }

            else if (transform.position.x < wantedPosition.x - (xLimit * .25f))
            {
                CameraMovement();
            }

            else if (transform.position.y > wantedPosition.y + (yLimit * .25f))
            {
                CameraMovement();
            }

            else if (transform.position.y < wantedPosition.y - (yLimit * .25f))
            {
                CameraMovement();
            }

            else
            {
                moving = false;
            }
        }
    }

    void GetPosition() //Se consigue la posicion que se espera de la camara. Editable por medio de los offsets. Dependiendo de el modo de la camara, la posicion del eje X y Y se deben asignar por el script del nivel
    {
        switch (camMode)
        {
            case cameraMode.xScroll:
                wantedPosition = target.transform.position;
                wantedPosition.x += xOffset;
                wantedPosition.y = wantedY;
                wantedPosition.z = -10;
                break;
            case cameraMode.yScroll:
                wantedPosition = target.transform.position;
                wantedPosition.x = wantedX;
                wantedPosition.y += yOffset;
                wantedPosition.z = -10;
                break;
            case cameraMode.free:
                wantedPosition = target.transform.position;
                wantedPosition.x += xOffset;
                wantedPosition.y += yOffset;
                wantedPosition.z = -10;
                break;
            case cameraMode.locked:
                wantedPosition.x = wantedX;
                wantedPosition.y = wantedY;
                wantedPosition.z = -10;
                break;
        }
    }

    private void CameraShake()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = wantedPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
        }
    }

    public void StartShake(float shakeMagnitude, float shakeDuration, float dampingSpeed)
    {
        this.shakeMagnitude = shakeMagnitude;
        this.shakeDuration = shakeDuration;
        this.dampingSpeed = dampingSpeed;
    }

    void CameraMovement() //Se mueve la camara
    {
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * speed);
    }

    public void ChangeOffsetX(float quantity) //Funcion que hice por si queremos cambiar el offset de la camara desde otro script facilmente (eje x)
    {
        xOffset = quantity;
    }

    public void ChangeOffsetY(float quantity) //Funcion que hice por si queremos cambiar el offset de la camara desde otro script facilmente (eje y)
    {
        yOffset = quantity;
    }

    public void ChangeCameraMode(cameraMode camMode, float wantedX, float wantedY, GameObject target)
    {
        this.camMode = camMode;

        switch (camMode)
        {
            case cameraMode.free:
                this.target = target;
                break;
            case cameraMode.locked:
                this.wantedX = wantedX;
                this.wantedY = wantedY;
                break;
            case cameraMode.xScroll:
                this.wantedY = wantedY;
                this.target = target;
                break;
            case cameraMode.yScroll:
                this.wantedX = wantedX;
                this.target = target;
                break;
        }
    }

    private void ChangingSize()
    {
        if(size > cam.orthographicSize)
        {
            cam.orthographicSize += Time.deltaTime * sizeChangeSpeed;

            if (cam.orthographicSize >= size - .001f)
            {
                changingSize = false;
            }
        }

        else if(size < cam.orthographicSize)
        {
            cam.orthographicSize -= Time.deltaTime * sizeChangeSpeed;

            if (cam.orthographicSize <= size - .001f)
            {
                changingSize = false;
            }
        }
    }

    public void ChangeCameraSize(float size, float speed)
    {
        changingSize = true;
        this.size = size;
        sizeChangeSpeed = speed;
    }

    public void ResetValues()
    {
        camMode = ocamMode;
        target = otarget;

        wantedX = owantedX;
        wantedY = owantedY;

        xOffset = ooffsetX;
        yOffset = ooffsetY;

        xLimit = oxLimit;
        yLimit = oyLimit;

        size = osize;
        sizeChangeSpeed = osizeChangeSpeed;
    }
}
