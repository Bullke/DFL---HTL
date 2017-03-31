using UnityEngine;
using System.Collections;
using HWTools.Grid;

/*
 * Tile
 * Abstract class that forms the components of the grid the game is played on.
 * Tiles can have varying effects on Squibbles passing over them or TileObjects positioned on them.
 * Tiles are not aware of the grid structure on their own: that is handled by the GameManager.
 * However, Tiles do know if something is occupying them.
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

[RequireComponent(typeof(GridTransform))]
public class Tile : MonoBehaviour
{
	public GridTransform GridTransform
	{
		get { return GetComponent<GridTransform>(); }
	}

	// For Squibble Pathing, Wizard and Art Prop Placement, and potentially Sprite/Texture assignment
	public bool isAPath;

	// Directional Priorities for Squibble Pathing. 
	// Priorities determine pathable directions. Higher priorities will be traversed first.
	// Negative priorities are non-valid directions;
	public short
		northPriority = -1,
		eastPriority = -1,
		westPriority = -1,
		southPriority = -1;

	// If true, Squibble will always initially try to move FORWARD relative to its
	//  current movement direction first when crossing this Tile.
	// Best used for four-way crossroad paths and t-intersections.
	public bool checkForwardFirst = false;

	// Affects Squibbles that walk over this Tile.
	// As Multipliers, value of 1 means no net affect to Squibbles.
	public float 
		speedMultiplier = 1f, 
		staminaMultiplier = 1f;

	// Affects Wizards that are placed on this Tile.
	// STEVEN THOUGHTS: are these floats needed? Squibbles and Wizards never occupy same tile, reuse floats?
	public float wizardSpeed, wizardRadius, wizardStrength;

	//enum filter. Since so many things use a filter, do we want a global enum?

	// Currently unused. May count the number of squibbles occupying this tile.
	public int squibbleCount;

	// Potential for tiles with passable state toggle. Currently not implemented.
	// public bool passable;

	// Boolean for checking occupancy, with link to occupant.
	protected TileObject occupant = null;
	public bool occupied = false;




	//**************************\\
	//CONSTRUCTOR AND DESTRUCTOR\\
	//**************************\\

	// Constructor for subclasses to implement
	// public Tile(){}
	// Use "Start()" Instead

	/*
	 * Use this for initialization
	 */
	private void Start()
	{
		//nothing
	}

	/*
	 * Update is called once per frame
	 */
	private void Update()
	{
		//occupant = null;
	}

	/*
	 * Returns whether a GameObject (Wizard, Obstacle, or ArtProp) is present on the tile
	 */
	public bool hasTileObject()
	{
		if (occupant == null)
		{
			return false;
		}
		return true;
	}

	/*
	 * Changes the current Occupant of the Tile to a new one.
	 */
	public void setOccupant(TileObject newOccupant)
	{
		occupant = newOccupant;
		occupied = true;
	}

	/*
	 * Returns a reference to the Occuppying object.
	 * Returns null if no occupant detected.
	 */
	public TileObject getOccupant()
	{
		if (!occupied)
		{
			return null;
		}
		return occupant;
	}


	/*
	 * Destroys the GameObject occupying this Tile, and sets Tile to be Occupiable.
	 */
	public void destroyOccupant()
	{
		if (occupant != null && occupied)
		{
			GameObject g = occupant.gameObject;

			MonoBehaviour.Destroy(occupant);
			GameObject.Destroy(g);
			occupied = false;
		}
		
	}

	/*
	 * Called when a Squibble moves on this tile.
	 * To be overridden by subclasses.
	 */
	public virtual void OnWalk(Squibble squib)
	{
		return;
	}

	/*
	 * Called when a Squibble picks a new direction on this tile.
	 * To be overridden by subclasses.
	 */
	public virtual void OnDirectionPick(Squibble squib)
	{
		return;
	}


	/*
	public bool isTeleportOut()
	{
		return false;
	}
	*/
}