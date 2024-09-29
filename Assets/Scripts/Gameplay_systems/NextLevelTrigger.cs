using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class NextLevelTrigger : MonoBehaviour
{
    void NextLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            NextLevel();
        }
    }
}
