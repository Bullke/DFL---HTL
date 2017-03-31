using UnityEngine;
using System.Collections;

/*
 * Basic Path Tile
 * This is a basic Path that has basic(default) variable values and sprites
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class BasicPath : Tile
{
    // Variables for the sprite and renderer
    private Sprite defaultSprite;
    private new SpriteRenderer renderer;
    private Color baseColor;

    bool hasSquibble = false;

    // Set of sprite variables corresponding to varius path directions
    public Sprite horizontalSprite;
    public Sprite verticalSprite;
    public Sprite northAndEastCorner;
    public Sprite southAndEastCorner;
    public Sprite northAndWestCorner;
    public Sprite southAndWestCorner;


    /*
     * Used for initialization
     */
    protected void Start ()
    {
        renderer = GetComponent<SpriteRenderer>();
        defaultSprite = renderer.sprite;
        baseColor = renderer.color;
    }

    /*
     * Update is called once per frame
     */
    protected void Update ()
    {
        // Sets to corresponding sprite based on current directional input values;

        //COMMENTED OUT UNTIL SPRITE CHANGE MECHANIC DESIGN FINALIZED
        //setSprite(getSpriteDirectionValue());
        //renderer.color = baseColor;
        if (this.occupied)
        {
            renderer.color = Color.blue;
        }
        else
        {
            renderer.color = baseColor;
        }
    }

    
    
    /// <summary>
    /// Update that occurs after all other updates. Ensures that Squibbles have all updated first.
    /// </summary>
    protected void LateUpdate()
    {
        if (hasSquibble)
        {
            hasSquibble = false;
        }
        else
        {
            //renderer.color = baseColor;
        }
    }

    /* 
     * Sets the corresponding directional sprite that matches the case value
     * Case values are between 0-15
     */
    void setSprite(int spriteDirectionValue)
    {
        switch (spriteDirectionValue)
        {
            case 1:
                //NORTH
                //TODO: GET NEW SPRITE
                renderer.sprite = verticalSprite;
                break;
            case 2:
                //EAST
                //TODO: GET NEW SPRITE
                renderer.sprite = horizontalSprite;
                break;

            case 3:
                //NORTH + EAST
                renderer.sprite = northAndEastCorner;
                break;

            case 4:
                //WEST
                //TODO: GET NEW SPRITE
                renderer.sprite = horizontalSprite;
                break;

            case 5:
                //NORTH + WEST
                renderer.sprite = northAndWestCorner;
                break;

            case 6:
                //WEST + EAST
                renderer.sprite = horizontalSprite;
                break;

            case 7:
                //NORTH + WEST + EAST
                //TODO: GET NEW SPRITE
                renderer.sprite = horizontalSprite;
                break;

            case 8:
                //SOUTH
                //TODO: GET NEW SPRITE
                renderer.sprite = verticalSprite;
                break;

            case 9:
                //NORTH + SOUTH
                renderer.sprite = verticalSprite;
                break;

            case 10:
                //EAST + SOUTH
                renderer.sprite = southAndEastCorner;
                break;

            case 11:
                //NORTH + EAST + SOUTH
                //TODO: GET NEW SPRITE
                renderer.sprite = southAndEastCorner;
                break;

            case 12:
                //WEST + SOUTH
                renderer.sprite = southAndWestCorner;
                break;

            case 13:
                //NORTH + WEST + SOUTH
                //TODO: GET NEW SPRITE
                renderer.sprite = southAndWestCorner;
                break;

            case 14:
                //EAST + WEST + SOUTH
                //TODO: GET NEW SPRITE
                renderer.sprite = southAndWestCorner;
                break;

            case 15:
                //NORTH + EAST + WEST + SOUTH
                renderer.sprite = southAndWestCorner;
                break;

            default:
                //NO DIRECTIONS
                renderer.sprite = defaultSprite;
                break;
        }
    }

    /* 
     * Function uses the directional priority of the Tile to calculate the
     * sprite update value
     * Function returns the calculated short value with a value between 0-15
     */ 
    short getSpriteDirectionValue()
    {
        short spriteUpdateValue = 0;
        if (northPriority > -1)
        {
            spriteUpdateValue += 1;
        }
        if (eastPriority > -1)
        {
            spriteUpdateValue += 2;
        }
        if (westPriority > -1)
        {
            spriteUpdateValue += 4;
        }
        if (southPriority > -1)
        {
            spriteUpdateValue += 8;
        }
        return spriteUpdateValue;
    }

    /// <summary>
    /// Executes on when squibble walks on this tile
    /// </summary>
    public override void OnWalk(Squibble squib)
    {
        //renderer.color = Color.cyan;
        hasSquibble = true;
    }
}
