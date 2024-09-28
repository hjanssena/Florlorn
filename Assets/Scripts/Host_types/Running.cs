using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : Host
{
    float moveDirection;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        lastPosition = transform.position;
        audioPlayer = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();

        if (sprite.flipX == true)
        {
            moveDirection = -1;
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
        else if (Input.GetAxis("Horizontal") < 0f)
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
}