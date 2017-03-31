/*
 *  Art Prop
 *  Extension of TileObject class. Has the following properties:
 *      -May Occupy multiple tiles, preventing Obstacle or Wizard placement.
 *      -Does not otherwise affect Gameplay.
 *  
 *  @author Steven Roberts
 *  @author Nenad Bulicic
 */


using UnityEngine;
using System.Collections;
using HWTools.Grid;


public class ArtProp : TileObject
{
    // Local Tile-Grid space that represents which tiles the ArtProp affects.
	public bool[,] propGridArea;
	public short propGridY = 1,
        propGridX = 1;

    // Holds the current position of propGridArea[0,0].
	private Vector2 zeroZeroPosition;

    //Holds a Helper that would otherwise be instantiated constantly with no need to return
    private Vector2 positioningHelper;

    private bool _attachedToGrid;

    /// <summary>
    /// Use this for initialization.
    /// Does not call base.Start() because this object instantiates based on multiple tiles.
    /// </summary>
    new protected void Start ()
	{
        _attachedToGrid = false;

        //create propGridArea
        propGridArea = new bool[propGridX, propGridY];

        gridTrans = this.GetComponent<GridTransform>();
        if (gridTrans.parent == null)
        {
            gridTrans.findGrid();
        }
        //Debug.LogWarning("GridTransform: " + gridTrans);
        //Debug.LogWarning("Grid2D: " + gridTrans.parent);
        //Debug.LogWarning("Grid2DCollection: " + gridTrans.parent.GetComponent<Grid2DCollection>());
        gridCollect = gridTrans.parent.gameObject.GetComponent<Grid2DCollection>();


        // Populate the propGridArea.
        populatePropGrid();

        // Calculate the position of propGridArea(0,0).
        zeroZeroPosition = calcZeroZeroPosition();

        //attach to all tiles specified by populated Grid Area.
        attachToGrid();
	}

    /// <summary>
	/// Update is called once per frame
    /// </summary>
	new protected void Update ()
	{
        // Detach all currently attached tiles.
        detachFromGrid();
        
        // Calculate position again in anticipation of the artprop being moved.
        zeroZeroPosition = calcZeroZeroPosition();

        // Re-attach all currently attached tiles.
        attachToGrid();
    }


    /// <summary>
    /// Populates the propGridArea
    /// To be overriden by Child Classes
    /// Base version assumes entire grid is True
    /// </summary>
    protected virtual void populatePropGrid()
    {
        for (int row = 0; row < propGridY; row++)
        {
            for (int column = 0; column < propGridX; column++)
            {
                propGridArea[column, row] = true;
            }
        }
    }

    /// <summary>
    /// Returns position of propGridArea[0,0] in Grid Space.
    /// Assumes that manager exists.
    /// </summary>
    private Vector2 calcZeroZeroPosition()
    {
        /* OLD
        // Calculate Vector from center of Grid (center of sprite) to propGridArea[0,0] in local grid space.
        Vector2 centerToZeroZero = new Vector2(propGridX - 1, propGridY -1);
        centerToZeroZero = centerToZeroZero / -2;

        // Add World space of center of grid to Translation vector.
        return this.transform.position + (manager.getTileHeightVector() * centerToZeroZero.y) + (manager.getTileWidthVector() * centerToZeroZero.x);
        */
        // Center of prop Grid - (X/2, Y/2) to get to lowerleft corner, then + .5,.5 to get to the center of the zerozerotile in the prop grid
        // Returns the tile in Grid2D space that maps to 0,0 in the prop grid.
        return gridTrans.GridPosition + new Vector3(-(propGridX/2),-(propGridY/2), 0f);
        
    }


    /// <summary>
    /// Sets value at desired location in the prop grid to occupy or not occupy. Returns false if given bad values.
    /// </summary>
    /// <param name="propX">X coordinate of a tile in prop grid</param>
    /// <param name="propY">Y coordinate of a tile in prop grid</param>
    /// <param name="occupancyValue">True If Target tile can occupy tiles, else false</param>
    /// <returns></returns>
    public bool setPropGridValue (int propX, int propY, bool occupancyValue)
    {
        // Array out of bounds check.
        if (propX < 0 || propY < 0 || propX >= propGridX || propY >= propGridY)
        {
            Debug.LogWarning("propGrid values cannot be set: Out of Bounds");
            return false;
        }
        //Set occupancy value in prop grid
        propGridArea[propX, propY] = occupancyValue;
        return true;

        // New occupancy value will take effect on next attachment.
    }

    /// <summary>
    /// Detaches the Art Prop from the Tile Grid. Does nothing if Prop not attached to grid.
    /// </summary>
    public void detachFromGrid()
    {
        //Checks for attachment to grid. Does not detach if not necessary
        if (_attachedToGrid)
        {
            // Check all tiles in propGrid
            for (int row = 0; row < propGridY; row++)
            {
                for (int column = 0; column < propGridX; column++)
                {
                    // Check if area could be attached
                    if (propGridArea[column, row] == true)
                    {
                        // Check if tile exists
                        positioningHelper = new Vector2(column, row);
                        var tileAtGridPos = gridCollect[zeroZeroPosition + positioningHelper];
                        if (tileAtGridPos != null)
                        {
                            // If it does exist and is not null, detach
                            
                            currentTile = tileAtGridPos.GetComponent<Tile>();
                            if (currentTile != null)
                            {
                                currentTile.occupied = false;
                                currentTile = null;
                            }
                        }                        
                    }
                }
            }
            // once detach complete, mark as detached.
            _attachedToGrid = false;
        }
    }

    /// <summary>
    /// Attaches the Art Prop to the Tile Grid. Returns false if attachment fails for any reason.
    /// </summary>
    /// <returns>True if attach successful. False if not.</returns>
    public bool attachToGrid()
    {
        // if not attached to grid
        if (_attachedToGrid)
        {
            Debug.Log("Attach attempted while already Attached to Grid");
            return false;
        }

        // Check if the artProp is able to occupy this space without conflicting with other TileObjects
        if (currentSpaceIsOccupiable())
        {
            // Check all tiles in propGrid
            for (int row = 0; row < propGridY; row++)
            {
                for (int column = 0; column < propGridX; column++)
                {
                    // Check if area could be attached
                    if (propGridArea[column, row] == true)
                    {
                        // Check if tile exists
                        positioningHelper = new Vector2(column, row);
                        var tileAtGridPos = gridCollect[zeroZeroPosition + positioningHelper];
                        if (tileAtGridPos != null)
                        {
                            // If it does exist and is not null, detach
                            currentTile = tileAtGridPos.GetComponent<Tile>();
                            if (currentTile != null)
                            {
                                currentTile.occupied = true;
                                currentTile.setOccupant(this);
                            }
                        }
                    }
                }
            }
            // Success! Mark that grid attachment successful
            _attachedToGrid = true;
            return true;
        }
        // ELSE current space not occupiable
        else {
            Debug.Log("Current Space not Occupiable");
            return false;
        }
    }

    /// <summary>
    /// Checks to see if all spaces that would be affected by the art prop's placement are occupiable.
    /// Does not check for null spaces, as art props can hang off the edge of the tile grid.
    /// </summary>
    private bool currentSpaceIsOccupiable()
    {
        for (int row = 0; row < propGridY; row++)
        {
            for (int column = 0; column < propGridX; column++)
            {
                // Check if tile exists
                positioningHelper = new Vector2(column, row);
                var tileAtGridPos = gridCollect[zeroZeroPosition + positioningHelper];
                if (tileAtGridPos != null)
                {
                    // If it does exist and is not null, detach
                    currentTile = tileAtGridPos.GetComponent<Tile>();

                    // If that tile exists and is occupied, current space is not occupiable, and therefore the artprop cannot occupy its current location.
                    if (currentTile != null && currentTile.occupied)
                    {
                        return false;
                    }                    
                }                
            }
        }
        // If reaches here, all tiles that can occupy are occupying.
        return true;
    }


    /// <summary>
    /// Called when object is destroyed. Overrides TileObject's destruction code.
    /// </summary>
    override protected void OnDestroy()
    {        
        detachFromGrid();
    }

}