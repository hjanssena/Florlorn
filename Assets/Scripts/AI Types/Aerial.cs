using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aerial : Animal
{
    [Header("Vertical oscilation")]
    [SerializeField] float yPath;
    [SerializeField][Range(0f, 1)] float movementFactor;
    [SerializeField] float period = 2f;
    Rigidbody2D rb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        wallOnLeft = CheckLeftWalls();
        wallOnRight = CheckRightWalls();
        spikeOnLeft = CheckLeftSpikes();
        spikeOnRight = CheckRightSpikes();
        floorOnLeft = true;
        floorOnRight = true;

        animator.SetBool("isRunning", false);
        SetPath();
        Move();
        VerticalOscilation();
    }

    void VerticalOscilation()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = (rawSinWave + 1f) / 2;
        float offset = yPath * movementFactor;
        transform.position = new Vector3(transform.position.x, startingPosition.y + offset, transform.position.z);
    }
}
