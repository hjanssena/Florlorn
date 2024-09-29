using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrestrial : Animal
{
    private void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        wallOnLeft = CheckLeftWalls();
        wallOnRight = CheckRightWalls();
        floorOnLeft = CheckLeftFloor();
        floorOnRight = CheckRightFloor();

        SetPath();
        Move();
    }
}
