using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ResourceTypeSO goldResource; // Verwijzing naar het ResourceTypeSO voor goud
    public ResourceTypeSO villagersResource; // Verwijzing naar het ResourceTypeSO voor villagers
    public PlacedObjectTypeSO goldMineSO;
    public PlacedObjectTypeSO MainBuildingSO;

    private Dictionary<PlacedObjectTypeSO, int> placedBuildings = new Dictionary<PlacedObjectTypeSO, int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Blijft tussen verschillende sc�nes bestaan
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    public void Victory()
    {
        Debug.Log("Victory!");
    }

    public void AddBuilding(PlacedObjectTypeSO buildingData)
    {
        //Debug.Log("Add to placedBuildings: " + buildingData.name);

        if (!placedBuildings.ContainsKey(buildingData))
        {
            placedBuildings[buildingData] = 0; // Initialiseer met 0
        }

        placedBuildings[buildingData] += 1; // Verhoog het aantal gebouwen

        ResourceManager.instance.OnBuildingPlaced();

    }

    public Dictionary<PlacedObjectTypeSO, int> GetPlacedBuildings()
    {
        return placedBuildings;
    }
}
