using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : Host
{

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        lastPosition = transform.position;
        audioPlayer = GetComponent<AudioSource>();
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
        if (Input.GetAxis("Horizontal") > 0f || Input.GetAxis("Horizontal") < 0f)
        {
            Movement();
        }
        else
        {
            StopMovement();
        }

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
        float xAxis = Input.GetAxis("Horizontal");
        if (!onFloor) //Movement while on air
        {
            if (xAxis > 0)
            {
                if (currentXSpeed < 0)
                {
                    currentXSpeed += moveSpeed * xAxis * airMovementPenalty * delta;
                }
                else if (currentXSpeed < maxAirSpeed)
                {
                    currentXSpeed += moveSpeed * xAxis * airMovementPenalty * delta;
                }
            }
            else
            {
                if (currentXSpeed > 0)
                {
                    currentXSpeed += moveSpeed * xAxis * airMovementPenalty * delta;
                }
                else if (currentXSpeed > -maxAirSpeed)
                {
                    currentXSpeed += moveSpeed * xAxis * airMovementPenalty * delta;
                }
            }
            //Max speed limit check
            if (currentXSpeed >= maxAirSpeed)
            {
                currentXSpeed = maxAirSpeed * xAxis;
            }
            else if (currentXSpeed <= -maxAirSpeed)
            {
                currentXSpeed = maxAirSpeed * xAxis;
            }
        }
        else //Movement when standing on floor
        {
            if (xAxis > 0)
            {
                if (currentXSpeed < 0)
                {
                    currentXSpeed += moveSpeed * 1.5f * xAxis * delta;
                }
                else if (currentXSpeed < maxSpeed)
                {
                    currentXSpeed += moveSpeed * xAxis * delta;
                }
            }
            else
            {
                if (currentXSpeed > 0)
                {
                    currentXSpeed += moveSpeed * 1.5f * xAxis * delta;
                }
                else if (currentXSpeed > -maxSpeed)
                {
                    currentXSpeed += moveSpeed * xAxis * delta;
                }
            }
            //Max speed limit check
            if (currentXSpeed >= maxSpeed)
            {
                currentXSpeed = maxSpeed * xAxis;
            }
            else if (currentXSpeed <= -maxSpeed)
            {
                currentXSpeed = maxSpeed * xAxis;
            }
        }
    }

    protected override bool CheckForJump()//Only to check if you can jump, its different from the normal floor check to make the input feel more responsive
    {
        return true;
    }
}