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
}
