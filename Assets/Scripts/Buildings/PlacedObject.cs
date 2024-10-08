﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        
        // instantiate the object
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        // get component
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        
        // fill in the data
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;

        placedObject.Setup();

        return placedObject;
    }

    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    protected virtual void Setup() {
        //Debug.Log("PlacedObject.Setup() " + transform);
    }

    public virtual void GridSetupDone() {
        //Debug.Log("PlacedObject.GridSetupDone() " + transform);
    }

    protected virtual void TriggerGridObjectChanged() {
        foreach (Vector2Int gridPosition in GetGridPositionList()) {
            BuildManager.Instance.GetGridObject(gridPosition).TriggerGridObjectChanged();
        }
    }

    public Vector2Int GetGridPosition() {
        return origin;
    }

    public List<Vector2Int> GetGridPositionList() {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public virtual void DestroySelf() {
        Destroy(gameObject);
    }

    public override string ToString() {
        return placedObjectTypeSO.nameString;
    }

    public SaveObject GetSaveObject() {
        return new SaveObject {
            placedObjectTypeSOName = placedObjectTypeSO.name,
            origin = origin,
            dir = dir,
            //floorPlacedObjectSave = (this is FloorPlacedObject) ? ((FloorPlacedObject)this).Save() : "",
        };
    }

    internal PlacedObjectTypeSO GetPlacedObjectType()
    {
        return placedObjectTypeSO;
    }

    [System.Serializable]
    public class SaveObject {

        public string placedObjectTypeSOName;
        public Vector2Int origin;
        public PlacedObjectTypeSO.Dir dir;
        public string floorPlacedObjectSave;

    }

}
