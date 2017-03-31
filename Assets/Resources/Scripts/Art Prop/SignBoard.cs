using UnityEngine;
using System.Collections;

/*
 * SignBoard Art Prop
 * This is a Sign Board art prop object.
 * This class is a child of Art Prop class which is a child of the Tile Object class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class SignBoard : ArtProp
{

    /*
     * @Inherits parent method
     * Used for initialization
     */
    new void Start()
    {
        base.Start();
    }


    /*
     * @Inherits parent method
     * Update is called once per frame
     */
    new void Update()
    {
        base.Update();
    }

    /*
     * @override ArtProp
     * This function populates the Art Prop's boolean grid with values
     */
    protected override void populatePropGrid()
    {
        base.populatePropGrid();
        setPropGridValue(0, 0, false);
        setPropGridValue(0, 1, false);
        setPropGridValue(2, 0, false);
        setPropGridValue(2, 1, false);
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
