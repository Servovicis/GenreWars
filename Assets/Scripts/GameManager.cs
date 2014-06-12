using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnStates
{
	NullState,
	InsertPhase,
	ResolutionPhase,
	EndPhase
}

public enum MenuStates
{
	NullState,
	OptionsMenu,
	PauseMenu
}

public class GameManager : MonoBehaviour 
{
	
	#region Singleton
	
	public static GameManager Instance { get; set; } //The singleton for this script. It can be called from any other script by typing GameManager.Instance
	//as long as the GameManager has been initialized.
	
	#endregion
	
	#region Delegates
	//delegate type that holds GUI functions, do not put functions here that you don't intend to be handled by the GUI.
	public delegate void GUIFunction();
	public GUIFunction guiFunction; //master GUI delegate, load all GUI functions into this delegate.
	public GUIFunction phaseGUIFunction; //holds the GUI which is being used for the current phase. Mostly used for moving in and out of the pause menu.
	public GUIFunction buttonsGUIFunction; //holds the buttons which are held within the GUISelectionBox function
	public GUISkin customSkin; //Master GUI skin, will load into all GUI functions unless overridden
	public GUIStyle phasesStyle; //The GUI style for most buttons in the game
	public float SelectionBoxSize; //TODO: Remove this
	private Vector2 scrollPosition = Vector2.zero; //TODO: Remove this
	
	//delegate type, for all delegates that exclusively recieve units. Currently unused.
	public delegate void UnitFunction(Unit thisUnit);
	
	//delegate type for delegates that do not recieve parameters.
	public delegate void GenericFunction();
	public GenericFunction OnResolveTransitionInitial; //Enter into the resolve phase transition
	public GenericFunction StackInsertFinalActions; //Items to be inserted into the final slots of the TurnActionOrderHandlers actionStack use this delegate.
	public GenericFunction StackInsertInitialActions; //Items to be inserted into the first slots of the TurnActionOrderHandlers actionStack use this delegate.
	public GenericFunction OnResolveTransitionFinal; //Exit the resolve phase transition
	public GenericFunction OnTurnBegin; //Enter into the beginning of a new turn
	public GenericFunction OnEndPhaseTransition; //Enacts at the beginning of the end phase.
	#endregion
	
	#region Script Declarations
	GUIOptionsMenu optionsMenu; //A variable that contains the GUIOptionsMenu script. Replace with singleton?
	GUIPauseMenu pauseMenu; //A variable that contains the GUIPauseMenu script. Replace with singleton?
	Camera MainCamera; //A variable that holds the camera
	#endregion
	
	#region State Declarations
	public TurnStates turnState; //What state the turn is at
	public MenuStates menuState; // What state the menu is in
	#endregion
	
	#region Other Declarations
	// A LinkedList that holds every unit in the game, used primarily for calculating attack ranges.
	public LinkedList<Unit> AllUnits = new LinkedList<Unit> ();
	
	//A visualization tool for testing ranges.
	public GameObject HeightMarker;
	
	//A list that handles the in-game GUI buttons.
	public GUILeftPaneButton[] LeftPaneButtons;
	#endregion
	
	#region Initialize
	void Awake ()
	{
		Instance = this;
		LeftPaneButtons = new GUILeftPaneButton[4];
	}
	
	void Start ()
	{
		//Find and assign script variables
		pauseMenu = gameObject.GetComponent<GUIPauseMenu> ();
		optionsMenu = gameObject.GetComponent<GUIOptionsMenu> ();
		//Tell the grid to spawn
		GridCS.Instance.CreateGrid ();
		//Set the currently loaded layer to the one designated by the grid script
		LayerSwitcher.Instance.CurrentLayer = GridCS.coreLayer;
		for (int layer = 0; layer <= GridCS.layerCount; layer++) {
			if (layer != GridCS.coreLayer)
				LayerSwitcher.Instance.HideLayer (layer);
		}
		//Go and find the main camera
		MainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		//Assign states, enter into the Insert phase
		turnState = TurnStates.InsertPhase;
		menuState = MenuStates.NullState;
		_EnterInsertPhase();
		//Put the Layer Switcher button into the main GUI
		guiFunction += LayerSwitcher.Instance.GUIFunction;
	}
	#endregion
	
	#region Update
	void Update () 
	{
		// Checking for "Esc" Key Being Pressed
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// Toggles Pause Menu based on which state it is in
			switch (menuState)
			{
			case MenuStates.NullState:
				_GoToPauseMenu();
				break;
			case MenuStates.OptionsMenu:
				_ReturnToPauseMenu();
				break;
			case MenuStates.PauseMenu:
				_ExitMenu();
				break;
			}
		}
	}
	#endregion
	
	#region GUI functions
	//The game GUI handler - this should hold all of the GUI data currently in the game.
	void OnGUI()
	{
		// Set the GUI dimensions and settings
		const int GuiWidth = 95;
		const int GuiHeight = 27;
		GUI.skin = customSkin;
		phasesStyle = "Phases";
		
		//If there's anything to load into the master GUI delegate, do that here
		if (guiFunction != null)
			guiFunction();
		
		//Displays Current Phase of Game
		if (turnState == TurnStates.InsertPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "Unit Placement Phase", phasesStyle);
		}
		
		else if(turnState == TurnStates.ResolutionPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "Resolution Phase", phasesStyle);
		}
		
		else if(turnState == TurnStates.EndPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "The Aftermath", phasesStyle);
		}
	}	
	
	//A small scroll box that appears at the bottom left-hand side of the screen, holding buttons which are loaded into the optionsGUIFunction delegate by other scripts.
	//TODO: Remove this, remove the SelectionBoxSize and scrollPosition variables from the GUI declarations region
	public void GUISelectionBox()
	{
		scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2f - Screen.width *.85f / 2f, Screen.height * .8f, Screen.width * .85f, Screen.height * .1f), 
		                                      scrollPosition, new Rect (0, 0, SelectionBoxSize, 30));
		if (buttonsGUIFunction != null) 
		{
			buttonsGUIFunction ();
		}
		GUI.EndScrollView ();
	}
	#endregion
	
	#region Menu State transitions
	//Go from the null menu state to the pause state
	public void _GoToPauseMenu (){
		guiFunction = pauseMenu.GUIFunction;
		pauseMenu.enabled = true;
		menuState = MenuStates.PauseMenu;
		CursorSelection.Instance.enabled = false;
	}
	
	//Go from the pause state to the options state
	public void _GoToOptionsMenu (){
		optionsMenu.enabled = true;
		pauseMenu.enabled = false;
		guiFunction = optionsMenu.GUIFunction;
		menuState = MenuStates.OptionsMenu;
	}
	
	//options state to pause state
	public void _ReturnToPauseMenu (){
		optionsMenu.enabled = false;
		pauseMenu.enabled = true;
		guiFunction = pauseMenu.GUIFunction;
		menuState = MenuStates.PauseMenu;
	}
	
	//pause state to null state
	public void _ExitMenu(){
		pauseMenu.enabled = false;
		guiFunction = phaseGUIFunction;
		menuState = MenuStates.NullState;
		CursorSelection.Instance.enabled = true;
	}
	#endregion
	
	#region Phase State transitions
	//null or end phase to insert phase
	public void _EnterInsertPhase(){
		//If the OnTurnBegin delegate isn't empty, activate it. Good for effects that happen early in the turn.
		if (OnTurnBegin != null)
			OnTurnBegin ();
		//Clear the UnitChoice list that contains all of the spawnable units in one team.
		UnitChoice.Instance.AllSpawnableUnits.Clear();
		//Set the camera to perspective view.
		CameraPerspective ();
		//set the cursor selection script to its ordinary function
		CursorSelection.Instance.mouseFunction = CursorSelection.Instance.InsertMouseFunction;
		//Show which spaces can be spawned in for this turn
		SwitchButton.Instance.EnableSpawnableArea ();
		//Set the phase state
		turnState = TurnStates.InsertPhase;
		//TODO: Remove this
		SelectionBoxSize = UnitChoice.Instance.NumberOfButtons * 200 + 50;
		//Insert all of the GUI buttons. TODO: Move this stuff to NGUI
		guiFunction += TurnActionOrderHandler.Instance.UndoGUI;
		guiFunction += GUISelectionBox;
		guiFunction += SwitchButton.Instance.GUIPlayerStats;
		guiFunction += UnitChoice.Instance.SpawnButton;
		phaseGUIFunction = guiFunction;
		UnitChoice.Instance.GUISelectionBoxInsert ();
	}
	
	public void _EnterResolvePhase() {
		//If there's anything loaded into the OnResolveTransitionInitial delegate, run it. This is useful for loading animations?
		if (OnResolveTransitionInitial != null)
			OnResolveTransitionInitial ();
		//Disable the spawn indicator
		SwitchButton.Instance.DisableSpawnableArea ();
		//Drop cursor selection functionality
		CursorSelection.Instance.mouseFunction = null;
		//Make a temp tile slot, fill it with the currently-selected tile. If such a tile exists, deselect it.
		Tile TileScript;
		TileScript = CursorSelection.Instance.selectedTile;
		if (TileScript != null) {
			if (TileScript.LoadedUnitScript != null)
				TileScript.LoadedUnitScript.OnActionDeselect ();
			if (TileScript != null)
				TileScript.TileSelectionType = Tile.OverlayType.Unselected;
		}
		CursorSelection.Instance.selectedTile = null;
		//If there was a tile that had a mouseover, then unload the mouseover as well.
		if (CursorSelection.Instance.tileMouseOverScript != null)
			CursorSelection.Instance.tileMouseOverScript.IsMouseOver = false;
		//Set the turn state
		turnState = TurnStates.ResolutionPhase;
		//Drop the buttons out of the buttonsGUIFunction, which loads into the GUI master
		buttonsGUIFunction = null;
		//Drop the layer, undo, scrollbox, and player stats from the master GUI.
		guiFunction -= LayerSwitcher.Instance.GUIFunction;
		guiFunction -= TurnActionOrderHandler.Instance.UndoGUI;
		guiFunction -= GUISelectionBox;
		guiFunction -= SwitchButton.Instance.GUIPlayerStats;
		//If there are any functions to drop actions into the front or the back of the GUI stack, resolve them here.
		if (StackInsertFinalActions != null)
			StackInsertFinalActions ();
		if (StackInsertInitialActions != null)
			StackInsertInitialActions ();
		//Resolve and empty the Action list.
		TurnActionOrderHandler.Instance.ResolveActions ();
		TurnActionOrderHandler.Instance.InitializeList ();
		//If there's anything that should happen after the action stack has resolved and emptied, it should happen here.
		if (OnResolveTransitionFinal != null)
			OnResolveTransitionFinal ();
	}
	
	public void _EnterEndPhase(){
		//Add in the layer buttons for viewing.
		guiFunction += LayerSwitcher.Instance.GUIFunction;
		//Change the phase
		turnState = TurnStates.EndPhase;
		//If anything happens at the beginning of the End phase, that should happen here.
		if (OnEndPhaseTransition != null)
			OnEndPhaseTransition ();
	}
	#endregion
	
	#region Camera Controls
	//Set the camera to be orthographic and view the screen from the top
	void CameraOrtho (){
		MainCamera.orthographic = true;
		MainCamera.transform.parent = OrthoCamera.Instance.transform;
		MainCamera.transform.position = OrthoCamera.Instance.transform.position;
		MainCamera.transform.rotation = OrthoCamera.Instance.transform.rotation;
	}
	
	//Set the camera to be perspective
	void CameraPerspective () {
		MainCamera.orthographic = false;
		MainCamera.transform.parent = PerspectiveCamera.Instance.transform;
		MainCamera.transform.position = PerspectiveCamera.Instance.transform.position;
		MainCamera.transform.rotation = PerspectiveCamera.Instance.transform.rotation;
	}
	#endregion
}