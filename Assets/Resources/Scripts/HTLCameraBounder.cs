using UnityEngine;
using System.Collections;
using HWTools.Grid;

/// <summary>
/// Controls HTL Camera Movement based on mouse position and restricts camera movement such that it can't go outside the grid area.
/// </summary>
public class HTLCameraBounder : MonoBehaviour
{
    // Points in Grid Space which represent the furthest points the camera can travel to in each direction
    Vector2 cameraBoundNorth;
    Vector2 cameraBoundSouth;
    Vector2 cameraBoundEast;
    Vector2 cameraBoundWest;

    //Smooth Movement
    public float cameraSmoothMoveTime = 0.3f;
    public float cameraMoveMaxSpeed = 0.1f;
    private Vector3 velocity = Vector3.zero;

    // Grid References
    private Grid2D grid;
    private Grid2DCollection collect;

    // Distance between furthest points on the grid
    private float nsDist;
    private float ewDist;

	// Use this for initialization
	void Start ()
    {
        //Establish Connection to Grid and initial position
        grid = GameObject.Find("Grid").GetComponent<Grid2D>();
        collect = GameObject.Find("Grid").GetComponent<Grid2DCollection>();
        this.transform.position = new Vector3(0f, 0f, -50f);

        // Get tile Grid Positions
        cameraBoundNorth = collect.getNorthernmostTilePos();
        cameraBoundSouth = collect.getSouthernmostTilePos();
        cameraBoundEast = collect.getEasternmostTilePos();
        cameraBoundWest = collect.getWesternmostTilePos();
        
        //Adjust the Camera Bounds to the tip of the furthest tiles in Grid Space
        cameraBoundNorth += new Vector2(0.5f, 0.5f);
        cameraBoundSouth += new Vector2(-0.5f, -0.5f);
        cameraBoundEast += new Vector2(0.5f, -0.5f);
        cameraBoundWest += new Vector2(-0.5f, 0.5f);

        //Transition all camera bounds to World Space
        cameraBoundNorth = grid.Projection2D(cameraBoundNorth);
        cameraBoundSouth = grid.Projection2D(cameraBoundSouth);
        cameraBoundEast = grid.Projection2D(cameraBoundEast);
        cameraBoundWest = grid.Projection2D(cameraBoundWest);

        //Determine the distance between the furthest north and south Y, and furthest East and West X in World Space.
        nsDist = cameraBoundNorth.y - cameraBoundSouth.y;
        ewDist = cameraBoundEast.x - cameraBoundWest.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Component dimensions of Camera
        var camHeight = 2 * Camera.main.orthographicSize;
        var camWidth = camHeight * Camera.main.aspect;

        var camBorder = 0.2f * Camera.main.pixelHeight;

        // Get Mouse Location, then determine if a camera move is necessary.
        // If not outside camBorder, do not bother moving
        Vector3 mousePoint = Input.mousePosition;
        if (mousePoint.x < camBorder ||
            mousePoint.x > (Camera.main.pixelWidth - camBorder) ||
            mousePoint.y < camBorder ||
            mousePoint.y > (Camera.main.pixelHeight - camBorder)  
            )
        {
            //Outside of range to Move, do a move
            Vector3 moveTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position =
                Vector3.SmoothDamp(Camera.main.transform.position, moveTarget, ref velocity, cameraSmoothMoveTime, cameraMoveMaxSpeed);

            Vector3 camMin = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 camMax = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f));

            // Snap Camera based on bound
            if (nsDist > camHeight)
            {
                if (camMax.y > cameraBoundNorth.y)
                {
                    Camera.main.transform.Translate(0f, (cameraBoundNorth.y - camMax.y), 0f, Space.World);
                }
                else if (camMin.y < cameraBoundSouth.y)
                {
                    Camera.main.transform.Translate(0f, (cameraBoundSouth.y - camMin.y), 0f, Space.World);
                }
            }
            if (ewDist > camWidth)
            {
                if (camMax.x > cameraBoundEast.x)
                {
                    Camera.main.transform.Translate((cameraBoundEast.x - camMax.x), 0f, 0f, Space.World);
                }
                else if (camMin.x < cameraBoundWest.x)
                {
                    Camera.main.transform.Translate((cameraBoundWest.x - camMin.x), 0f, 0f, Space.World);
                }
            }
        }  
    }
}