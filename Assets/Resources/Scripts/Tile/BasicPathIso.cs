using UnityEngine;
using System.Collections;

/*
 * Basic Path Iso
 * CURRENTLY DEPRECATED DUE TO NO NEED OF FLIPPING TILES
 * This is a basic Path for an Isometric Grid view that has basic(default) variable values and sprites
 * This class is a child of Tile class
 * 
 * @author Steven Roberts
 * @author Nenad Bulicic
 */

public class BasicPathIso : Tile {

    // Variables for the sprite and renderer
    private Sprite defaultSprite;
    private new SpriteRenderer renderer;
    private Color baseColor;

    // Set of sprite variables corresponding to varius path directions
    public Sprite straightPath;
    public Sprite deadEndTopOpen;
    public Sprite deadEndBottomOpen;
    public Sprite cornerTopOpen;
    public Sprite cornerBottomOpen;
    public Sprite cornerSide;
    public Sprite threewayTopOpen;
    public Sprite threewayBottomOpen;
    public Sprite fourWay;


    /*
     * Used for initialization
     */
    protected void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        defaultSprite = renderer.sprite;
        baseColor = renderer.color;
    }

    /*
     * Update is called once per frame
     */
    protected void Update()
    {
        // Sets to corresponding sprite based on current directional input values;        
        if (occupied)
        {
            renderer.color = Color.blue;
        }
        else
        {
            renderer.color = baseColor;
        }
        setSprite(getSpriteDirectionValue());

    }

    /* 
     * Sets the corresponding directional sprite that matches the case value
     * Case values are between 0-15
     * Sprites are flipped so that asset re-use is more feasible
     */
    void setSprite(int spriteDirectionValue)
    {
        switch (spriteDirectionValue)
        {
            case 1:
                //NORTH
                renderer.sprite = deadEndTopOpen;
                renderer.flipX = false;
                renderer.flipY = false;
                break;
            case 2:
                //EAST
                renderer.sprite = deadEndBottomOpen;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            case 3:
                //NORTH + EAST
                renderer.sprite = cornerSide;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            case 4:
                //WEST
                renderer.sprite = deadEndTopOpen;
                renderer.flipX = true;
                renderer.flipY = false;
                break;

            case 5:
                //NORTH + WEST
                renderer.sprite = cornerTopOpen;
                renderer.flipX = false;
                renderer.flipY = true;
                break;

            case 6:
                //WEST + EAST
                renderer.sprite = straightPath;
                renderer.flipX = false;
                renderer.flipY = true;
                break;

            case 7:
                //NORTH + WEST + EAST
                renderer.sprite = threewayTopOpen;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            case 8:
                //SOUTH
                renderer.sprite = deadEndBottomOpen;
                renderer.flipX = true;
                renderer.flipY = false;
                break;

            case 9:
                //NORTH + SOUTH
                renderer.sprite = straightPath;
                renderer.flipX = true;
                renderer.flipY = true;
                break;

            case 10:
                //EAST + SOUTH
                renderer.sprite = cornerBottomOpen;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            case 11:
                //NORTH + EAST + SOUTH
                renderer.sprite = threewayBottomOpen;
                renderer.flipX = true;
                renderer.flipY = false;
                break;

            case 12:
                //WEST + SOUTH
                renderer.sprite = cornerSide;
                renderer.flipX = true;
                renderer.flipY = false;
                break;

            case 13:
                //NORTH + WEST + SOUTH
                renderer.sprite = threewayTopOpen;
                renderer.flipX = true;
                renderer.flipY = false;
                break;

            case 14:
                //EAST + WEST + SOUTH
                renderer.sprite = threewayBottomOpen;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            case 15:
                //NORTH + EAST + WEST + SOUTH
                renderer.sprite = fourWay;
                renderer.flipX = false;
                renderer.flipY = false;
                break;

            default:
                //NO DIRECTIONS
                renderer.sprite = defaultSprite;
                renderer.flipX = false;
                renderer.flipY = false;
                renderer.color = Color.black;
                break;
        }
    }

    /* 
     * Function uses the directional priority of the Tile to calculate the sprite update value
     * A path will lead in a direction based on this function if it has a non-negative directional priority in that direction
     * Function returns the calculated short value with a value between 0-15, and will map to the Cases in setSprite()
     * Number returned based on Bit-wise interpretation of the short value with the following mapping: SWEN
     */
    short getSpriteDirectionValue()
    {
        short spriteUpdateValue = 0;
        
        // 000N
        if (northPriority > -1)
        {
            spriteUpdateValue += 1;
        }

        // 00E0
        if (eastPriority > -1)
        {
            spriteUpdateValue += 2;
        }

        // 0W00
        if (westPriority > -1)
        {
            spriteUpdateValue += 4;
        }

        // S000
        if (southPriority > -1)
        {
            spriteUpdateValue += 8;
        }
        return spriteUpdateValue;
    }
}
