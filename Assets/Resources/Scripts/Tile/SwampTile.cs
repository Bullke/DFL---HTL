using UnityEngine;
using System.Collections;

/*
 * Swamp Tile
 * Squibbles that step into the center of a swamp tile become stuck. They can be freed by having other squibbles pass into the level.
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class SwampTile : BasicPath
{
    /// <summary>
    /// The number of squibbles required to be in a tile before squibbles can be freed
    /// </summary>
    public int squibbleThreshold;

    // State: Swamp Tile is able to grab and hold Squibbles
    private bool holdSquibbles;

    // Stores the Original speed multiplier of the tile while it is holding Squibbles
    private float originalSpeedMultiplier;

	// Use this for initialization
	new void Start ()
	{
        base.Start();
        holdSquibbles = true;
        originalSpeedMultiplier = speedMultiplier;
	}
	
	// Update is called once per frame
	new void Update ()
	{
        base.Update();
	}


    //Called by Squibbles walking ont his tile
    override public void OnWalk(Squibble squib)
    {
        squibbleCount++;
    }

    public override void OnDirectionPick(Squibble squib)
    {
        // When a Squibble reaches the center, it gets stuck. Other squibbles stop to help.
        base.OnDirectionPick(squib);
        if (holdSquibbles)
        {
            speedMultiplier = 0f;
        }
    }

    /*
     * Done after all other update functions are complete. SquibbleCount here will equal the number of squibbles on the tile.
     */
    new void LateUpdate()
    {
        //If enough squibbles around to help, he is unstuck. Movement proceeds. Else do not.
        if (squibbleCount >= squibbleThreshold)
        {
            holdSquibbles = false;
            speedMultiplier = originalSpeedMultiplier;
        }
        else
        {
            holdSquibbles = true;
        }
        // Reset squibblecount
        squibbleCount = 0;
    }
}
