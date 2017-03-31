using UnityEngine;
using System.Collections;

/*
 * Basic (Turnip) Squibble
 * This is a basic Squibble that has basic(default) health and speed multipliers
 * This class is a child of Squibble class which is a child of Tile Object class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class SquibbleBasic : Squibble
{

	/*
     * @Inherits parent method
     * Used for initialization
     */
	new void Start ()
    {
        base.Start();
    }

    /*
     * @Inherits parent method
     * Update is called once per frame
     */
    new void Update ()
    {
        base.Update(); 
	}
}
