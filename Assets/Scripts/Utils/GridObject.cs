using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridObject
{
    public class GridObject
    {
        private GridXZ<GridObject> grid; // Reference to the grid that this object belongs to
        private int x;                   // X coordinate of the grid cell
        private int z;                   // Z coordinate of the grid cell
        private PlacedObject placedObject; // Reference to the placed object in this grid cell
        private GameObject placedGameObject; // Nieuwe variabele voor het GameObject

        // Constructor to initialize GridObject with the grid reference and its position
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        // Override ToString for easy debugging and visualization of the grid position
        public override string ToString()
        {
            return x + ", " + z + "\n" + (placedObject != null ? placedObject.ToString() : "No Object");
        }

        // Notify the grid that this grid object's state has changed
        public void TriggerGridObjectChanged()
        {
            grid.TriggerGridObjectChanged(x, z);
        }

        // Set the placed object and notify the grid of the change
        public void SetPlacedObject(PlacedObject placedObject, GameObject gameObject)
        {
            this.placedObject = placedObject;
            this.placedGameObject = gameObject;
            grid.TriggerGridObjectChanged(x, z); // Notify change
        }

        // Clear the placed object and notify the grid of the change
        public void ClearPlacedObject()
        {
            placedObject = null;
            TriggerGridObjectChanged(); // Notify change
        }

        public GameObject GetPlacedGameObject() // Nieuwe functie
        {
            return placedGameObject; // Retourneer het GameObject
        }

        // Get the currently placed object
        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        // Check if a new object can be placed in this grid cell
        public bool CanBuild()
        {
            return placedObject == null; // Can build if there's no object already placed
        }
    }
}
