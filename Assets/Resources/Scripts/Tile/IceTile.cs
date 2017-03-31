using UnityEngine;
using System.Collections;

/*
 * Ice Path Tile
 * Squibbles Slip on Ice tiles, going forward when possible.
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class IceTile : BasicPath
{

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
