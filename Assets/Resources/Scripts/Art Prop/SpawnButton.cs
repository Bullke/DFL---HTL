using UnityEngine;
using System.Collections;

/*
 * SpawnButton Art Prop
 * This is a Spawn Button art prop object.
 * This class is a child of Art Prop class which is a child of the Tile Object class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class SpawnButton : ArtProp
{
	// Reference to the Spawner
	private Spawner spawner;

    public Sprite waveInactiveSprite;
    public Sprite waveActiveSprite;

	/*
     * @Inherits parent method
     * Used for initialization
     * Finds the Spawn Factory game object for reference
     */
	new void Start()
	{
		base.Start();
		// Find Spawner to talk to it when to start the wave
		spawner = this.GetComponent<Spawner> ();
		//spawner = GameObject.Find();
        //waveInactiveSprite = Resources.Load<Sprite>("art/Old/Java HTL/DevArt/barricade_100");
        //waveActiveSprite = Resources.Load<Sprite>("art/Old/Java HTL/Background/Path-Background Pieces/Path1");
        

    }
		
	/*
     * @Inherits parent method
     * Update is called once per frame
     * Listens for button down input to start Squibble wave via SquibbleFactory
     * Checks if wave is spawning and changes the Sprite accordingly
     */
	new void Update()
	{
		base.Update();
		// Checks to see if Spawn Factory's waveEnded is true or false
		// True = Use start button Sprite, False = Use path Sprite
		if ((spawner != null) && spawner.waveEnded)
		{
            //this.GetComponent<SpriteRenderer> ().enabled = true;
            //this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("barricade_100");
            this.GetComponent<SpriteRenderer>().sprite = waveInactiveSprite;
        }
        else
		{
            //this.GetComponent<SpriteRenderer> ().enabled = false;
            this.GetComponent<SpriteRenderer>().sprite = waveActiveSprite;

        }
		// Check for user input
		if (Input.GetKeyDown ("down"))
		{
			// If there is no wave currently running start the wave
			if (spawner.waveEnded)
			{
				spawner.startWave = true;
			}
		}


	}

	/*
     * @override ArtProp
     * This function populates the Art Prop's boolean grid with values
     */
	protected override void populatePropGrid()
	{
		base.populatePropGrid();
		//setPropGridValue(-4, 7, false);
	}

	/*
     * @Inherits parent class method
     * Detaches the art prop object from the Tile 
     */
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
