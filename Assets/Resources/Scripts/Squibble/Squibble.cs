using UnityEngine;
using System.Collections;
using HWTools.Grid;
using HWTools.GameStats;

/// Squibble
/// <summary>
/// This is an abstract class that represents Squibbles, which traverse the grids in HTL.
/// This class is a parent object to all varieties of Squibble.
///
/// Directional Priorities map to the following values:
/// North = 0
/// East = 1
/// West = 2
/// South = 3
/// Acronym to help remember: NEWS = NORTH, EAST, WEST, SOUTH.///
/// </summary>
/// @author Steven Roberts
/// @author Nenad Bulicic
public partial class Squibble : MonoBehaviour, IHasStats
{
	[SerializeField, HideInInspector]
	GameStatContainer _stats;

	public GameStatContainer Stats
	{
		get
		{
			return _stats;
		}
	}

	// Tracks Stamina of Squibbles. When _stats["health"].baseValue reaches 0, the squibble dies.
	public float maxStamina;

	// Squibble Stamina Loss per Second. Combined with Obstacle and Tile Multipliers during movement.
	public float baseStaminaLossRate;

	// Movement Speed of Squiggle in Seconds per Tile.
	public float baseSpeedMultiplier; // todo: replace entirely with GameStatContainer item

	#region external reference variables
	//Reference to next tile to move to. During new target selection, this tile is currently occupied by the Squibble
	private Tile target;

	// Grid Space Location of the current Tile.
	private int targetTileX, targetTileY;

	// Reference to the Game Manager for access to the grid and other manager functions.
	//private GameManager manager;

	private GridTransform gt;
	private Grid2DCollection gc;

	#endregion

	// Float distance in unity units from center of tile where Squibble will select a new direction.
	public float centerOfTileThreshold = 0.05f;

	#region Tile selection variables
	// Current tile being verified as a candidate to be the Squibble's next movement target.
	private Tile priorityTile;

	// Current Tile's directional priorities, in an array. 4 values in order: North, East, West, South.
	private short[] tilePriorityList = new short[4];

	// Short value representing the Direction (North(0), East(1), West(2), South(3)) to the target tile,
	//  relative to the currently occupied tile.
	// While a Squibble is Moving and when it begins choosing a next path, 
	//  this value represents its current direction of movement.
	private short priorityLocation;

	// The priority value of the direction currently being considered.
	private short currentPriority;
	#endregion
	bool alreadyTeleported;

	Animator anim;
	SpriteRenderer render;


	/*
	 * Use this for initialization.
	 */
	protected void Start ()
	{
        // Create GameStatContainer to handle outside modifiers to Stats.
		_stats = new GameStatContainer();
		_stats.AddOrGetUnique("speed").baseValue = 1;// <- temporary test value
        _stats.AddOrGetUnique("healthRate").baseValue = 1;// <- temporary test value
		var health = _stats.AddOrGetUnique("health");
		health.baseValue = maxStamina;
		health.hasMax = true;
		health.max = maxStamina;

		// Ensures that Squibbles begin with no particular directional value.
		// All directional priorities are considered equally upon initial spawn.
		priorityLocation = -1;

		gt = this.GetComponent<GridTransform>();
		//Debug.LogWarning("squibble grid position: " + gt.GridPosition);
		if (gt.parent == null)
		{
			//Debug.LogWarning("gt.parent = null");
			gt.findGrid();
		}
		//Debug.LogWarning("squibble grid position: " + gt.GridPosition);
		gc = gt.parent.gameObject.GetComponent<Grid2DCollection>();
		if (gc == null)
		{
			Debug.LogWarning("Squibble has no Grid2DCollection");
		}
		
		target = gc[gt.GridPosition].GetComponent<Tile>();

		currentPriority = 0;
		targetTileX = (int)gt.GridPosition.x;
		targetTileY = (int)gt.GridPosition.y;
		//Debug.LogWarning("squibble target location: " + target.transform.localPosition);

		anim = GetComponent<Animator>();
		if (anim == null)
		{
			anim = GetComponentInChildren<Animator>();
			if (anim == null)
			{
				Debug.LogError("Animator not found");
			}
		}
		Debug.Log("Animator: " + anim);

		render = GetComponent<SpriteRenderer>();
		if (render == null)
		{
			render = GetComponentInChildren<SpriteRenderer>();
			if (render == null)
			{
				Debug.LogError("Sprite not found");
			}
		}
        //gt.GridPosition = target.transform.localPosition;

        _stats["health"].baseValue = maxStamina;
	}
	

	/*
	 * Update is called once per frame.
	 */
	protected void Update ()
	{
		Move();

        // Handle Stamina Loss and Death
        if (_stats["health"].baseValue < 0f)
        {
            //DIE
            //TEMPORARY REVIVE
            _stats["health"].baseValue = maxStamina;
        }
        else
        {
            _stats["health"].baseValue -= baseStaminaLossRate * Time.smoothDeltaTime * _stats["healthRate"].EffectiveValue;
        }
	}


	/*
	 * Move towards next tile. Called once per Update().
	 * Also detects if the Squibble needs to check for the next Path.
	 * TODO: ACCOUNT FOR OBSTACLE SPEED EFFECTS.
	 */
	private void Move()
	{
		if (target != null)
		{
			// Vector from current Squibble Position to the Center of the target tile.
			Vector3 distVec = target.GridTransform.GridPosition - gt.GridPosition;
			Vector3 moveVec = distVec;
			Tile curTile = gc[gt.GridPosition].GetComponent<Tile>();



			//do special tile effects for when squibble walks on it (if necessary). Currently, no Tiles use this, but leave for Design.
			//curTile.OnWalk();

			// If close enough to target, get a new target.
			if (Vector3.Magnitude(moveVec) < centerOfTileThreshold)
			{
				curTile.OnDirectionPick(this);
				if (!this.gameObject.activeSelf)
				{
					return;
				}
				if ((curTile.tag == "TeleportOut") && curTile.GetComponent<TeleportOutTile>().isActive)
				{
					target = curTile.GetComponent<TeleportOutTile> ().pickTeleportIn ();
					Vector2 newCoordinates = target.GridTransform.GridPosition;
					targetTileX = (int)newCoordinates.x;
					targetTileY = (int)newCoordinates.y;
					gt.GridPosition = target.GridTransform.GridPosition;
				}
				//Vector3 pos = target.GetComponent<GridTransform>().GridPosition;
				target = GetNextTarget();
				//Debug.Log(pos + " To\n "
				//	+ target.GetComponent<GridTransform>().GridPosition);
			}
						
			// Move in direction of MoveVec at speed based on Squibble, current Tile, and Obstacle (TODO) speed multipliers.
			// Don't move if current movement priority is less than 0.
			if (currentPriority >= 0)
			{
				moveVec.Normalize();
				//Tile curTile = manager.getTileAtWorldPosition(this.transform.position);

				//do special tile effects for when squibble walks on it (if necessary).
				curTile.OnWalk(this);
				
				// Get net Speed Multiplier. If speed set to 0 Seconds per Tile, set Speed Multiplier to 0.
				float speedMultiplier = 0;
				if (baseSpeedMultiplier > 0f)
				{
					// Convert Seconds/Tile to Tiles per Second.
					speedMultiplier = 1 / baseSpeedMultiplier;

					// If finds a tile with valid speed multiplier, uses it.
					if (curTile != null && curTile.speedMultiplier >= 0f)
					{
						speedMultiplier *= curTile.speedMultiplier;
					}

				}
				
				moveVec = moveVec * speedMultiplier * Time.smoothDeltaTime * Stats["speed"].EffectiveValue;
                // Prevent Squibble from overshooting target destination.
				if (moveVec.magnitude > distVec.magnitude)
				{
					moveVec = distVec;
				}
				gt.GridPosition += moveVec;
			}
		}

		// If no target, do nothing.
		else
		{
			Debug.Log("TARGET = NULL");
		}     
	}
}

