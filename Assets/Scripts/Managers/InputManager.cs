using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public GameObject selectedObject;
    public LayerMask BuildingsLayer;  // Voeg een LayerMask toe voor selecteerbare objecten
    public GameObject detailsPanel; // The UI panel to show object details
    public TextMeshProUGUI nameText; // UI text to display object name

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        detailsPanel.SetActive(false); // Hide panel initially
    }

    void Update()
    {
        // Linkermuisknop om een object te selecteren
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            SelectObject();
        }

        // Rechtermuisknop of Escape om te deselecteren
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            DeselectObject();
        }
    }

    void SelectObject()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, BuildingsLayer))
        {
            //Debug.Log("Hit " + hit);
            if (hit.collider.gameObject != selectedObject)
            {
                //Debug.Log("selectedObject " + selectedObject);
                DeselectObject(); // Deselect the previous object

                if (hit.collider.gameObject.transform.parent != null &&
                    hit.collider.gameObject.transform.parent.parent != null)
                {
                    // onderdeel > visual > prefab
                    selectedObject = hit.collider.gameObject.transform.parent.parent.gameObject;
                }
                else
                {
                    selectedObject = hit.collider.gameObject; // Fallback in case there isn't a grandparent
                }


                var outline = selectedObject.GetComponent<Outline>() ?? selectedObject.AddComponent<Outline>();
                outline.enabled = true;

                ShowDetails(selectedObject); // Show object details in UI
            }
        }
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
            detailsPanel.SetActive(false); // Hide UI panel
        }
    }

    void ShowDetails(GameObject obj)
    {
        if (obj != null)
        {
            PlacedObject placedObject = obj.GetComponent<PlacedObject>();
            if(placedObject != null)
            {
                PlacedObjectTypeSO placedObjectType = placedObject.GetPlacedObjectType();
                if(placedObjectType != null)
                {
                    // Update UI panel with the building's information
                    nameText.text = placedObjectType.nameString;

                    detailsPanel.SetActive(true); // Show the details UI panel
                }
            }
            else
            {
                Debug.LogError($"Kon extra gegevens van aangeklikt object niet ophalen: {obj}");
            }
                

            
        }
    }
}
