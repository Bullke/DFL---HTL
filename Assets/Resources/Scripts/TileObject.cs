using UnityEngine;
using System.Collections;
using HWTools.Grid;

/*
 * Tile Object
 * This is an abstract class that represents an HTL Tile object, which can be placed on a Tile.
 * This class will be used as a parent class to many different HTL game objects.
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public abstract class TileObject : MonoBehaviour
{
	// Variables to hold the game manager and current Tile
    protected GridTransform gridTrans;
    protected Grid2DCollection gridCollect;
	protected Tile currentTile = null;

	/*
	 * Used for initialization
	 * Sets the game manager variable to the existing game manager
	 * Calls SnapToTileGrid to set the current Tile to an existing Tile in game space
	 * If null do nothing
	 * If the current Tile is occupied destroy this object
	 * Otherwise connect itself to the set current Tile
	 */
	protected void Start ()
	{
        //establish link to the Grid, and snap to it
        //.manager = GameObject.Find("Manager").GetComponent<GameManager>();

        gridTrans = this.GetComponent<GridTransform>();
        if (gridTrans.parent == null)
        {
            gridTrans.findGrid();
        }
        gridCollect = gridTrans.parent.gameObject.GetComponent<Grid2DCollection>();
        SnapToTileGrid();
	}

	/*
	 * Update is called once per frame
	 * Right meow it does this to support mouse dragging and dropping
	 */
	//MIGHT BE CHANGED TO UPDATE ONLY ONCE IN THE FUTURE
	protected void Update ()
	{

        //Debug.LogWarning("Grid2D: " + gridTrans.parent);
        if (currentTile != null)
		{
			currentTile.occupied = false;
			currentTile = null;
		}
		SnapToTileGrid();
        //Redundant due to SnapToTileGrid() changes
        /*
		if (currentTile != null)
		{
			currentTile.occupied = true;
			currentTile.setOccupant(this);
		}
        */
	}

	//Snaps the TileObject to the Grid
	//Sets the tile's Occupation State to Occupied
	/*
	 * Used to snap the Tile object to a Tile
	 * Sets the targetCoord 2D vector to the world position (in-game) coordinates
	 * Returns a boolean of if the current Tile has been succesfully set to an existing Tile
	 * If the target coordinates are outside of the grid 2D array, return false
	 * Otherwise fetch the Tile from the grid array with the given coordinates and set current Tile
	 * equal to it
	 * If the current Tile is null or occupied, return false
	 * Otherwise set this object position to the current Tile position(snap to it) and return true
	 */
	protected bool SnapToTileGrid()
	{
        var tileAtPos = gridCollect[gridTrans.GridPosition];
        if (tileAtPos != null)
        {
            currentTile = tileAtPos.GetComponent<Tile>();
        }
        if (tileAtPos == null || currentTile.occupied)
		{
			Debug.Log("Unable to occupy tile");
			return false;
		}
        //this.transform.position = currentTile.transform.position;
        currentTile.occupied = true;
        currentTile.setOccupant(this);
		return true;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos">A location in Grid2D space</param>
    /// <returns></returns>
    protected bool SnapToTileGridAtGridPos(Vector2 gridPos)
    {
        var tileAtPos = gridCollect[gridPos];
        if (tileAtPos != null)
        {
            currentTile = tileAtPos.GetComponent<Tile>();
        }
        if (tileAtPos == null || currentTile.occupied)
        {
            Debug.Log("Unable to occupy tile");
            return false;
        }
        //this.transform.position = currentTile.transform.position;
        currentTile.occupied = true;
        currentTile.setOccupant(this);
        return true;
    }

	/*
	 * @Inherits parent class method
	 * Detaches the Tile object from the Tile 
	 */
	protected virtual void OnDestroy()
	{
		if (currentTile != null)
		{
			Debug.Log("beep");
			currentTile.occupied = false;
		}
	}

    /// <summary>
    /// When Disabled, unoccupy current tile
    /// </summary>
    protected virtual void OnDisable()
    {
        if (currentTile != null)
        {
            currentTile.occupied = false;
        }
    }
}
