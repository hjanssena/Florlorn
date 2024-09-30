using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class MainMenu : MonoBehaviour
{
    AudioSource audioPlayer;
    [SerializeField] AudioClip enterSound;
    Image black;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        TimeKeeping.totalTimes = new List<System.TimeSpan>();
        black = GameObject.Find("Black").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Begin());
        }
    }

    IEnumerator Begin()
    {
        audioPlayer.PlayOneShot(enterSound);
        do
        {
            yield return new WaitForSeconds(.01f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a + .0027f);
        } while (black.color.a < 1);

        SceneManager.LoadScene("Sandbox");
    }
}
