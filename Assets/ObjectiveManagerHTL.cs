using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using HWTools.Grid;
using HTLWizards;

/// <summary>
/// Manages Level Completion, Objective Tracking, Level Timer, and associated UI elements.
/// DOES NOT MANAGE Wizard-related UI elements such as Wizard-Spawning buttons or Move Limits.
/// </summary>
public class ObjectiveManagerHTL : MonoBehaviour
{
    public WizardManager wizMan;
    public Text heartPointsCounter;

	#region Private Fields

	private SqibbleFactory factory;
	private SpawnButton button;
	private int[] waveInfoArray = {1,10,2,5,5,1,1,1,1,1,1,1,1,1,1};
	private Vector3 vec;

	#endregion

    #region Time Fields
    private Timer gameTime;
    public bool TimerCountdownModeActive;
    public int TimeLimitInSeconds = 1;
    public Text gameTimerText;

    private bool paused;
    #endregion

    public SquibbleRescueContainer[] rescueCounters;    
    private Dictionary<string, int> squibbleRescueCounters;
    private float heartPoints;
    
    void Start()
    {
		// TODO A LEVEL CONFIG FILE ENCODER AND DECODER
		// PROBABLY HAVE LEVEL EDITOR CREATE LIST OF SPAWN BUTTONS
		// TODO DISPLAY WAVE AND SQUIBBLE LOGISTICS
		factory = GameObject.Find("SquibbleFactory").GetComponent<SqibbleFactory>();
		button = GameObject.Find ("SpawnButton").GetComponent<SpawnButton> ();
		vec = new Vector3 (-4,7,0);
		factory.SetSpawnPoint(vec);
		button = GameObject.Find ("SpawnButton").GetComponent<SpawnButton> ();
		button.GetComponent<Spawner> ().delaySpawnTime = 1F;
		factory.buttonQueue.Enqueue (button);
		factory.SetWaveInfoArray (waveInfoArray);


        squibbleRescueCounters = new Dictionary<string, int>();
        //Initialize all counters in the Dictionary to 0.
        foreach(var squibbleButtonPair in rescueCounters)
        {
            squibbleRescueCounters[squibbleButtonPair.squibble.gameObject.name] = 0;
            Debug.Log(squibbleButtonPair.squibble.gameObject.name + " " + squibbleRescueCounters[squibbleButtonPair.squibble.gameObject.name]);
        }

        //If countdown mode, set countdown time. Timer Loops at the end
        if (TimerCountdownModeActive)
        {
            gameTime = new Timer(TimeLimitInSeconds, null, null, null, false, true);
        }
        // If not countdown mode, set timer to maximum value for long-lasting timer.
        else
        {
            gameTime = new Timer(float.MaxValue);
        }
        gameTime.Run();
        paused = false;

        heartPoints = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
		Debug.Log (factory.totalSquibbleNumber);
		Debug.Log(factory.TotalSquibbleWaves(vec));
		Debug.Log (factory.SquibbleNumberCurrentWave (vec));
		Debug.Log (factory.SquibbleNumberNextWave (vec));

        // Display Game Timer
        if (gameTimerText && gameTime != null)
        {
            if (TimerCountdownModeActive)
            {
                //Display Time Remaining in Minutes:Seconds
                gameTimerText.text = gameTime.F("{5}:{2:00}");

            }
            else
            {
                //Display Time Passed in Minutes:Seconds
                gameTimerText.text = gameTime.F("{3}:{0:00}");
            }
        }

        //A dictionary entry for each squibbleButtonPair in rescueCounters was created in the Start() function.
        foreach(var squibbleButtonPair in rescueCounters)
        {
            squibbleButtonPair.textBox.text = "" + squibbleRescueCounters[squibbleButtonPair.squibble.gameObject.name];
        }

        if (Input.GetButtonDown("pause"))
        {
            if (paused)
            {
                Time.timeScale = 1.0f;
                paused = false;
            }
            else
            {
                Time.timeScale = 0.0f;
                paused = true;
            }
        }
    }

    //pause the game
    public void pause()
    {
        if (paused)
        {
            Time.timeScale = 1.0f;
            paused = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            paused = true;
        }
    }

    /// <summary>
    /// Notification function, called by the End of Path Tiles
    /// Tells the Objective Manager that a squibble of type squib has been rescued
    /// </summary>
    /// <param name="squib"></param>
    public void rescue(Squibble squib)
    {
        squibbleRescueCounters[squib.gameObject.name]++;
        heartPoints += squib.Stats.AddOrGetUnique("health").EffectiveValue;
        heartPointsCounter.text = "" + Mathf.FloorToInt(heartPoints);
    }
}

/// <summary>
/// Class for managing the Squibble Rescue Counters and assigning text.
/// </summary>
[Serializable]
public class SquibbleRescueContainer
{
    public Squibble squibble;
    public Text textBox;
}
