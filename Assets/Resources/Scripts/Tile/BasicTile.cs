using UnityEngine;
using System.Collections;

/*
 * Basic Non-Path Tile
 * This is a basic Tile that has basic(default) variable values and sprites
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class BasicTile : Tile
{

    // Variables used for sprites and renderer
    private new SpriteRenderer renderer;
    public Sprite hasWizardSprite;
    private Color unoccupiedColor;

    /*
     * Used for initialization
     */
    void Start ()
    {
        renderer = GetComponent<SpriteRenderer>();
        unoccupiedColor = renderer.color;
	}

    /*
     * Update is called once per frame
     */
    void Update ()
    {
        /*
         * If the Tile is occupied the sprite is set to the red(denied) default sprite
         * Otherwise the sprite is set to the default sprite
         */
        if (this.occupied)
        {
            renderer.color = Color.blue;
        }
        else
        {
            renderer.color = unoccupiedColor;
        }
        
    }
}
