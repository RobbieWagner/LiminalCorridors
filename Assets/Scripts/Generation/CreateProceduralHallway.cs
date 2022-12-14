using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateProceduralHallway : MonoBehaviour
{
    [SerializeField]
    private int[] initialTilesToCreate;
    [SerializeField]
    private GameObject[] hallwayTiles;
    public GameObject backwall;

    List<int> positiveZHallEndHallwayCompatibility;
    int positiveZHallEndHallway;
    int negativeZHallEndHallway;

    float positiveZHallEndPos;
    float negativeZHallEndPos;

    System.Random rnd;
    [SerializeField]
    private RND random;

    public int renderDistance;

    [SerializeField]
    private GameObject character;

    public List<int> idsOfSpawnedItems;

    public int tilesBeforeMonster = 200;
    public int tilesCreated = 0;

    public int currentBook;

    [TextArea]
    public string[] bookText;

    [SerializeField]
    private GameObject monsterGO;
    public bool spawningMonster;

    // Start is called before the first frame update
    void Start()
    {
        currentBook = 0;
        rnd = random.rnd;
        tilesCreated = 0;

        positiveZHallEndPos = initialTilesToCreate.Length * 10/2;
        positiveZHallEndHallwayCompatibility = hallwayTiles[0].GetComponent<CompatibleHallways>().compatibleHallways;
        CreateHallsGoingForward(initialTilesToCreate, 0, initialTilesToCreate.Length * 10 / 2);
    }

    void FixedUpdate()
    {
        //calls hallway generation functions if the player is reaching the edge of the already generated map
        if(character.transform.position.z > (positiveZHallEndPos - (renderDistance * 10)))
        {
            CreateHallsGoingBackwards(1, positiveZHallEndHallway, positiveZHallEndPos);
        }
        if(character.transform.position.z < (negativeZHallEndPos + (renderDistance * 10)))
        {
            CreateHallsGoingForward(1, negativeZHallEndHallway, negativeZHallEndPos);
        }

        if(tilesCreated >= tilesBeforeMonster && !monsterGO.activeSelf) monsterGO.SetActive(true);
    }

    public void IncrementBook()
    {
        if(currentBook != bookText.Length - 1) currentBook++;
    }

    public void CreateHallsGoingForward(int[] tilesToCreate, int lastHallwayUsed, float lastHallwayUsedPosition)
    {
        //places already generated hall tiles in the environment going in the negative z direction
        float hallPos;
        for(int i = 0; i < tilesToCreate.Length; i++)
        {
            hallPos = lastHallwayUsedPosition - (i * 10);
            Instantiate(hallwayTiles[tilesToCreate[i]], new Vector3(0, 0, hallPos), Quaternion.identity);

            lastHallwayUsed = tilesToCreate[i];
            if(i == tilesToCreate.Length - 1)
            {
                negativeZHallEndHallway = tilesToCreate[i];
                negativeZHallEndPos = hallPos;
            }
        }
    }

    public void CreateHallsGoingBackwards(int[] tilesToCreate, int lastHallwayUsed, float lastHallwayUsedPosition)
    {
        //places already generated hall tiles in the environment going in the positive z direction
        float hallPos;
        for(int i = 0; i < tilesToCreate.Length; i++)
        {
            hallPos = lastHallwayUsedPosition + (i * 10);
            Instantiate(hallwayTiles[tilesToCreate[i]], new Vector3(0, 0, hallPos), Quaternion.identity);

            lastHallwayUsed = tilesToCreate[i];
            if(i == tilesToCreate.Length - 1)
            {
                negativeZHallEndHallway = tilesToCreate[i];
                negativeZHallEndPos = hallPos;
            }
        }
    }

    public void CreateHallsGoingForward(int tilesToCreate, int lastHallwayUsed, float lastHallwayUsedPosition)
    {
        //generates hallway tiles and places them in the environment going in the negative z direction
        int hallway;
        float hallPos;

        for(int i = 1; i <= tilesToCreate; i++)
        {
            hallPos = lastHallwayUsedPosition - (i * 10); 
            bool keepPipesGoing = rnd.Next(4) > 0;
            if(keepPipesGoing) hallway = 0;
            else hallway = rnd.Next(0, hallwayTiles.Length);
            while(lastHallwayUsed != -1 && !hallwayTiles[hallway].GetComponent<CompatibleHallways>().compatibleHallways.Contains(lastHallwayUsed))
            {
                hallway = rnd.Next(0, hallwayTiles.Length);
            }

            Instantiate(hallwayTiles[hallway], new Vector3(0, 0, hallPos), Quaternion.identity);

            lastHallwayUsed = hallway;
            if(i == tilesToCreate)
            { 
                negativeZHallEndHallway = hallway;
                negativeZHallEndPos = hallPos;
            }
        }

        tilesCreated += tilesToCreate;
    }

    public void CreateHallsGoingBackwards(int tilesToCreate, int lastHallwayUsed, float lastHallwayUsedPosition)
    {
        //generates hallway tiles and places them in the environment going in the positive z direction
        int hallway;
        float hallPos;

        for(int i = 1; i <= tilesToCreate; i++)
        {
            hallPos = lastHallwayUsedPosition + (i * 10); 
            bool keepPipesGoing = rnd.Next(8) > 0;
            if(keepPipesGoing) hallway = 0;
            else if(rnd.Next(20) == 0)
            {
                hallway = rnd.Next(64, hallwayTiles.Length);
            }
                        
            else hallway = rnd.Next(0, 64);
            while(lastHallwayUsed != -1 && !hallwayTiles[lastHallwayUsed].GetComponent<CompatibleHallways>().compatibleHallways.Contains(hallway))
            {
                hallway = rnd.Next(0, 65);
            }

            Instantiate(hallwayTiles[hallway], new Vector3(0, 0, hallPos), Quaternion.identity);

            lastHallwayUsed = hallway;
            if(i == tilesToCreate)
            { 
                positiveZHallEndHallway = hallway;
                positiveZHallEndPos = hallPos;
            }
        }
        
        tilesCreated += tilesToCreate;
    }
}
