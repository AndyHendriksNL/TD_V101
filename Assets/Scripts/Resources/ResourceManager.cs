using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public ResourceTypeListSO resourceTypeList; // Referentie naar de ResourceTypeListSO

    private Dictionary<ResourceTypeSO, int> resourceAmounts = new Dictionary<ResourceTypeSO, int>();
    private int totalVillagers;
    private int assignedVillagers;
    private int freeVillagers;
    private int totalGoldGeneration;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {        
        InitializeResources();
    }

    private void InitializeResources()
    {
        if (ResourceUI.instance == null)
        {
            Debug.LogError("UIManager instance is not assigned!");
            return;
        }

        if (resourceTypeList == null)
        {
            Debug.LogError("ResourceTypeListSO is not assigned in ResourceManager!");
            return;
        }

        if (resourceTypeList.list == null || resourceTypeList.list.Count == 0)
        {
            Debug.LogError("ResourceTypeListSO is empty!");
            return;
        }

        foreach (ResourceTypeSO resourceType in resourceTypeList.list)
        {
            if (!resourceAmounts.ContainsKey(resourceType))
            {
                resourceAmounts[resourceType] = 0; // Start elke resource met 0
            }
        }

        // Initializeer resources en villagers vanuit de dictionary
        UpdateCachedValuesFromBuildings();

        StartCoroutine(GenerateGold());
    }

    private void UpdateCachedValuesFromBuildings()
    {
        totalVillagers = 0;
        totalGoldGeneration = 0;
        assignedVillagers = 0;

        // Haal een lijst op van alle geplaatste gebouwen uit de GameManager
        Dictionary<PlacedObjectTypeSO, int> placedBuildings = GameManager.instance.GetPlacedBuildings();

        foreach (var buildingEntry in placedBuildings)
        {
            PlacedObjectTypeSO buildingData = buildingEntry.Key;
            int buildingCount = buildingEntry.Value;

            totalVillagers += buildingData.villagerIncrease * buildingCount;  // Voeg het aantal villagers van de huizen toe
            totalGoldGeneration += buildingData.goldIncrease * buildingCount; // Voeg de goudgeneratie van gebouwen toe
            assignedVillagers += buildingData.villagerUse * buildingCount; // Bereken het totaal aantal villagers die ander werk hebben
        }

        freeVillagers = totalVillagers - assignedVillagers;
        ResourceTypeSO villagerResource = resourceTypeList.list.Find(r => r.type == ResourceType.Villagers);
        ResourceUI.instance.UpdateResourceUI(villagerResource, freeVillagers); // Update de UI

        totalGoldGeneration = freeVillagers;

        //Debug.Log("Total assignedVillagers calculated: " + assignedVillagers);
        //Debug.Log("Total Villagers calculated: " + totalVillagers);
        //Debug.Log("Total Gold/tick calculated: " + totalGoldGeneration);
    }

    // add resource to the total and update UI
    public void AddResource(ResourceTypeSO resourceType, int amount)
    {
        //Debug.Log("Add: " + amount + " of "+ resourceType+ ". Total is now:" + resourceAmounts[resourceType]);

        if (resourceAmounts.ContainsKey(resourceType))
        {
            resourceAmounts[resourceType] += amount;
            ResourceUI.instance.UpdateResourceUI(resourceType, resourceAmounts[resourceType]); // Update de UI
        }
    }

    public int GetResourceAmount(ResourceTypeSO resourceType)
    {
        return resourceAmounts.ContainsKey(resourceType) ? resourceAmounts[resourceType] : 0;
    }

    /*public void AssignVillager()
    {
        if (totalVillagers - assignedVillagers > 0)
        {
            assignedVillagers++;
        }
    }

    public void RemoveVillager()
    {
        if (assignedVillagers > 0)
        {
            assignedVillagers--;
        }
    }*/

    private IEnumerator GenerateGold()
    {
        while (true) // Dit blijft oneindig doorgaan
        {
            yield return new WaitForSeconds(1f);

            ResourceTypeSO goldType = resourceTypeList.list.Find(r => r.type == ResourceType.Gold);
            if (goldType != null)
            {
                AddResource(goldType, totalGoldGeneration); // Voeg de hoeveelheid totalGoldGeneration toe aan de huidige goudvoorraad
                //Debug.Log($"Generated {totalGoldGeneration} gold. New total is: {GetResourceAmount(goldType)}.");
            }

            yield return null; // Wacht tot de volgende frame
        }
    }

    // Aanroep vanuit GameManager of BuildingPlacementValidator wanneer een gebouw wordt geplaatst
    public void OnBuildingPlaced()
    {
        UpdateCachedValuesFromBuildings(); // Herbereken totalen wanneer een gebouw wordt toegevoegd
    }
}
