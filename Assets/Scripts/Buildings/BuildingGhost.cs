using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private void Awake()
    {
        //meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Start()
    {
        RefreshVisual();

        BuildManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = BuildManager.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, BuildManager.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);

        UpdateGhostMaterial();
    }

    private void UpdateGhostMaterial()
    {
        if (meshRenderer != null)
        {
            // Controleer of je op deze locatie kunt bouwen en pas het juiste materiaal toe
            Material ghostMaterial = BuildManager.Instance.CanBuildHere() ? BuildManager.Instance.M_CanPlace : BuildManager.Instance.M_CantPlace;
            SetMaterial(ghostMaterial);
        }
    }
    public void SetMaterial(Material material)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
        }
        else
        {
            Debug.LogError("MeshRenderer is null! Make sure to call this after visual is instantiated.");
        }
    }

    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = BuildManager.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);            
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;

            // Zet het ghost object in de juiste layer
            SetLayerRecursive(visual.gameObject, 11);

            // Update de meshRenderer referentie met de nieuwe visual
            meshRenderer = visual.GetComponentInChildren<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.LogError("No MeshRenderer found on the visual prefab!");
            }
            else
            {
                // Roep direct SetMaterial aan met het juiste materiaal gebaseerd op de bouwstatus
                Material ghostMaterial = BuildManager.Instance.CanBuildHere() ? BuildManager.Instance.M_CanPlace : BuildManager.Instance.M_CantPlace;
                //Debug.Log("Can build? " + BuildManager.Instance.CanBuildHere());
                //Debug.Log(ghostMaterial);
                SetMaterial(ghostMaterial);
            }
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}

