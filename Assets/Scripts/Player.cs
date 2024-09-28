using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float stasisDuration;
    private float stasisStart;
    [SerializeField] private float skillShotSpeed;
    private Vector2 fireDirection;

    [SerializeField] private bool onStasis;
    [SerializeField] private bool migrating;

    CircleCollider2D coll;

    void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        stasisStart = float.MaxValue;
        onStasis = true;
        migrating = false;
    }

    void Update()
    {
        if (onStasis)
        {
            AimSkillShot();
        }
        else if (migrating)
        {
            Migrating();
        }
        else
        {
            IsHostAlive();

            if (Input.GetAxis("Stasis") > 0)
            {
                EnterStasis();
            }
        }
    }

    void EnterStasis()
    {
        if (transform.parent != null)
        {
            transform.parent.GetComponent<Host>().Die();
        }
        onStasis = true;
        stasisStart = Time.time;
    }

    void AimSkillShot()
    {
        if (stasisStart + stasisDuration <= Time.time || Input.GetAxis("Fire") > 0)
        {
            Fire();
        }
    }

    void Fire()
    {
        coll.enabled = true;
        onStasis = false;
        migrating = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        fireDirection = mouseWorldPos - transform.position;
        fireDirection.Normalize();
    }

    void Migrating()
    {
        transform.Translate(fireDirection * skillShotSpeed * Time.deltaTime);
    }

    void MigrateHost(GameObject newHost)
    {
        coll.enabled = false;
        transform.SetParent(newHost.transform);
        newHost.GetComponent<Host>().SetAsHost();
        transform.position = newHost.transform.position;
        migrating = false;
    }
    
    void IsHostAlive()
    {
        if (transform.parent == null)
        {
            EnterStasis();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.collider.gameObject;
        if (collided.tag == "Animal" && migrating)
        {
            MigrateHost(collided);
        }
        else if(migrating)
        {
            //Lose logic here, respawn to checkpoint
        }
    }
}
