using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float stasisDuration;
    private bool onStasis;
    private float stasisStart;
    [SerializeField] private float skillShotSpeed;

    void Start()
    {

    }

    void Update()
    {
        if (onStasis)
        {
            SkillShot();
        }
        else
        {
            IsHostAlive();

            //add if player input then enter stasis after figuring out controls
        }
    }

    void EnterStasis()
    {
        transform.GetChild(0).GetComponent<Host>().Die();
        onStasis = true;
        stasisStart = Time.time;
    }

    void Stasis()
    {

    }

    void SkillShot()
    {

    }

    void MigrateHost()
    {

    }

    void IsHostAlive()
    {
        if (transform.GetChild(0).gameObject.activeSelf == false)
        {
            EnterStasis();
        }
    }
}
