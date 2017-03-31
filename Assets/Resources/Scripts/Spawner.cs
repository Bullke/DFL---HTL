using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HWTools.Grid;

public class Spawner : SqibbleFactory
{
	#region Private Fields

	/// <summary>
	/// Reference to the Squibble factory to update its values.
	/// </summary>
	private SqibbleFactory factory;

	/// <summary>
	/// Count for number of started waves since the beginning.
	/// </summary>
	private int startCount;

	/// <summary>
	/// The Squibble spawn delay time as a float. Default value
	/// = 3F
	/// </summary>
	private float remainingTime = 3F;

	/// <summary>
	/// Boolian to check if next Squibble to spawn is existent to prevent null reference errors. 
	/// </summary>
	private bool nextSquibble;

	/// <summary>
	/// A Queue of Game Object Queues which represents a collection of Squibble waves.
	/// Queue contains a number of Queues of Squibble.
	/// </summary>
	private Queue<Queue<GameObject>> squibbleWave;

	/// <summary>
	/// A Queue that contains the current Squibble wave to spawn.
	/// </summary>
	private Queue<GameObject> waveToSpawn;

	/// <summary>
	/// A Queue that contains the current wave size.
	/// </summary>
	public Queue<int> currentWaveSize; // MIGHT NOT BE NEEDED BECAUSE IT UPDATES FACTORY'S MAP

	#endregion

	#region Public Fields

	/// <summary>
	/// A modifiable Squibble spawn delay time as a float.
	/// </summary>
	public float delaySpawnTime { get; set; }

	/// <summary>
	/// Boolian to keep track if wave is to be started or not. Changed by SpawnButton.
	/// </summary>
	public bool startWave;

	/// <summary>
	/// Boolian to keep track if wave has ended or not.
	/// </summary>
	public bool waveEnded;

	/// <summary>
	/// A Vector3 that contains the spawn location which equals the location of the
	/// Spawn Button this Spawner is attached to.
	/// </summary>
	private Vector3 spawnLocation;

	/// <summary>
	/// Used to locate and attach to the correct grid.
	/// </summary>
	private Grid2DCollection gridLocator;

	/// <summary>
	/// Used to perform grid transforms.
	/// </summary>
	private GridTransform gridTrans;

	/// <summary>
	/// Reference to the Squibble currently to be spawned.
	/// </summary>
	private Squibble squibbleToSpawn;

	#endregion

	#region Private Methods

	/// <summary>
	/// Used to get new Squibble wave by Dequing from squibbleWave Queue and assigning it to waveToSpawn Queue.
	/// </summary>
	private void GetNewWave()
	{
		if (squibbleWave.Count != 0)
		{
			// Dequeue from squibbleWave and assign to waveToSpawn
			waveToSpawn = squibbleWave.Dequeue ();
			// Update factory's total number of waves for this Spawn Button
			factory.totalSquibbleWaveNumberMap [spawnLocation]--;
		}
		// Error handling for check in Update()
		else
		{
			waveToSpawn = null;
		}
	}

	/// <summary>
	/// Used to et new Squibble by Dequing from waveToSpawn Queue and assigning it to tempObject Game Object.
	/// </summary>
	private void GetNewSquibble()
	{
		// Used as reference to Game Object Dequed from waveToSpawn Queue
		GameObject tempObject;
		if (waveToSpawn.Count != 0)
		{
			// Dequeue from waveToSpawn and assign to tempObject
			tempObject = waveToSpawn.Dequeue ();
			// Activate the Game Object
			tempObject.SetActive (true);
			// Set squibbleToSpawn reference to tempObject's Squibble component
			squibbleToSpawn = tempObject.GetComponent<Squibble>();
			// Update factory's total number of Squibble for the entire level
			factory.totalSquibbleNumber--;
		}
		// Error handling for check in Update()
		else
		{
			squibbleToSpawn = null;
		}
	}

	#endregion

	#region Protected Methods

	// Used to initialize all of the variables
	void Start ()
	{
		factory = GameObject.Find("SquibbleFactory").GetComponent<SqibbleFactory>();
		gridTrans = this.GetComponent<GridTransform> ();
		gridLocator = gridTrans.parent.gameObject.GetComponent<Grid2DCollection> ();

		startWave = false;
		waveEnded = true;
		nextSquibble = false;
		startCount = 0;
		squibbleWave = new Queue<Queue<GameObject>> ();
		waveToSpawn = new Queue<GameObject> ();
	}

	// Used to check if wave needs to be spawned
	void Update ()
	{
		// Does the wave need to be started?
		if (startWave)
		{
			// Get new wave and assign wave to waveToSpawn, otherwise null
			GetNewWave ();
			// Error handling
			if (waveToSpawn == null)
			{
				nextSquibble = false;
				Debug.Log ("GETNEWWAVE RETURNED NULL");
			}
			// Modify required variables to pass next section of Update()
			else
			{
				nextSquibble = true;
				waveEnded = false;
				remainingTime = delaySpawnTime;
			}
			startWave = false;
			startCount++;
		}
		// If there is remaining time, Squibble is valid, and wave has not ended yet
		if ((remainingTime <= 0) && nextSquibble && !waveEnded)
		{
			// If current wave to be spawned is not null or empty
			if ((waveToSpawn != null) && (waveToSpawn.Count != 0))
			{
				// Get new Squibble
				GetNewSquibble ();
				// Spawn Squibble at target location
				SpawnSquibble (spawnLocation);
				// Reset time
				remainingTime = delaySpawnTime;
				// Update factory's number of Squibble for current wave and this Spawn Button
				factory.waveSquibleCountMap [spawnLocation][0]--;
			}
			// Exit spawning becuase waveToSpawn is empty
			else
			{
				// Update factory's number of Squibble by removing front cotainer from the list
				factory.waveSquibleCountMap [spawnLocation].RemoveAt (0);
				// Adjust the variables to skip this section of Update()
				nextSquibble = false;
				waveEnded = true;
			}
		}
		// Decrease delay time
		remainingTime -= Time.deltaTime;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Used to modify startWave Boolian to start a new wave.
	/// </summary>
	public void StartNewWave()
	{
		startWave = true;
	}

	/// <summary>
	/// Sets the current squibbleToSpawn's Grid Transform Vector3 location to target.
	/// </summary>
	/// <param name="target"> Vector3 spawn location for the Squibble </param>
	public void SpawnSquibble(Vector3 target)
	{
		squibbleToSpawn.GetComponent<GridTransform> ().SetReference (gridLocator.GetComponent<Grid2D> ());
		squibbleToSpawn.GetComponent<GridTransform> ().GridPosition = target;
	}

	/// <summary>
	/// Sets the gridLocation to passed in target Grid to be used as a reference Grid to attach to.
	/// </summary>
	/// <param name="targetGrid"> Grid2DCollection passed in to be used as Grid reference </param>
	public void SetGridLocator(Grid2DCollection targetGrid)
	{
		gridLocator = targetGrid;
	}

	/// <summary>
	/// Sets the squibbleWave to inputed Queue of GameObject Queues from which new Squibble waves can extracted from.
	/// </summary>
	/// <param name="inputQueue"> Queue of GameObject Queues to get the Squibble Queues from </param>
	public void SetWaves(Queue<Queue<GameObject>> inputQueue)
	{
		squibbleWave = inputQueue;
	}

	/// <summary>
	/// Sets the spawnLocation to inputed Vector3 to be used as spawn location for Squibble.
	/// </summary>
	/// <param name="targetLocation"> Vector3 spawn location for the spawn location variable </param>
	public void SetSpawnLocation(Vector3 targetLocation)
	{
		spawnLocation = targetLocation;
	}

	#endregion
}
