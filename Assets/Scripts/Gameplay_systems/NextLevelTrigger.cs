using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

[RequireComponent(typeof(BoxCollider2D))]
public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] string nextLevel;
    SpriteRenderer black;

    private void Start()
    {
        black = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<SpriteRenderer>();
    }

    IEnumerator NextLevel()
    {
        do
        {
            yield return new WaitForSeconds(.001f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a + .01f);
        } while (black.color.a < 1);
        SceneManager.LoadScene(nextLevel);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(NextLevel());
        }
        if (collision.gameObject.tag == "Animal" && collision.gameObject.GetComponent<Host>().enabled == true)
        {
            StartCoroutine(NextLevel());
        }
    }
}
