using UnityEngine;
using System.Collections;

/*
 * Obstacle
 * This is an abstract obstacle class that is a parent to different kinds of HTL obstacles
 * This class is a child of Tile Object class
 * TODO: CURRENTLY NOT ABSTRACT BECAUSE NO SPECIFIC CHILD OBSTACLE TYPES CREATED
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class Obstacle : TileObject
{
    /* Boolean variables that determine the state and type of obstacle
     * Passable obstacle can be passed by a Squibble and it can be toggled
     * Permanent obstacle cannot be passed by a Squibble
     */
    public bool passable;
    public bool permanent;

    /*
     * @Inherits parent method
     * Used for initialization.
     */
    new void Start ()
    {
        base.Start();

        // Labels the object as "Obstacle"
        this.tag = "Obstacle";
	}

    /*
     * @Inherits parent method
     * Update is called once per frame.
     */
    new void Update ()
    {
        base.Update();
    }

    /*
     * Testing function to make sure a connection to this obstacle is properly established.
     */
    public void beep()    
    {
        Debug.Log("Beep Beep! Passable = " + passable);
    }
}
