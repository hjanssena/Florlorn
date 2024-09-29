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
    protected int movDirection;
    protected float movementDuration;
    protected float movementStart;

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

    protected void Move()
    {
        transform.position = new Vector3(transform.position.x + (movDirection * Time.deltaTime * xMovement), transform.position.y, transform.position.z);
    }

    protected void SetPath()
    {
        if (movementDuration + movementStart < Time.time)
        {
            movementDuration = Random.Range(2, 7);
            movementStart = Time.time;

            movDirection = Random.Range(-1, 2);
        }

        float distanceFromOrigin = Vector2.Distance(transform.position, startingPosition);

        if(distanceFromOrigin > maxDistanceFromOrigin)
        {
            movementDuration = 0;
        }
        if(movDirection == 1)
        {
            if (!floorOnRight || wallOnRight)
            {
                movementDuration = 0;
            }
        }
        if(movDirection == -1)
        {
            if (!floorOnLeft || wallOnLeft)
            {
                movementDuration = 0;
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
}
