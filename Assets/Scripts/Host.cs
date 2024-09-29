using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Host : MonoBehaviour
{
    [SerializeField] private float lifeDuration;
    private float lifeStart;
    protected Rigidbody2D rb;
    protected BoxCollider2D coll;

    protected float delta;

    //Movement on x
    [Header("Movement on x")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float currentXSpeed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxAirSpeed;
    [SerializeField] protected float airMovementPenalty;
    [SerializeField] protected float stopMoveDrag;
    protected bool adjustedToFloor;
    protected Vector2 lastPosition;

    //Movement on y
    [Header("Movement on y")]
    [SerializeField] protected float gravity;
    [SerializeField] protected float maxFallSpeed;
    [SerializeField] protected float jumpStartSpeed;
    [SerializeField] protected float jumpSustainedSpeed;
    [SerializeField] protected float maxJumpForce;
    protected float jumpForceApplied;
    protected float currentYSpeed;
    protected bool jumping;
    protected bool jumpInUse = false;
    [SerializeField] protected float jumpBuffer;
    protected bool jumpBuffered;
    protected float jumpPressed;



    //Walls and floor detection
    protected bool onFloor;
    protected bool rightWall;
    protected bool leftWall;
    protected bool ceiling;

    //Raycast Positions for wall and floor detection
    [Header("Raycasting for wall collisions")]
    [SerializeField] protected float raycastLateralLenght;
    [SerializeField] protected float raycastLateralOffset;
    [SerializeField] protected float raycastVerticalLenght;
    [SerializeField] protected float raycastVerticalOffset;



    //Sounds
    [Header("Sound")]
    //[SerializeField] AudioClip stepSound;
    //[SerializeField] AudioClip jumpSound;
    protected AudioSource audioPlayer;
    protected SpriteRenderer sprite;

    protected abstract void Movement();

    protected void CheckLifeTime()
    {
        if (lifeDuration + lifeStart < Time.time)
        {
            Die();
        }
    }

    public void SetAsHost()
    {
        GetComponent<Host>().enabled = true;
        GetComponent<Animal>().enabled = false;
        lifeStart = Time.time;
    }

    public void Die()
    {
        GameObject.FindGameObjectWithTag("Player").transform.parent = null;
        gameObject.SetActive(false);
    }


    protected void StopMovement() //When player is not pressing horizontal axis, apply force to stop the movement
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
            if (currentXSpeed <= .02f && currentXSpeed >= -.06f)
            {
                currentXSpeed = 0;
            }
            else
            {
                currentXSpeed -= stopMoveDrag * 2 * direction * delta;
            }
        }
        else if (!onFloor)
        {
            if (currentXSpeed <= .02f && currentXSpeed >= -.06f)
            {
                currentXSpeed = 0;
            }
            else
            {
                currentXSpeed -= stopMoveDrag * 2 * airMovementPenalty * direction * delta;
            }
        }
    }

    protected void Gravity() //Applied every frame when not standing on floor
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

    protected void Jump()
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

    protected void ApplyMovementLimits() //If walls or ceiling is detected, stop movement towards them
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
    protected bool CheckOnFloor()
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

    protected virtual bool CheckForJump()//Only to check if you can jump, its different from the normal floor check to make the input feel more responsive
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 leftRayPos = new Vector2(transform.position.x - raycastVerticalOffset, transform.position.y);
        Vector2 rightRayPos = new Vector2(transform.position.x + raycastVerticalOffset, transform.position.y);

        //Centro, izquierda y derecha
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.down, raycastVerticalLenght + 0.05f, mask);
        RaycastHit2D hitL = Physics2D.Raycast(leftRayPos, Vector2.down, raycastVerticalLenght + 0.05f, mask);
        RaycastHit2D hitR = Physics2D.Raycast(rightRayPos, Vector2.down, raycastVerticalLenght + 0.05f, mask);

        if (hitC || hitL || hitR)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void AdjustToFloor(RaycastHit2D hit)
    {
        transform.position = new Vector2(transform.position.x, hit.point.y + raycastVerticalLenght);
        adjustedToFloor = true;
    }

    protected bool CheckRightWalls()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset / 2);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset / 2);
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

    protected bool CheckLeftWalls()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastLateralOffset);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastLateralOffset / 2);
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

    protected void AdjustToWall(RaycastHit2D hit, int dir)
    {
        transform.position = new Vector2(hit.point.x + .055f * dir, transform.position.y);
    }

    protected bool CheckCeiling()
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
