using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float xMovement;
    [SerializeField] protected float maxDistanceFromOrigin;
    protected Vector2 startingPosition;
    [SerializeField] protected int movDirection;
    [SerializeField] protected float movementDuration;
    protected float movementStart;
    protected bool repositioning;

    //Raycast Positions for floor detection
    [Header("Raycasting for floor detection")]
    [SerializeField] protected float raycastFloorLenght;
    [SerializeField] protected float raycastFloorOffset;

    //Raycast Positions for wall detection
    [Header("Raycasting for wall detection")]
    [SerializeField] protected float raycastWallLength;
    [SerializeField] protected float raycastWallOffset;

    protected bool floorOnRight;
    protected bool floorOnLeft;
    protected bool wallOnRight;
    protected bool wallOnLeft;
    protected bool spikeOnRight;
    protected bool spikeOnLeft;

    protected Animator animator;
    protected SpriteRenderer sr;

    protected void Move()
    {
        if (movDirection == 1)
        {
            sr.flipX = false;
            animator.SetBool("isRunning", true);
        }
        else if (movDirection == -1)
        {
            sr.flipX = true;
            animator.SetBool("isRunning", true);
        }
        transform.position = new Vector3(transform.position.x + (movDirection * Time.deltaTime * xMovement), transform.position.y, transform.position.z);
    }

    protected void SetPath()
    {
        if(maxDistanceFromOrigin != 0)
        {
            if (!repositioning)
            {
                if (movementDuration + movementStart < Time.time)
                {
                    movementDuration = Random.Range(1, 4);
                    movementStart = Time.time;
                    movDirection = Random.Range(-1, 2);
                }

                float distanceFromOrigin = Vector2.Distance(transform.position, startingPosition);
                if (distanceFromOrigin > maxDistanceFromOrigin)
                {
                    movementDuration = 2;
                    movDirection = -movDirection;
                    repositioning = true;
                }

                if (movDirection == 1)
                {
                    if (!floorOnRight || wallOnRight || spikeOnRight)
                    {
                        movementDuration = 0;
                    }
                }
                if (movDirection == -1)
                {
                    if (!floorOnLeft || wallOnLeft || spikeOnLeft)
                    {
                        movementDuration = 0;
                    }
                }
            }
            else
            {
                if (movementDuration + movementStart < Time.time)
                {
                    movementDuration = Random.Range(1, 4);
                    movementStart = Time.time;

                    if (transform.position.x - startingPosition.x > 0)
                    {
                        movDirection = Random.Range(0, 2);
                    }
                    else
                    {
                        movDirection = Random.Range(-1, 1);
                    }

                    repositioning = false;
                }
            }
        }
    }

    protected bool CheckLeftFloor()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 leftRayPos = new Vector2(transform.position.x - raycastFloorOffset, transform.position.y);
        RaycastHit2D hitL = Physics2D.Raycast(leftRayPos, Vector2.down, raycastFloorLenght, mask);

        if (hitL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckRightFloor()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 rightRayPos = new Vector2(transform.position.x + raycastFloorOffset, transform.position.y);
        RaycastHit2D hitR = Physics2D.Raycast(rightRayPos, Vector2.down, raycastFloorLenght, mask);

        if (hitR)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckRightWalls()
    {
        LayerMask mask;
        mask = (1 << 6);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset / 2);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset / 2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.right, raycastWallLength, mask);

        if (hitC || hitUC || hitLC || hitU || hitL)
        {
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

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset / 2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.left, raycastWallLength, mask);

        if (hitC || hitUC || hitLC || hitU || hitL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckRightSpikes()
    {
        LayerMask mask;
        mask = (1 << 8);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset / 2);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset / 2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.right, raycastWallLength, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.right, raycastWallLength, mask);

        if (hitC || hitUC || hitLC || hitU || hitL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckLeftSpikes()
    {
        LayerMask mask;
        mask = (1 << 8);

        Vector2 upperRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerUpRayPos = new Vector2(transform.position.x, transform.position.y + raycastWallOffset);
        Vector2 centerRayPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 centerLowRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset / 2);
        Vector2 lowerRayPos = new Vector2(transform.position.x, transform.position.y - raycastWallOffset);

        RaycastHit2D hitU = Physics2D.Raycast(upperRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitUC = Physics2D.Raycast(centerUpRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitC = Physics2D.Raycast(centerRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitLC = Physics2D.Raycast(centerLowRayPos, Vector2.left, raycastWallLength, mask);
        RaycastHit2D hitL = Physics2D.Raycast(lowerRayPos, Vector2.left, raycastWallLength, mask);

        if (hitC || hitUC || hitLC || hitU || hitL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMovDuration(float value)
    {
        movementDuration = value;
    }
}
