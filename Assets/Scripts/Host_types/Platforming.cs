using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforming : Host
{
    private void OnEnable()
    {
        audioPlayer = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        lastPosition = transform.position;
        audioPlayer = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDead)
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

            if (!onFloor)
            {
                animator.SetBool("isJumping", true);
            }
            else
            {
                animator.SetBool("isJumping", false);
            }
        }
        else
        {
            onFloor = CheckOnFloor();
            rightWall = CheckRightWalls();
            leftWall = CheckLeftWalls();
            ceiling = CheckCeiling();
            animator.SetBool("isDead", true);
            currentXSpeed = 0;
            Gravity();
            transform.Translate(currentXSpeed * delta, currentYSpeed * delta, 0);
        }
    }

    protected override void Movement()
    {
        float xAxis = Input.GetAxis("Horizontal");
        animator.SetBool("isRunning", true);

        if (xAxis > 0)
        {
            sr.flipX = false;
        }
        else if (xAxis < 0)
        {
            sr.flipX = true;
        }

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
                    currentXSpeed += moveSpeed * stopMoveDrag * xAxis * delta;
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
                    currentXSpeed += moveSpeed * stopMoveDrag * xAxis * delta;
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
}
