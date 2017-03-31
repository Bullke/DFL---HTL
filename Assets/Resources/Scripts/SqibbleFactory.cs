using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HWTools.Grid;

public class SqibbleFactory : MonoBehaviour
{
	#region Public Fields

	/// <summary>
	/// The total number of Squible in the current Level.
	/// </summary>
	public int totalSquibbleNumber;

	/// <summary>
	/// Dictionary of total number of Squibble waves for each button. 
	/// Container is an int.
	/// </summary>
	public Dictionary<Vector3, int> totalSquibbleWaveNumberMap;

	/// <summary>
	/// Dictionary of total number of Squibble per waves for each button. 
	/// Containers is a List.
	/// </summary>
	public Dictionary<Vector3, List<int>> waveSquibleCountMap;

	#endregion

	#region Private Fields

	/// <summary>
	/// The current position in the waveInfoArray which contains all of the information needed
	/// to construct all waves and assignments. Only used once per factory initialization.
	/// </summary>
	private int position;

	/// <summary>
	/// The total number of Squibble for the entire level.
	/// </summary>
	private int totalSquibbleWaveNumber;

	/// <summary>
	/// Used upon initialization of factory to determine if entire Factory has been initialized.
	/// </summary>
	private bool alreadyInitialized;

	/// <summary>
	/// The total number of Spawn Points for a spawn location.
	/// </summary>
	private int totalSpawnPointsNumber;

	/// <summary>
	/// The total number of Spawn points for the level.
	/// </summary>
	private int spawnPointCount;

	/// <summary>
	/// The total number of Squibble enum types.
	/// </summary>
	private int numberOfSquibbleTypes = Enum.GetNames(typeof(SquibbleType)).Length;

	/// <summary>
	/// The Squibble prefab file folder path
	/// = "Prefabs/"
	/// </summary>
	private string squibblePrefabPath = "Prefabs/";

	/// <summary>
	/// An array of Squibble prefab types.
	/// Size = numberOfSquibbleTypes
	/// </summary>
	private GameObject[] prefabList;

	/// <summary>
	/// A list containing spawn locations for all buttons.
	/// </summary>
	private List<Vector3> spawnLocationList;

	/// <summary>
	/// A Dictionary of Spawners attached to Spawn Buttons.
	/// </summary>
	private Dictionary<Vector3, Spawner> spawnerMap;

	/// <summary>
	/// A Queue of Squibble removed from the level used for recycling.
	/// </summary>
	private Queue<GameObject> recycleQueue;

	/// <summary>
	/// A Queue of Squibble waves passed to each Spawner.
	/// </summary>
	private Queue<Queue<GameObject>> squibbleWaveQueue;

	/// <summary>
	/// A list of wave sizes for each spawn location.
	/// </summary>
	private List<int> waveSize;

	/// <summary>
	/// Used for Grid attaching
	/// </summary>
	private Grid2DCollection temp;

	/// <summary>
	/// Spawn point location used as reference for each button/spawn point.
	/// </summary>
	private Vector3 spawnPoint;


	//TODO TODO TODO TODO
	/// <summary>
	/// The total number of Squibble for the entire level.
	/// <para/>
	/// = √3
	/// </summary>
	private int[] waveInfoArray;
	/// <summary>
	/// The total number of Squibble for the entire level.
	/// <para/>
	/// = √3
	/// </summary>
	private SpawnButton button;
	/// <summary>
	/// The total number of Squibble for the entire level.
	/// <para/>
	/// = √3
	/// </summary>
	public Queue<SpawnButton> buttonQueue;
	/// <summary>
	/// The total number of Squibble for the entire level.
	/// <para/>
	/// = √3
	/// </summary>
	private Dictionary<Vector3, SpawnButton> buttonMap;

	#endregion

	#region Public Enums

	/// <summary>
	/// Enum for all prefab Squibble types. Requires update for each new prefab that is added.
	/// </summary>
	public enum SquibbleType
	{
		WalkingSquib1
	}

	#endregion

	#region Public Properties

	/// <summary>
	///   Total number of Squibble getter.
	/// </summary>
	/// <returns> Total number of Squibble for entire level. </returns>
	public int TotalSquibble ()
	{
		return totalSquibbleNumber;
	}

	/// <summary>
	///   Total number of rescued Squibble getter.
	/// </summary>
	/// <returns> Total number of rescued Squibble for entire level. </returns>
	public int TotalResquedSquibble ()
	{
		return recycleQueue.Count;
	}

	/// <summary>
	///   Total number of Squibble waves for spawn location getter.
	/// </summary>
	/// <param name="target"> The Vector3 coordinate of Spawn location </param>
	/// <returns> Total number of Squibble waves. </returns>
	public int TotalSquibbleWaves (Vector3 target)
	{
		// Error handling
		if (totalSquibbleWaveNumberMap.ContainsKey (target) == false)
		{
			return 0;
		}
		return totalSquibbleWaveNumberMap [target];
	}

	/// <summary>
	///   Total number of Squibble for current wave for spawn location getter.
	/// </summary>
	/// <param name="target"> The Vector3 coordinate of Spawn location </param>
	/// <returns> Total number of Squibble for current wave. </returns>
	public int SquibbleNumberCurrentWave(Vector3 target)
	{
		// Error handling
		if ((waveSquibleCountMap.ContainsKey (target) == false) || (waveSquibleCountMap [target].Count == 0))
		{
			return 0;
		}
		return waveSquibleCountMap [target] [0];
	}

	/// <summary>
	///   Total number of Squibble for next wave for spawn location getter.
	/// </summary>
	/// <param name="target"> The Vector3 coordinate of Spawn location </param>
	/// <returns> Total number of Squibble for next wave. If no more waves, returns -1. </returns>
	public int SquibbleNumberNextWave(Vector3 target)
	{
		// Error handling
		if ((waveSquibleCountMap.ContainsKey (target) == false) || (waveSquibleCountMap [target].Count <= 1))
		{
			return 0;
		}
		else
		{
			return waveSquibleCountMap [target] [1];
		}
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Populates the prefab array with Squibble prefabs.
	/// </summary>
	/// <returns> Returns true/false </returns>
	private bool PopulatePrefabs()
	{
		SquibbleType type;
		// Error handling
		if (numberOfSquibbleTypes == 0)
		{
			Debug.LogError ("No Squibble Types to Preload");
			return false;
		}
		for (int i = 0; i < numberOfSquibbleTypes; i++)
		{
			// Cast as Enum by index
			type = (SquibbleType)i;
			// Add loaded Squibble prefab to list ("Prefabs/" + WalkingSquib1...etc)
			prefabList[i] = Resources.Load<GameObject>(squibblePrefabPath + type.ToString());
		}
		return true;
	}
		
	/// <summary>
	/// Initializes all Dictionaries. Spawn locations are needed as keys.
	/// </summary>
	/// <returns> Returns true/false </returns>
	private bool InitializeDictionaries()
	{
		if (spawnLocationList.Count != 0)
		{
			// Iterate through all spawn locations
			for (int i = 0; i < spawnLocationList.Count; i++)
			{
				// Get spawn point as a Vector3
				spawnPoint = spawnLocationList[i];
				// Add to Dictionary
				totalSquibbleWaveNumberMap.Add (spawnPoint, 0);
				waveSquibleCountMap.Add (spawnPoint, new List<int>());
				buttonMap.Add (spawnPoint, buttonQueue.Dequeue());
				// MAYBE NOT NEEDED IF MANAGER HANDLES THIS
				buttonMap[spawnPoint].GetComponent<Spawner>().SetSpawnLocation(spawnPoint);
				//buttonMap[spawnPoint].GetComponent<Spawner> ().delaySpawnTime = 3F;

				spawnerMap.Add(spawnPoint, buttonMap[spawnPoint].GetComponent<Spawner>());
			}
			return true;
		}
		return false;
	}

	/*
	 * Wave File is assumed to be passed in as an int array and structured this way:
	 * Total Number of Spawn Points
	 * Total Number of Squibble
	 * Total Number of Waves
	 * Wave Size for the next (Total Number of Waves) numbers
	 * Squibble type for next (Wave Size) numbers for (Total Number of Waves) times
	 * Example:
	 * 1-10-2-5-5-1-1-1-1-1-1-1-1-1-1
	 */

	/// <summary>
	/// Initializes the entire wave for the entire level. Each Spawner is passed its own Queue of waves.
	/// </summary>
	/// <returns> Returns true/false </returns>
	private bool InitializeEntireWave()
	{
		// Set to 1 because 0th index is assigned in Update and if passes if statement the waveInfoArray
		// position needs to be incremented
		position = 1;
		for(int i = 0; i < totalSpawnPointsNumber; i++)
		{
			//{1,10,2,5,5,1,1,1,1,1,1,1,1,1,1}
			// Add to sum the total Squibble number for the spawn location
			totalSquibbleNumber += waveInfoArray [position++];
			// Get the total number of waves for the spawn location
			totalSquibbleWaveNumber = waveInfoArray [position++];
			// Get the spawn point at 0 index
			spawnPoint = spawnLocationList [0];
			// Remove front so previous assignment can work
			spawnLocationList.RemoveAt (0);
			// Keep track of current spawn points #
			spawnPointCount--;
			// Update the total number of Squibble waves for current spawn point
			totalSquibbleWaveNumberMap[spawnPoint] = totalSquibbleWaveNumber;
			// Popoulate the waveSize list for current spawn point to be passed to CreateIndividualWaves()
			for(int j = 0; j < totalSquibbleWaveNumber; j++)
			{
				waveSize.Add(waveInfoArray [position++]);
			}
			// Call to create individual waves for current spawn point and pass it in
			CreateIndividualWaves ();
			// Reset the total number of Squibble waves
			totalSquibbleWaveNumber = 0;
		}
		return true;
	}

	/// <summary>
	/// Creates a Queue of individual waves for current spawn point and passes it in to it.
	/// </summary>
	/// <returns> Returns true/false </returns>
	private bool CreateIndividualWaves()
	{
		// tempObject is for instantiating the Squibble from prefab
		GameObject tempObject;
		// Iterate through the number of waves and create Squibble Queue for each wave
		for (int i = 0; i < totalSquibbleWaveNumber; i++)
		{
			//POSSIBLY NOT NEEDED
			Queue<int> singleWaveSize = new Queue<int> (); // WOULD HAVE BEEN USED TO TRACK # OF SQUIBBLE IN A SINGLE WAVE
			singleWaveSize.Enqueue (waveSize[i]); // WOULD HAVE BEEN USED TO TRACK # OF SQUIBBLE IN A SINGLE WAVE

			// Create a single wave Squibble Queue for current wave
			Queue<GameObject> singleWaveQueue = new Queue<GameObject> ();
			// Iterate through the size of the current wave
			for (int j = 0; j < waveSize[i]; j++)
			{
				// Error handling
				if ((position > waveInfoArray.Length) || (prefabList.Length == 0))
				{
					Debug.LogError ("INT Array does not have enough values - wavePosition > array.length or Prefab List is Empty");
					return false;
				}
				// Instantiate the Squibble from as GameObject by passing in the Squibble index from waveInfoArray into prefab list
				tempObject = (GameObject.Instantiate(prefabList[waveInfoArray[position++]-1]) as GameObject);
				// Remove the "(Clone)" suffix from instantiated GameObject
                tempObject.name = tempObject.name.Replace("(Clone)", "");
				// Turn off the GameObject and Scripts
                tempObject.SetActive(false);
				// Add the GameObject to the single wave Queue
				singleWaveQueue.Enqueue (tempObject);
			}
			// Add the number of Squibble for current wave and current spawn point
			waveSquibleCountMap[spawnPoint].Add(waveSize[i]);
			// Single wave Queue is complete and add to current spawn point squibble wave Queue
			squibbleWaveQueue.Enqueue (singleWaveQueue);
		}
		// Pass the entire Squibble wave Queue to the current spawn point Spawner
		spawnerMap[spawnPoint].SetWaves(squibbleWaveQueue);
		return true;
	}

	#endregion

	#region Protected Methods

	// Awake and initialize all needed variables
    void Awake()
	{
		position = 0;
		totalSquibbleNumber = 0;
		totalSquibbleWaveNumber = 0;
		spawnPointCount = 0;
		totalSpawnPointsNumber = int.MinValue;
		prefabList = new GameObject[numberOfSquibbleTypes];
		spawnLocationList = new List<Vector3> ();
		totalSquibbleWaveNumberMap = new Dictionary<Vector3, int> ();
		waveSquibleCountMap = new Dictionary<Vector3, List<int>> ();
		spawnerMap = new Dictionary<Vector3, Spawner> ();
		buttonMap = new Dictionary<Vector3, SpawnButton>();
		recycleQueue = new Queue<GameObject>();
		squibbleWaveQueue = new Queue<Queue<GameObject>> ();
		waveSize = new List<int> ();
		buttonQueue = new Queue<SpawnButton>();
		alreadyInitialized = false;

		// Call to populate initialize
		PopulatePrefabs ();
	}

	// Used to find the Grid to attach itself to
	// POSSIBLY NOT NEEDED IF OBJECTIVE MANAGER PASSES IS IT INSTEAD (NEEDS TESTING)
	void Start()
	{
		GridTransform gridTrans = this.GetComponent<GridTransform> ();
		temp = gridTrans.parent.gameObject.GetComponent<Grid2DCollection> ();
    }

	// Used to initialize Dictionaries and Squibble waves and pass them to correct Spawners
	// Keeps checking until all of the needed/correct values and their quantities (waveInfoArray and spawn points)
	// are passed in and then initializes it all
    void Update()
    {
		if(!alreadyInitialized)
		{
			// Does array existing or is empty
			if ((waveInfoArray != null) && (waveInfoArray.Length != 0))
			{
				// Set to the first value of the waveInfoArray sequence which corresponds
				// to the total number of spawn points
				totalSpawnPointsNumber = waveInfoArray [0];
				// Check if number of passed in spawn points equals the number of points
				// specified in the waveInfoArray
				if (spawnPointCount == totalSpawnPointsNumber)
				{
					if (InitializeDictionaries ())
					{
						Debug.Log ("DICTIONARIES INITIALIZED");
						if (InitializeEntireWave ())
						{
							Debug.Log ("ENTIRE WAVE INITIALIZED");
							alreadyInitialized = true;
						}
						else
						{
							// Something went wrong with InitializeEntireWave()
							Debug.LogError ("ENTIRE WAVE INITIALIZATION FAILED");
						}
					}
					else
					{
						// Something went wrong with InitializeDictionaries()
						Debug.LogError ("DICTIONARY INITIALIZATION FAILED");
					}
				}
			}
		}
    }

	#endregion

	#region Public Methods

	/// <summary>
	/// Sets waveInfoArray to the passed in int array.
	/// </summary>
	/// <param name="targetArray"> Int array passed in to be waveInfoArray </param>
	public void SetWaveInfoArray(int[] targetArray)
	{
		waveInfoArray = targetArray;
	}

	/// <summary>
	/// Passed in Vector3 spawn location is added to spawnLocationList.
	/// </summary>
	/// <param name="target"> Vector3 added to spawnLocationList </param>
	public void SetSpawnPoint(Vector3 target)
	{
		spawnLocationList.Add(target);
		// Keep track of added spawn points to use for comparison in Update()
		spawnPointCount++;
	}

	/// <summary>
	/// Passed in Squibble is first turned off and then added to recycleQueue.
	/// </summary>
	/// <param name="target"> Squibble added to recycleQueue </param>
    public void RecycleSquibble(Squibble target)
    {
        target.gameObject.SetActive(false);
		recycleQueue.Enqueue(target.gameObject);
    }

	/// <summary>
	/// Destroys all of the saved/recycled Squibble from the recycleQueue.
	/// Should be called upon level exit to prevent performance issues.
	/// </summary>
	/// <returns> Returns true/false </returns>
    public bool DestroyAllWaves()
    {
		while(recycleQueue != null)
        {
			Destroy(recycleQueue.Dequeue().GetComponent<GameObject>());
        }
        return true;
    }

	#endregion
}