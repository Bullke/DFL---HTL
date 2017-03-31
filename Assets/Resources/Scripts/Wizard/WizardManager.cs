using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using HWTools.Grid;
using HTLWizards;

/// <summary>
/// Class that manages the placement and movement of Wizards about the HTL Grid, as well as various UI Elements Including:
/// - Wizard Spawner Counter
/// - Moves Counter
/// - Game Timer
/// </summary>
public class WizardManager : MonoBehaviour
{
    #region Fields
    #region Move Counter Fields
    /// <summary>
    /// Limit to number of times Wizards can be moved per game
    /// </summary>
    public int movesLimit = -1;
    /// <summary>
    /// UI Element for the moves counter
    /// </summary>
    public Text movesCounterText;
    private int movesCounter;


    #endregion

    // Containers for data regarding wizards for the Manager, (TODO) can be loaded from file. 
    // Each Contains a Wizard, a Count, and an associated Text Object in the UI. See the Bottom of this class.
    public WizQueue[] wizards;
    
    private SpriteRenderer wizPreview;
    private Animator wizAnimator;
    private GridTransform gridTrans;
    private Grid2DCollection gridCollect;

    // The wizard last selected by the Wizard Manager.
    private GameObject selectWiz;

    private Dictionary<string, Queue<GameObject>> wizardQueues;
    #endregion

    enum WizManState { Neutral, NewWizard, WizardSelected };
    private WizManState state;

    #region Properties
    /// <summary>
    /// Count of Wizard Moves done in this game session. A wizard Move is counted when a wizard is moved from one position on the grid to another.
    /// </summary>
    public int Moves
    {
        get
        {
            return movesCounter;
        }
    }

    /// <summary>
    /// The wizard currently being manipulated by the WizardManager. Assumes that Animator already attached to the Wizard Object
    /// </summary>
    private GameObject SelectedWizard
    {
        get { return selectWiz; }
        set
        {
            selectWiz = value;
            wizAnimator.runtimeAnimatorController = SelectedWizard.GetComponentInChildren<Animator>().runtimeAnimatorController;
            Animator animComponent;
            //TODO: Figure out how to deal with the Warnings that come up from this
            if (animComponent = selectWiz.GetComponentInChildren<Animator>())
            {
                animComponent.SetBool("selected", true);
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Button Interaction Class to be called by Wizard-Spawning Buttons
    /// </summary>
    /// <param name="buttonWiz">Wizard corresponding to the Button</param>
    public void buttonClick(GameObject buttonWiz)
    {
        switch (state)
        {
            //Normal Gameplay. Begin Placing a new Wizard
            case WizManState.Neutral:
                if (wizardQueues[buttonWiz.name].Count > 0)
                {
                    SelectedWizard = wizardQueues[buttonWiz.name].Dequeue();
                    state = WizManState.NewWizard;
                }
                break;

            // Placing a new Wizard. Stop placing a Wizard.
            case WizManState.NewWizard:
                SelectedWizard.SetActive(false);
                wizardQueues[SelectedWizard.name].Enqueue(SelectedWizard);
                state = WizManState.Neutral;
                break;

            // Wizard Moving. Return the Wizard to the Queue
            case WizManState.WizardSelected:
                //Get the Wizard off the grid
                SelectedWizard.SetActive(false);
                wizardQueues[SelectedWizard.name].Enqueue(SelectedWizard);
                state = WizManState.Neutral;
                break;
        }
    }
    #endregion

    #region Unity Runtime Methods
    /// <summary>
    /// Initialize Wizard Dictionary, Instantiate Wizard Objects
    /// </summary>
    void Start()
    {
        //Create Wizards and instantiate dictionary based on entries from Unity Inspector (or file TODO)
        wizardQueues = new Dictionary<string, Queue<GameObject>>();
        foreach (var wizData in wizards)
        {
            wizardQueues[wizData.wizardPrefab.name] = new Queue<GameObject>(wizData.count);
            //instantiate number of wizards specified
            for (int curWiz = 0; curWiz < wizData.count; curWiz++)
            {
                GameObject tempObject = (GameObject.Instantiate(wizData.wizardPrefab));
                //trim (clone) so that object can be recognized by Queues
                tempObject.name = tempObject.name.Replace("(Clone)", "");
                tempObject.SetActive(false);
                wizardQueues[wizData.wizardPrefab.name].Enqueue(tempObject);
            }
        }

        wizPreview = this.GetComponentInChildren<SpriteRenderer>();
        wizAnimator = this.GetComponentInChildren<Animator>();
        gridTrans = this.GetComponent<GridTransform>();
        gridCollect = gridTrans.parent.GetComponent<Grid2DCollection>();

        movesCounter = 0;

    }

    /// <summary>
    /// Handle Input, Mouse clicks, Raycasting to select Wizards, update related UI
    /// </summary>
    void Update()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gridTrans.GridPosition = (Vector3)gridTrans.parent.SelectTile(mousePoint);

        switch (state)
        {
            //Neutral State. Detect if clicking on a Wizard.
            case WizManState.Neutral:
                wizPreview.color = Color.clear;
                //on click, try to select a wizard. In event of an overlap, Raycast grabs the Wizard that is in front at the point of click.
                //If Move Limit reached, do not select. Do not consider Move Limit if Move Limit is less than 0.
                if (((movesLimit < 0) || (movesCounter < movesLimit)) && Input.GetMouseButtonUp(0))
                {
                    RaycastHit2D raycast = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Camera.main.transform.forward);
                    if (raycast.collider != null)
                    {
                        Debug.Log("Found a Thing");
                        SelectedWizard = raycast.collider.gameObject.GetComponentInParent<Wizard>().gameObject;
                        state = WizManState.WizardSelected;
                    }
                }
                break;

            // Creating a new Wizard
            case WizManState.NewWizard:
                //check if there is a tile at this location
                var theTileObj = gridCollect[gridTrans.GridPosition];
                if (theTileObj)
                {
                    // Check if invalid object placement location. If invalid, color red, do not accept input
                    if (theTileObj.GetComponent<Tile>().isAPath || theTileObj.GetComponent<Tile>().occupied)
                    {
                        wizPreview.color = new Color(1f, 0f, 0f, 0.5f);
                    }
                    else
                    {
                        wizPreview.color = new Color(1f, 1f, 1f, 0.5f);
                        // On left click
                        if (Input.GetMouseButtonUp(0))
                        {
                            placeSelectedWizard(theTileObj);
                        }
                    }
                }
                break;

            //A wizard on the map was selected via a click
            case WizManState.WizardSelected:
                theTileObj = gridCollect[gridTrans.GridPosition];

                //check if there is a tile at this location
                if (theTileObj)
                {
                    // Check if selecting same wizard
                    if (theTileObj.GetComponent<GridTransform>().GridPosition == SelectedWizard.GetComponent<GridTransform>().GridPosition)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            SelectedWizard.GetComponentInChildren<Animator>().SetBool("selected", false);
                            //SelectedWizard = null;
                            state = WizManState.Neutral;
                        }
                    }
                    // Check if valid wizard placement location
                    else if (theTileObj.GetComponent<Tile>().isAPath || theTileObj.GetComponent<Tile>().occupied)
                    {
                        wizPreview.color = new Color(1f, 0f, 0f, 0.5f);
                    }
                    // Current location is a valid placement for the wizard
                    else
                    {
                        wizPreview.color = new Color(1f, 1f, 1f, 0.5f);
                        // on left click
                        if (Input.GetMouseButtonUp(0))
                        {
                            placeSelectedWizard(theTileObj);
                            movesCounter++;
                        }
                    }
                }
                break;
        }

        // Display to current wizard count to all buttons
        foreach (var wiz in wizards)
        {
            if (wiz.buttonText)
            {
                wiz.buttonText.text = "" + wizardQueues[wiz.wizardPrefab.name].Count;
            }
            else
            {
                Debug.LogError("Wizard Button Text object unassigned in Wizard Manager!");
            }
        }

        // Update Moves Counter UI if it exists
        if (movesCounterText)
        {
            // If moves limit turned off ( is <0 ) display number of moves performed
            if (movesLimit < 0)
            {
                movesCounterText.text = "" + movesCounter;
            }
            // Else display number of moves remaining to the user
            else
            {
                movesCounterText.text = "" + (movesLimit - movesCounter);
            }
        }
    }
    #endregion

    #region Private Methods
    //Section of repeated code used when placing a wizard
    private void placeSelectedWizard(GameObject theTileObj)
    {
        SelectedWizard.GetComponent<GridTransform>().SetReference(FindObjectOfType<Grid2D>());
        SelectedWizard.GetComponent<GridTransform>().GridPosition = theTileObj.GetComponent<GridTransform>().GridPosition;
        SelectedWizard.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        SelectedWizard.gameObject.SetActive(true);
        SelectedWizard.GetComponentInChildren<Animator>().SetBool("selected", false);
        state = WizManState.Neutral;
    }
    #endregion

    /// <summary>
    /// Container class for constructing Wizard Queues in the Wizard Manager.
    /// </summary>
    [Serializable]
    public class WizQueue
    {
        public GameObject wizardPrefab;
        public int count;
        public Text buttonText;
    }
}


