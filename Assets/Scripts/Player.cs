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
    protected ProgressBar progressBar;

    protected SpriteRenderer sr;
    [SerializeField] List<GameObject> flowers;

    CircleCollider2D coll;

    AudioSource audioPlayer;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip hitSound;

    private void OnEnable()
    {
        audioPlayer = GetComponent<AudioSource>();
        progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
        coll = GetComponent<CircleCollider2D>();
        Initiate();
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
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
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        }
        else if (migrating)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            Migrating();
            if (CheckShotCollision())
            {
                if (!dead)
                {
                    audioPlayer.PlayOneShot(deadSound);
                }
                dead = true;
            }
        }
        else
        {
            IsHostAlive();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);

            if (Input.GetKeyDown(KeyCode.Mouse0))
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
        progressBar.SetMax(stasisDuration);
    }

    void AimSkillShot()
    {
        progressBar.ChangeProgress(Time.time - stasisStart);
        if (stasisStart + stasisDuration <= Time.time || Input.GetKey(KeyCode.Mouse0))
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
        audioPlayer.PlayOneShot(shootSound);
    }

    void Migrating()
    {
        if (!dead)
        {
            transform.Translate(fireDirection * skillShotSpeed * Time.deltaTime);
        }
    }

    void MigrateHost(GameObject newHost)
    {
        coll.enabled = false;
        transform.SetParent(newHost.transform);
        newHost.GetComponent<Host>().SetAsHost();
        transform.position = newHost.transform.position;
        migrating = false;
        audioPlayer.PlayOneShot(hitSound);
    }
    
    void IsHostAlive()
    {
        if (transform.parent == null)
        {
            EnterStasis();
        }
    }

    public GameObject SpawnFlower()
    {
        int rng = Random.Range(0, 6);
        GameObject flower = Instantiate(flowers[rng]);
        flower.transform.position = transform.position + new Vector3(0, .2f, 0);
        return flower;
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
        if (collided.tag == "Animal" && migrating && !collided.GetComponent<Host>().isDead)
        {
            MigrateHost(collided);
        }
    }
}
