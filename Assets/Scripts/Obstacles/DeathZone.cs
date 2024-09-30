using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Animal" && collision.gameObject.GetComponent<Host>().enabled == true && !collision.gameObject.GetComponent<Host>().isDead)
        {
            collision.gameObject.GetComponent<Host>().Death();
        }
        else if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().dead = true;
        }
    }
}
