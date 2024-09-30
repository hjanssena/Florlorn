using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    Image black;

    // Start is called before the first frame update
    void Start()
    {
        black = GameObject.Find("Black").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(End());
        }
    }

    IEnumerator End()
    {
        do
        {
            yield return new WaitForSeconds(.01f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a + .012f);
        } while (black.color.a < 1);
        try
        {
            SceneManager.MoveGameObjectToScene(GameObject.Find("MusicPlayer"), SceneManager.GetActiveScene());
        }
        catch
        {

        }
        SceneManager.LoadScene("MainMenu");
    }
}
