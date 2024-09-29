using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrestrial : Animal
{
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
    }

    void Update()
    {
        wallOnLeft = CheckLeftWalls();
        wallOnRight = CheckRightWalls();
        floorOnLeft = CheckLeftFloor();
        floorOnRight = CheckRightFloor();
        spikeOnLeft = CheckLeftSpikes();
        spikeOnRight = CheckRightSpikes();

        animator.SetBool("isRunning", false);
        SetPath();
        Move();
    }
}
