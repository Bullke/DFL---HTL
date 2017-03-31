using UnityEngine;
using System.Collections;

/// <summary>
/// Provides UI showing the Health of the Squibble to the user
/// </summary>
public class SquibbleLifeDisplay : MonoBehaviour {


    private Squibble squib;
    private SpriteRenderer spRender;
    /// <summary>
    /// Used as a percent: 0 - 100
    /// </summary>
    public float healthPercent;

    // RGB Color Values, from 0 to 1
    private float greenVal;
    private float redVal;



	// Use this for initialization
	void Start ()
    {
        // Connect to Components
	    spRender = this.GetComponent<SpriteRenderer>();
        squib = this.GetComponentInParent<Squibble>();

        // Initialize values as Current Stamina = Maximum Stamina
        spRender.color = Color.green;
        healthPercent = 100f;
        greenVal = 1f;
        redVal = 0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        #region TEST CODE BLOCK
        /*
        //Death Condition. For now reset.
	    if (healthPercent < 0f)
        {
            healthPercent = 100f;
        }
        else
        {
            //lose 10 health per second
            healthPercent -= Time.smoothDeltaTime * 10;
        }
        */
        #endregion

        healthPercent = (squib.Stats["health"].baseValue / squib.maxStamina) * 100;

        // Update color of health pip based on HP Percentage
        // Red value increases from min to max as HP Decreases from 100 to 50 percent
        // Green value decreases from max to min as HP Decreases from 50 to 0 percent
        greenVal = healthPercent / 50f; 
        redVal = (100f - healthPercent) / 50f;
        if (greenVal > 1f)
        {
            greenVal = 1f;
        }
        if (redVal > 1f)
        {
            redVal = 1f;
        }
        spRender.color = new Color(redVal, greenVal, 0f);
	}
}
