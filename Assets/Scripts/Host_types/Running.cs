using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : Host
{
    float delta;

    //Movement on x
    [Header("Movement on x")]
    [SerializeField] float moveSpeed;
    float currentXSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float maxAirSpeed;
    [SerializeField] float airMovementPenalty;
    bool adjustedToFloor;
    Vector2 lastPosition;
    float moveDirection;

    //Movement on y
    [Header("Movement on y")]
    [SerializeField] float gravity;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float jumpStartSpeed;
    [SerializeField] float jumpSustainedSpeed;
    [SerializeField] float maxJumpForce;
    float jumpForceApplied;
    float currentYSpeed;
    bool jumping;
    bool jumpInUse = false;
    [SerializeField] float jumpBuffer;
    bool jumpBuffered;
    float jumpPressed;

    //Raycast Positions for wall and floor detection
    [Header("Raycasting for wall collisions")]
    [SerializeField] float raycastLateralLenght;
    [SerializeField] float raycastLateralOffset;
    [SerializeField] float raycastVerticalLenght;
    [SerializeField] float raycastVerticalOffset;

    //Walls and floor detection
    bool onFloor;
    bool rightWall;
    bool leftWall;
    bool ceiling;



    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        lastPosition = transform.position;
        audioPlayer = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();

        if(sprite.flipX == true)
        {
            moveDirection = -   1;
        }
        else
        {
            moveDirection = 1;
        }
    }

    void Update()
    {
        CheckLifeTime();

        delta = Time.unscaledDeltaTime * Time.timeScale;
        onFloor = CheckOnFloor();
        rightWall = CheckRightWalls();
        leftWall = CheckLeftWalls();
        ceiling = CheckCeiling();

        //xMovement
        if (Input.GetAxis("Horizontal") > 0f)
        {
            moveDirection = 1;
        }
        else if(Input.GetAxis("Horizontal") < 0f)
        {
            moveDirection = -1;
        }
        Movement();

        //yMovement
        Gravity();
        if (jumpPressed + jumpBuffer < Time.time)
        {
            jumpBuffered = false;
        }
        if (Input.GetAxis("Jump") > 0 && !jumpBuffered && !jumpInUse) //The buffer is to let the players input a jump some milliseconds before touching ground
        {
            jumpPressed = Time.time;
            jumpBuffered = true;
            jumpInUse = true;//JumpInUse is to make the getaxis act like a getkeydown
        }
        if (Input.GetAxis("Jump") > 0)
        {
            Jump();
        }
        else
        {
            jumping = false;
            jumpBuffered = false;
            jumpInUse = false;
        }

        //movement
        ApplyMovementLimits();
        lastPosition = transform.position;
        transform.Translate(currentXSpeed * delta, currentYSpeed * delta, 0);
    
    }

    protected override void Movement()
    {
        if (!onFloor) //Movement while on air
        {
            if (moveDirection > 0)
            {
                if (currentXSpeed < 0)
                {
                    currentXSpeed += moveSpeed * moveDirection * airMovementPenalty * delta;
                }
                else if (currentXSpeed < maxAirSpeed)
                {
                    currentXSpeed += moveSpeed * moveDirection * airMovementPenalty * delta;
                }
            }
            else
            {
                if (currentXSpeed > 0)
                {
                    currentXSpeed += moveSpeed * moveDirection * airMovementPenalty * delta;
                }
                else if (currentXSpeed > -maxAirSpeed)
                {
                    currentXSpeed += moveSpeed * moveDirection * airMovementPenalty * delta;
                }
            }
            //Max speed limit check
            if (currentXSpeed >= maxAirSpeed)
            {
                currentXSpeed = maxAirSpeed * moveDirection;
            }
            else if (currentXSpeed <= -maxAirSpeed)
            {
                currentXSpeed = maxAirSpeed * moveDirection;
            }
        }
        else //Movement when standing on floor
        {
            if (moveDirection > 0)
            {
                if (currentXSpeed < 0)
                {
                    currentXSpeed += moveSpeed * 1.5f * moveDirection * delta;
                }
                else if (currentXSpeed < maxSpeed)
                {
                    currentXSpeed += moveSpeed * moveDirection * delta;
                }
            }
            else
            {
                if (currentXSpeed > 0)
                {
                    currentXSpeed += moveSpeed * 1.5f * moveDirection * delta;
                }
                else if (currentXSpeed > -maxSpeed)
                {
                    currentXSpeed += moveSpeed * moveDirection * delta;
                }
            }
            //Max speed limit check
            if (currentXSpeed >= maxSpeed)
            {
                currentXSpeed = maxSpeed * moveDirection;
            }
            else if (currentXSpeed <= -maxSpeed)
            {
                currentXSpeed = maxSpeed * moveDirection;
            }
        }
    }

    void StopMovement() //When player is not pressing horizontal axis, apply force to stop the movement
    {
        float direction = 0;
        if (transform.position.x > lastPosition.x)
        {
            direction = 1;
        }
        else if (transform.position.x < lastPosition.x)
        {
            direction = -1;
        }

        if (onFloor)
        {
            if (currentXSpeed <= .05f && currentXSpeed >= -.05f)
            {
                currentXSpeed = 0;
            }
            else
            {
                currentXSpeed -= moveSpeed * 1.5f * direction * delta;
            }
        }
        else if (!onFloor)
        {
            if (currentXSpeed <= .05f && currentXSpeed >= -.05f)
            {
                currentXSpeed = 0;
            }
            else
            {
                currentXSpeed -= moveSpeed * airMovementPenalty * direction * delta;
            }
        }
    }

    void Gravity() //Applied every frame when not standing on floor
    {
        if (!onFloor)
        {
            currentYSpeed -= gravity * delta;
            if (currentYSpeed <= -maxFallSpeed)
            {
                currentYSpeed = -maxFallSpeed;
            }
        }
        else if (onFloor)
        {
            currentYSpeed = 0;
        }
    }

    void Jump()
    {
        //Initial jump push
        if (CheckForJump() && !jumping && jumpBuffered)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .003f);
            currentYSpeed = jumpStartSpeed;
            jumpForceApplied = 0;
            jumping = true;
            jumpBuffered = false;
        }
        //Aditional force if player holds jump axis
        if (jumping && maxJumpForce > jumpForceApplied + (jumpSustainedSpeed * delta))
        {
            jumpForceApplied += jumpSustainedSpeed * delta;
            currentYSpeed += jumpSustainedSpeed * delta;
        }
        //Add remaining force if player is still holding jump
        else if (jumping && maxJumpForce > jumpForceApplied)
        {
            float remainingForce = maxJumpForce - jumpForceApplied;
            jumpForceApplied += remainingForce;
            currentYSpeed += remainingForce;
            jumping = false;
        }
    }

    void ApplyMovementLimits() //If walls or ceiling is detected, stop movement towards them
    {
        if (rightWall && currentXSpeed > 0 && !onFloor && currentYSpeed <= 0)
        {
            currentXSpeed = 0;
        }
        else if (rightWall && currentXSpeed > 0)
        {
            currentXSpeed = 0;
        }
        if (leftWall && currentXSpeed < 0 && !onFloor && currentYSpeed <= 0)
        {
            currentXSpeed = 0;
        }
        else if (leftWall && currentXSpeed < 0)
        {
            currentXSpeed = 0;
        }
        if (ceiling && currentYSpeed > 0)
        {
            currentYSpeed = 0;
        }

    }

    //WALL AND FLOOR DETECTION FUNCTIONS
    bool CheckOnFloor()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 leftRayPos = new Vector2(transform.position.x - raycastVerticalOffset, transform.position.y);
        Vector2 rightRayPos = new Vector2(transform.position.x + raycastVerticalOffset, transform.position.y);

        //Centro, izquierda y derecha
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.down, raycastVerticalLenght, mask);
        RaycastHit2D hitL = Physics2D.Raycast(leftRayPos, Vector2.down, raycastVerticalLenght, mask);
        RaycastHit2D hitR = Physics2D.Raycast(rightRayPos, Vector2.down, raycastVerticalLenght, mask);

        if (hitC)
        {
            if (!adjustedToFloor)
            {
                AdjustToFloor(hitC);
            }
            return true;
        }
        else if (hitL)
        {
            if (!adjustedToFloor)
            {
                AdjustToFloor(hitL);
            }
            return true;
        }
        else if (hitR)
        {
            if (!adjustedToFloor)
            {
                AdjustToFloor(hitR);
            }
            return true;
        }
        else
        {
            adjustedToFloor = false;
            return false;
        }
    }

    bool CheckForJump()//Only to check if you can jump, its different from the normal floor check to make the input feel more responsive
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 leftRayPos = new Vector2(transform.position.x - raycastVerticalOffset, transform.position.y);
        Vector2 rightRayPos = new Vector2(transform.position.x + raycastVerticalOffset, transform.position.y);

        //Centro, izquierda y derecha
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.down, raycastVerticalLenght + 0.5f, mask);
        RaycastHit2D hitL = Physics2D.Raycast(leftRayPos, Vector2.down, raycastVerticalLenght + 0.5f, mask);
        RaycastHit2D hitR = Physics2D.Raycast(rightRayPos, Vector2.down, raycastVerticalLenght+0.5f, mask);

        if (hitC || hitL || hitR)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void AdjustToFloor(RaycastHit2D hit)
    {
        transform.position = new Vector2(transform.position.x, hit.point.y + raycastVerticalLenght);
        adjustedToFloor = true;
    }

    bool CheckRightWalls()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset/2);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset/2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.right, raycastLateralLenght, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.right, raycastLateralLenght, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.right, raycastLateralLenght, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.right, raycastLateralLenght, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.right, raycastLateralLenght, mask);

        if (hitC)
        {
            AdjustToWall(hitC, -1);
            return true;
        }
        else if (hitUC)
        {
            AdjustToWall(hitUC, -1);
            return true;
        }
        else if (hitLC)
        {
            AdjustToWall(hitLC, -1);
            return true;
        }
        else if (hitU)
        {
            AdjustToWall(hitU, -1);
            return true;

        }
        else if (hitL)
        {
            AdjustToWall(hitL, -1);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckLeftWalls()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset/2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.left, raycastLateralLenght, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.left, raycastLateralLenght, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.left, raycastLateralLenght, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.left, raycastLateralLenght, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.left, raycastLateralLenght, mask);

        if (hitC)
        {
            AdjustToWall(hitC, 1);
            return true;
        }
        else if (hitUC)
        {
            AdjustToWall(hitUC, 1);
            return true;
        }
        else if (hitLC)
        {
            AdjustToWall(hitLC, 1);
            return true;
        }
        else if (hitU)
        {
            AdjustToWall(hitU, 1);
            return true;

        }
        else if (hitL)
        {
            AdjustToWall(hitL, 1);
            return true;
        }
        else
        {
            return false;
        }
    }

    void AdjustToWall(RaycastHit2D hit, int dir)
    {
        transform.position = new Vector2(hit.point.x + .055f * dir, transform.position.y);
    }

    bool CheckCeiling()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 leftRayPos = new Vector2(transform.position.x - raycastVerticalOffset, transform.position.y);
        Vector2 rightRayPos = new Vector2(transform.position.x + raycastVerticalOffset, transform.position.y);

        //Centro, izquierda y derecha
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.up, raycastVerticalLenght, mask);
        RaycastHit2D hitL = Physics2D.Raycast(leftRayPos, Vector2.up, raycastVerticalLenght, mask);
        RaycastHit2D hitR = Physics2D.Raycast(rightRayPos, Vector2.up, raycastVerticalLenght, mask);

        if (hitC || hitL || hitR)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
