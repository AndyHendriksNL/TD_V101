using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{
    public static ResourceUI instance;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI villagersText;
    public List<PlacedObjectTypeSO> buildingDataList;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Zorg ervoor dat er maar ��n instantie is
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject); // Als je wilt dat het over verschillende sc�nes blijft
    }

    private void Start()
    {

    }

    private void Update()
    {
        //UpdateBUildButtons();
    }

    public void UpdateBUildButtons()
    {
        /* // Controleer hoeveel goud er momenteel is
         ResourceTypeSO goldType = ResourceManager.instance.resourceTypeList.list.Find(r => r.type == ResourceType.Gold);
         int currentGold = ResourceManager.instance.GetResourceAmount(goldType);

         foreach (BuildingDataSO buildingData in buildingDataList)
         {
             if(buildingData.buildButton != null)
             {
                 Button button = buildingData.buildButton;

                 // Controleer of er genoeg goud is voor dit specifieke gebouw
                 bool canAfford = currentGold >= buildingData.goldCost;

                 Debug.Log("Button: " + button + ", kan gebouwd worden " + canAfford + ". Kost " + buildingData.goldCost);

                 // Zet de knop interactief als er genoeg goud is
                 button.interactable = canAfford;
             }
         }*/
    }

    public void UpdateResourceUI(ResourceTypeSO resourceType, int amount)
    {
        //Debug.Log($"Updating UI for {resourceType.type}: {amount}");

        if (resourceType.type == ResourceType.Gold)
        {
            goldText.text = amount.ToString();
            //Debug.Log($"Gold Updated: {goldText.text}");
        }
        else if (resourceType.type == ResourceType.Villagers)
        {
            villagersText.text = amount.ToString();
            //Debug.Log($"Villagers Updated: {villagersText.text}");
        }
        else
        {
            //Debug.LogWarning($"Resource type not recognized: {resourceType.type}");
        }
    }
}
