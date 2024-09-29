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

    void Start()
    {
        black = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<SpriteRenderer>();
        RegisterPlayer();
        RegisterAnimals();
    }

    void Update()
    {
        if (player.GetComponent<Player>().dead)
        {
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
            yield return new WaitForSeconds(.1f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a + .005f);
        } while (black.color.a < 1);
        
        player.transform.position = playerInitialPosition;
        player.SetActive(false);
        player.GetComponent<Player>().Initiate();

        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].transform.position = animalInitialPositions[i];
            animals[i].GetComponent<Host>().enabled = false;
            animals[i].GetComponent<Animal>().enabled = true;
            animals[i].GetComponent<Animal>().SetMovDuration(0);
            animals[i].AddComponent<Rigidbody2D>();
            animals[i].SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        foreach (GameObject animal in animals)
        {
            animal.SetActive(true);
        }
        player.SetActive(true);

        do
        {
            yield return new WaitForSeconds(.1f);
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a - .005f);
        } while (black.color.a > 0);

    }
}
