using UnityEngine;
using System.Collections;

/*
 * Teleport In Tile
 * This tile has no particular behaviors, but acts as a potential landing point for Teleport Out Tiles
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class TeleportInTile : BasicPath
{
	public bool isActive = false;
	// Use this for initialization
	new void Start ()
	{
		base.Start();
	}

	// Update is called once per frame
	new void Update ()
	{
		base.Update();
		this.checkForwardFirst = true;
	}
}
