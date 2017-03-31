using UnityEngine;
using System.Collections;
using HWTools.Grid;
using HWTools.GameStats;

/*
 *  This Partial Class contains all functions involved in selecting the next direction for a Squibble to travel.
 *  All are private functions, accessed by calling Squibble.GetNextTarget.
 */
public partial class Squibble
{
    /// <summary>
    /// Entry Function for Squibble Tile Selection Process. Called by Squibble.Move()
    /// Returns the next movement target Tile.
    /// </summary>
    /// <returns>next movement target tile</returns>
    private Tile GetNextTarget()
    {
        //Debug.Log("Aquiring new Target");
        // Determine possible paths.
        populateTileList();

        // If tile wants squibbles to check forward first, do special variation of checks
        if (priorityLocation >= 0 && target.checkForwardFirst)
        {
            // If path in forward direction > 0, check for path validity there.
            currentPriority = tilePriorityList[priorityLocation];
            if (currentPriority >= 0)
            {
                return checkForValidPath();
            }

            // Else, remove the priority from the priority list
            tilePriorityList[priorityLocation] = short.MinValue;
        }

        // Else (or if forward direction is invalid), proceed to Recursive portion.
        return GetNextTargetRecursive();
    }

    /*
	 * Recursive Portion. Called when next highest priority needs to be determined before validity is checked.
	 */
    private Tile GetNextTargetRecursive()
    {
        // Sets PriorityLocation and CurrentPriority to the highest Priority direction for from the current tile.
        getHighestPriority();
        return checkForValidPath();
    }


    /*
	 * Checks the validity of the path at the current Priority Location.
	 * Assumes that PriorityLocation is already set to the direction of the currently desired priority.
	 * Assumes that CurrentPriority is already set to the Priority value in PriorityLocations's direction.
     * ANIMATOR CHANGES MAY AFFECT THIS FUNCTION
	 */
    private Tile checkForValidPath()
    {
        // Get the tile at the current priority location
        priorityTile = getPriorityTile(priorityLocation);

        // If the Priority Value is < 0, no paths are available. Restart the pathfinding function on next update.
        if (currentPriority < 0)
        {
            return target;
        }

        // Verifies that the tile to check is a valid tile for pathing.
        if (priorityTile != null && priorityTile.isAPath)
        {
            // Checks for the prescence of an Obstacle on the Tile.
            // If unoccupied, path is valid. Proceed.
            if (priorityTile.occupied)
            {
                // If occupying object is not an obstacle, path is valid. Proceed.
                #region obstacle detection
                Debug.Log("priorityTile is Occupied");

                if (priorityTile.getOccupant().tag == "Obstacle")
                {
                    // LAG SOURCE HERE? What do? (Confirmed, slow on Steven's terrible computer).
                    Obstacle theObstacle = priorityTile.getOccupant().GetComponent<Obstacle>();

                    //Test/Debug function to determine Obstacle accessed correctly.
                    theObstacle.beep();

                    //If the object is "passable", path is valid. Proceed.
                    if (!theObstacle.passable)
                    {
                        if (!theObstacle.permanent)
                        {
                            // WAIT FOR OBSTACLE TO BECOME PASSABLE: nonpermanent obstacles will eventually become passable.
                            // TODO: Change from Busy-wait to more reasonable waiting function
                            return target;
                        }

                        //If obstacle is permanenent and impassable, path is invalid. Get a new path.
                        tilePriorityList[priorityLocation] = short.MinValue;
                        return GetNextTargetRecursive();
                    }
                }
                #endregion
            }
        }

        // IF current Tile is null or not a path
        else
        {
            //Debug.Log("PriorityTile is not a path");
            // Remove the priority from the priority list, and call recursion again.
            tilePriorityList[priorityLocation] = short.MinValue;
            return GetNextTargetRecursive();
        }

        /*
		if (manager.getTileAtGridPosition == "TeleportOut")
		{
			return priorityTile.GetComponent<TeleportOutTile> ().pickTeleportIn ();
		}
		*/

        // Priority selected. If tile in target direction is not a path or has low priority, now paths are available.
        // Return target, try again next update.
        // Steven's Notes: Redundant?
        if (currentPriority < 0)
        {
            return target;
        }

        // If has reached this, has found a valid path to traverse to.
        // Corrects the target tile's grid location.
        switch (priorityLocation)
        {
            // NORTH
            case 0:
                targetTileY += 1;
                anim.SetBool("SquibbleFacesCamera", false);
                render.flipX = true;
                break;

            // EAST
            case 1:
                targetTileX += 1;
                anim.SetBool("SquibbleFacesCamera", false);
                render.flipX = false;
                break;

            // WEST
            case 2:
                targetTileX -= 1;
                anim.SetBool("SquibbleFacesCamera", true);
                render.flipX = true;
                break;

            // SOUTH
            case 3:
                targetTileY -= 1;
                anim.SetBool("SquibbleFacesCamera", true);
                render.flipX = false;
                break;
        }
        return priorityTile;
    }


    /*
	 * GET PRIORITIES OF CURRENT TILE
	 * Since called during initial getNewTarget() only, Target == Tile currently occupied by this Squibble.
	 * All changes made to priority are done in this local list: occupyied tile priorities are unaffected.
	 */
    private void populateTileList()
    {
        tilePriorityList[0] = target.northPriority;
        tilePriorityList[1] = target.eastPriority;
        tilePriorityList[2] = target.westPriority;
        tilePriorityList[3] = target.southPriority;

        // Check for backward direction (backwards direction has lowest priority always in Squibble pathing).
        // For each direction...
        for (short i = 0; i < 4; i++)
        {
            // If < 0, this direction is irrelevent. Continue.
            if (tilePriorityList[i] < 0)
            {
                continue;
            }

            // If Backwards, set priority to 0 (lowest possible that signifies a viable path).
            // North + South = 0 + 3 = 3
            // East + West = 1 + 2 = 3
            else if ((i + priorityLocation) == 3)
            {
                tilePriorityList[i] = 0;
            }

            // If not backwards, assure higher than 0 so that backwards will become lowest.
            else
            {
                tilePriorityList[i] += 1;
            }
        }

    }

    /* 
	 * From the current priority list, retrieve the highest priority direction.
	 * Sets priorityLocation to the current highest priority direction.
	 * Sets CurrentPriority to that direction's priority value.
	 */
    private void getHighestPriority()
    {
        currentPriority = short.MinValue + 1;
        priorityLocation = 0;
        for (short current = 0; current < 4; current++)
        {
            // If higher priority found, set to that direction.
            if (tilePriorityList[current] > currentPriority)
            {
                currentPriority = tilePriorityList[current];
                priorityLocation = current;
            }
            // If tie found, randomly decide whether to switch or not.
            else if (tilePriorityList[current] == currentPriority)
            {
                if (randomPick())
                {
                    currentPriority = tilePriorityList[current];
                    priorityLocation = current;
                }
            }
        }
    }


    /*
	 * Retrieves the Tile from the Tile Grid in the direction of the Priority Location.
	 * May return Null if target tile not found.
	 * Direcitons: North(0), East(1), West(2), South(3).
	 */
    private Tile getPriorityTile(short location)
    {
        GameObject newLoc = null;
        switch (location)
        {
            case 0:
                //NORTH
                newLoc = gc[targetTileX, targetTileY + 1];
                break;

            case 1:
                //EAST
                newLoc = gc[targetTileX + 1, targetTileY];
                break;

            case 2:
                //WEST
                newLoc = gc[targetTileX - 1, targetTileY];
                break;

            case 3:
                //SOUTH
                newLoc = gc[targetTileX, targetTileY - 1];
                break;
        }
        // Could return null if manager returns a null tile or Target Tile is out of array bounds.
        if (newLoc)
        {
            return newLoc.GetComponent<Tile>();
        }
        return null;
    }


    /*
	 * Randomly make a decision between to directional priorities. 50/50 odds.
	 */
    private bool randomPick()
    {
        return ((Random.Range(0, 100) % 2) == 0);
    }
}
