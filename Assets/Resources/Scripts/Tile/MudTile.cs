using UnityEngine;
using System.Collections;

/*
 * Mud Path Tile
 * Mud paths slow Squibbles passing through them and may reduce effects from Wizards
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class MudTile : BasicPath
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
		//this.checkForwardFirst = true;
	}
}
