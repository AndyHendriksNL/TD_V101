using System;
using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private PlacedObjectTypeSO mainBuildingSO;
    [SerializeField] private PlacedObjectTypeSO goldMineSO;
    [SerializeField] public Material M_CanPlace;
    [SerializeField] public Material M_CantPlace;

    private GridXZ<GridObject> grid;
    private BuildingGhost buildingGhost;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    private ResourceTypeSO goldType;


    private bool isDemolishActive;

    private void Awake()
    {
        Instance = this;

        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> gameObject, int x, int z) => new GridObject(gameObject, x, z));
    }

    private void Start()
    {
        PlaceDefaultBuildings();
        goldType = ResourceManager.instance.resourceTypeList.list.Find(r => r.type == ResourceType.Gold);
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        // place grid object in each grid cell
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        // display coordinates in grid
        public override string ToString()
        {
            return x + ", " + z + "\n" + placedObject;
        }

        public void TriggerGridObjectChanged()
        {
            grid.TriggerGridObjectChanged(x, z);
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;

            // let the grid know the object has changed
            grid.TriggerGridObjectChanged(x, z);
        }
        public void ClearPlacedObject()
        {
            placedObject = null;
            // let the grid know the object has changed
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            // if transform is null (spot is empty), we can build.
            // not null we cant build
            return placedObject == null;
        }
    }

    private void Update()
    {
        HandleNormalObjectPlacement();
        HandleDirRotation();
        HandleDemolish();
        CanBuildHere();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Cancel Build");
            DeselectObjectType();
        }
    }

    // Test of je op de huidige muis positie kan bouwen
    public bool CanBuildHere()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        GridObject gridObject = grid.GetGridObject(x, z);

        // Controleer of de positie geldig is en er geen object is geplaatst
        if (gridObject != null && gridObject.CanBuild())
        {
            return true;
        }
        return false;
    }


    private void PlaceDefaultBuildings()
    {
        // Posities voor de standaard gebouwen
            Vector2Int mainBuildingPosition = new Vector2Int(2, 2); // Pas deze posities aan naar waar je de gebouwen wilt plaatsen
            Vector2Int goldMinePosition = new Vector2Int(8, 8);     // Pas deze posities aan naar waar je de gebouwen wilt plaatsen

            // Plaats MainBuilding
            if (TryPlaceObject(mainBuildingPosition, mainBuildingSO, PlacedObjectTypeSO.Dir.Down))
            {
                //Debug.Log("MainBuilding geplaatst op " + mainBuildingPosition);
            }
            else
            {
                Debug.LogError("Kan MainBuilding niet plaatsen!");
            }

            // Plaats GoldMine
            if (TryPlaceObject(goldMinePosition, goldMineSO, PlacedObjectTypeSO.Dir.Down))
            {
                //Debug.Log("GoldMine geplaatst op " + goldMinePosition);
            }
            else
            {
                Debug.LogError("Kan GoldMine niet plaatsen!");
            }   
    }

    private void HandleNormalObjectPlacement()
    {
        if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null && !UtilsClass.IsPointerOverUI())
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            if(CheckEnoughGold(placedObjectTypeSO.buildCost))
            {
                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                if (TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject))
                {
                    // Object placed
                }
                else
                {
                    // Error!
                    UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                }
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Not enough gold!", mousePosition);
            }            
        }
    }

    private bool CheckEnoughGold(int buildCost)
    {
        int currentGold = ResourceManager.instance.GetResourceAmount(goldType);

        if (currentGold > buildCost)
        {
            return true;
        }
        else { return false; }
    }

    private void HandleDirRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }
    }

    private void HandleDemolish()
    {
        if (isDemolishActive && Input.GetMouseButtonDown(0) && !UtilsClass.IsPointerOverUI())
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null)
            {
                // Demolish
                placedObject.DestroySelf();

                // get the objects other grid positions
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                // remove from list of placed objects
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }
    public void DeselectObjectType()
    {
        placedObjectTypeSO = null;
        isDemolishActive = false;
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TryPlaceObject(int x, int y, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir)
    {
        return TryPlaceObject(new Vector2Int(x, y), placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir)
    {
        return TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject)
    {
        // Test Can Build
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            //bool isValidPosition = grid.IsValidGridPositionWithPadding(gridPosition);
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition)
            {
                // Not valid
                canBuild = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        Debug.Log("Canbuild:" + canBuild);

        if (canBuild)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            placedObject.GridSetupDone();

            // let GameManger know a building was added
            GameManager.instance.AddBuilding(placedObjectTypeSO);

            // pay for building
            if(placedObjectTypeSO.buildCost > 0)
            {
                ResourceManager.instance.AddResource(goldType, -placedObjectTypeSO.buildCost);
            }            

            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);

            return true;
        }
        else
        {
            // Cannot build here
            placedObject = null;
            return false;
        }
    }

    /* --------------------------------------- */

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector2Int gridPosition)
    {
        return grid.GetGridObject(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector3 worldPosition)
    {
        return grid.GetGridObject(worldPosition);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return grid.IsValidGridPosition(gridPosition);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    public void SetSelectedPlacedObject(PlacedObjectTypeSO placedObjectTypeSO)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        isDemolishActive = false;
        RefreshSelectedObjectType();
    }

    public void SetDemolishActive()
    {
        placedObjectTypeSO = null;
        isDemolishActive = true;
        RefreshSelectedObjectType();
    }

    public bool IsDemolishActive()
    {
        return isDemolishActive;
    }
}
