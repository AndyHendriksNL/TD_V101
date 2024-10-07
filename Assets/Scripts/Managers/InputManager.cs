using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject selectedObject;
    public LayerMask BuildingsLayer;  // Voeg een LayerMask toe voor selecteerbare objecten

    void Update()
    {
        // Linkermuisknop om een object te selecteren
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
            Debug.Log("mouse position: " + mouseWorldPosition);

            // Schiet een ray vanaf de muispositie in de wereld
            Ray ray = new Ray(mouseWorldPosition, Vector3.down); // Ray richting beneden of een andere richting

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, BuildingsLayer))  // Gebruik LayerMask voor selecteerbare objecten
            {
                Debug.Log("Hit gameobject: " + hit.collider.gameObject);
                if (hit.collider.gameObject.CompareTag("hasPopup"))
                {
                    SelectObject(hit.collider.gameObject);
                }
            }
        }

        // Rechtermuisknop of Escape om te deselecteren
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectObject();
        }
    }

    private void SelectObject(GameObject obj)
    {
        if (obj == selectedObject) return;

        // Deselecteer het huidige geselecteerde object, indien aanwezig
        if (selectedObject != null)
        {
            DeselectObject();
        }

        // Voeg outline-component toe of activeer deze indien al aanwezig
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            obj.AddComponent<Outline>();
        }
        else
        {
            outline.enabled = true;
        }

        selectedObject = obj;
    }

    private void DeselectObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }
}
