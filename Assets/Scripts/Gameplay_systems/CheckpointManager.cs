using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    SpriteRenderer black;
    GameObject player;
    Vector2 playerInitialPosition;
    GameObject[] animals;
    List<Vector2> animalInitialPositions;
    bool resetting;

    void Start()
    {
        black = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<SpriteRenderer>();
        RegisterPlayer();
        RegisterAnimals();
        resetting = false;
    }

    void Update()
    {
        if (player.GetComponent<Player>().dead && !resetting)
        {
            resetting = true;
            StartCoroutine(ResetLevel());
        }
    }

    void RegisterPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInitialPosition = player.transform.position;
    }

    void RegisterAnimals()
    {
        animals = GameObject.FindGameObjectsWithTag("Animal");
        animalInitialPositions = new List<Vector2>();
        foreach(GameObject animal in animals)
        {
            animalInitialPositions.Add(animal.transform.position);
        }
    }

    public IEnumerator ResetLevel()
    {
        do
        {
            yield return new WaitForSeconds(.008f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a + .045f);
        } while (black.color.a < 1);
        
        player.transform.position = playerInitialPosition;
        player.SetActive(false);
        player.GetComponent<Player>().Initiate();

        foreach (GameObject flower in GameObject.FindGameObjectsWithTag("Flower"))
        {
            flower.SetActive(false);
            flower.transform.parent = null;
        }

        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive(false);
            animals[i].transform.rotation = Quaternion.identity;
            animals[i].transform.position = animalInitialPositions[i];
            animals[i].GetComponent<Animator>().SetBool("isDead", false);
            animals[i].GetComponent<Animator>().SetBool("isRunning", false);
            animals[i].GetComponent<Animator>().SetBool("isJumping", false);
            animals[i].GetComponent<Host>().isDead = false;
            animals[i].GetComponent<Host>().enabled = false;
            animals[i].GetComponent<Animal>().enabled = true;
            animals[i].GetComponent<Animal>().SetMovDuration(0);
            animals[i].AddComponent<Rigidbody2D>();
        }

        yield return new WaitForSeconds(1f);

        foreach (GameObject animal in animals)
        {
            animal.SetActive(true);
        }
        player.SetActive(true);

        do
        {
            yield return new WaitForSeconds(.008f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a - .045f);
        } while (black.color.a > 0);

        resetting = false;
    }
}
