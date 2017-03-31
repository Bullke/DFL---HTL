using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Teleport Out Tile
 * When a Squibble steps on this tile, it teleports to one of the Teleport In tiles available to this tile.
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class TeleportOutTile : BasicPath
{
    /// <summary>
    /// If True, Squibbles that step on this tile will Teleport
    /// </summary>
	public bool isActive;

    //Link to Grid

    /// <summary>
    /// List of Teleport Locations when teleporting from this tile
    /// </summary>
	public List<Tile> teleportInTileList = new List<Tile>();

	// Use this for initialization
	new void Start ()
	{
		base.Start();
	}

	// Update is called once per frame
	new void Update ()
	{
		base.Update();
	}

    /// <summary>
    /// Stores Target Tiles in this Teleporter's potential Teleport In List
    /// </summary>
    /// <param name="target">The Tile to add</param>
    /// <returns>Returns True if target Tile was added to Teleport In List</returns>
	public bool storeTeleportIn(Tile target)
	{
        //Verify that target exists, is a valid path, and is not already in the Teleport In List
		if (target != null && target.isAPath && !teleportInTileList.Contains(target))
		{
			teleportInTileList.Add(target);
			return true;
		}
		return false;
	}

    /// <summary>
    /// Randomly select a Teleport In from the list
    /// </summary>
    /// <returns>A Tile that is a Path</returns>
	public Tile pickTeleportIn()
	{
		return (Tile)teleportInTileList [Random.Range (0, teleportInTileList.Count)];
	}
}
