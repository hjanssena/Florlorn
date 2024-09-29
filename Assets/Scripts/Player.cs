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
    public bool dead;

    CircleCollider2D coll;

    void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        Initiate();
    }

    public void Initiate()
    {
        stasisStart = float.MaxValue;
        transform.parent = null;
        onStasis = true;
        migrating = false;
        dead = false;
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
            if (CheckShotCollision())
            {
                dead = true;
            }
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
            transform.parent.GetComponent<Host>().ExpireHost();
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

    protected bool CheckShotCollision()
    {
        LayerMask mask;
        mask = (1 << 6);

        //Centro, izquierda y derecha
        RaycastHit2D hitU = Physics2D.Raycast(transform.position, Vector2.up, .05f, mask);
        RaycastHit2D hitD = Physics2D.Raycast(transform.position, Vector2.down, .05f, mask);
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.down, .05f, mask);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.down, .05f, mask);

        if (hitU || hitD || hitL || hitR)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.collider.gameObject;
        if (collided.tag == "Animal" && migrating)
        {
            MigrateHost(collided);
        }
    }
}
