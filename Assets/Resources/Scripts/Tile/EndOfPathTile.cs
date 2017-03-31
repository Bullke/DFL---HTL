using UnityEngine;
using System.Collections;

/// <summary>
/// Tile type that serves as a goal for the Squibbles to reach.
/// Squibbles that reach this tile are removed from the grid and counted toward level objectives
/// </summary>
public class EndOfPathTile : BasicPath
{
    SqibbleFactory factory;
    public ObjectiveManagerHTL objMan;

    #region Public Methods
    /// <summary>
    /// Called when a Squibble moves on this tile.
    /// </summary>
    /// <param name="squib">The Squibble being affected (passes itself)</param>
    public override void OnWalk(Squibble squib)
    {
        return;
    }

    /// <summary>
    /// Called when a Squibble reaches the center of this tile and would change direction.
    /// This event is where squibbles are rescued
    /// </summary>
    /// <param name="squib"></param>
    public override void OnDirectionPick(Squibble squib)
    {
        if (objMan)
        {
            objMan.rescue(squib);
        }
        factory.RecycleSquibble(squib);
        //TODO: Contribute to level end conditions, score, etc.
        return;
    }
    #endregion

    #region Unity Runtime Methods
    new void Start ()
    {
        base.Start();
        factory = GameObject.Find("SquibbleFactory").GetComponent<SqibbleFactory>();
        objMan = GameObject.Find("Objective Manager").GetComponent<ObjectiveManagerHTL>();
	}
	
	// Update is called once per frame
	new void Update ()
    {
        base.Update();
	}
    #endregion 
}
