using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingTypeSelectUI : MonoBehaviour
{
    [SerializeField] private List<PlacedObjectTypeSO> PlacedObjectTypeList;

    //private Dictionary<PlacedObjectTypeSO, Transform> btnTransformDictionary;

    private Transform btnTransform;
    private Transform btnCancel;

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        //Debug.Log("UI ontvangt select change");
        ShowHideCancelButton();
    }

    private void Awake()
    {
        // haal button UI template op
        Transform btnTemplate = transform.Find("btnTemplate");

        // hide template
        btnTemplate.gameObject.SetActive(false);

        // create dictionary
        //btnTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>();

        int index = 0;

        foreach (PlacedObjectTypeSO buildingType in PlacedObjectTypeList)
        {
            // clone template
            btnTransform = Instantiate(btnTemplate, transform);
            btnTransform.gameObject.SetActive(true);

            // positioning
            float offsetAmount = +80f;
            btnTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetAmount * index, 0);

            // set button text
            btnTransform.Find("text").GetComponent<TextMeshProUGUI>().SetText(buildingType.name);

            // set button
            btnTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                //Debug.Log("Click on button detected:" + buildingType);

                BuildManager.Instance.SetSelectedPlacedObject(buildingType);
            });

            //btnTransformDictionary[buildingType] = btnTransform;

            index++;
        }

        // clone template for cancel button
        btnCancel = Instantiate(btnTemplate, transform);
        btnCancel.gameObject.SetActive(false);

        // positioning
        float offsetAmountCancel = +80f;
        btnCancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetAmountCancel * index, 0);

        // set button text
        btnCancel.Find("text").GetComponent<TextMeshProUGUI>().SetText("Cancel");
        btnCancel.Find("text").GetComponent<TextMeshProUGUI>().color = Color.red; 

        // set button
        btnCancel.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Click on button detected: cancel");

            BuildManager.Instance.DeselectObjectType();
        });
    }
    private void Start()
    {
        BuildManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }
    private void ShowHideCancelButton()
    {
        PlacedObjectTypeSO placedObjectTypeSO = BuildManager.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO == null)
        {
            // hide button
            btnCancel.gameObject.SetActive(false);
        }
        else
        {
            // show button
            btnCancel.gameObject.SetActive(true);
        }
    }

}
