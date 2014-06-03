using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnStates
{
	NullState,
	InsertPhase,
	ActionPhase,
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
	
	public static GameManager Instance { get; set; }
	
	#endregion

	#region GUI declarations
	//The delegates which hold the GUI functions.
	public delegate void GUIFunction(); //delegate type
	public GUIFunction guiFunction; //master delegate
	public GUIFunction phaseGUIFunction; //holds the GUI which is being used for the current phase. Mostly used for moving in and out of the pause menu.
	public GUIFunction buttonsGUIFunction; //holds the buttons which are held within the GUISelectionBox function
	//public GUIFunction insertGUIFunction; //GUI buttons for the Insert phase; currently disabled
	//public GUIFunction actionGUIFunction;//GUI buttons for the Action phase; currently disabled
	public GUISkin customSkin; //Master GUI skin, will load into all GUI functions unless overridden
	public GUIStyle phasesStyle;
	public float SelectionBoxSize;
	private Vector2 scrollPosition = Vector2.zero;
	#endregion

	#region Delegates
	public delegate void UnitFunction(Unit thisUnit); //delegate type

	public delegate void GenericFunction(); //delegate type
	public GenericFunction OnResolveTransitionInitial; //Enter into the resolve phase transition
	public GenericFunction StackInsertFinalActions; //Items to be inserted into the final slots of the TurnActionOrderHandlers actionStack use this delegate.
	public GenericFunction StackInsertInitialActions; //Items to be inserted into the first slots of the TurnActionOrderHandlers actionStack use this delegate.
	public GenericFunction OnResolveTransitionFinal; //Exit the resolve phase transition
	public GenericFunction OnTurnBegin; //Enter into the beginning of a new turn
	public GenericFunction OnEndPhaseTransition; //Enacts at the beginning of the end phase.
	#endregion

	#region Script Declarations
	GUIOptionsMenu optionsMenu;
	GUIPauseMenu pauseMenu;
	UnitChoice unitChoice;
	SwitchButton switchButton;
	CursorSelection cursorSelectionScript;
	TurnActionOrderHandler actionOrderHandler;
	Camera MainCamera;
	GridCS gridScript;
	public GameObject HeightMarker;
	#endregion

	#region State Declarations
	public TurnStates turnState;
	public MenuStates menuState;
	#endregion

	#region Other Declarations
	public LinkedList<Unit> AllUnits = new LinkedList<Unit> ();
	#endregion

	#region Initialize
	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		//Find and assign script variables
		pauseMenu = gameObject.GetComponent<GUIPauseMenu> ();
		unitChoice = gameObject.GetComponent<UnitChoice> ();
		switchButton = gameObject.GetComponent<SwitchButton> ();
		optionsMenu = gameObject.GetComponent<GUIOptionsMenu> ();
		cursorSelectionScript = gameObject.GetComponent<CursorSelection> ();
		actionOrderHandler = gameObject.GetComponent<TurnActionOrderHandler> ();
		gridScript = (GameObject.Find ("Grid Controller")).GetComponent<GridCS> ();
		gridScript.CreateGrid ();
		LayerSwitcher.Instance.CurrentLayer = GridCS.coreLayer;
		for (int layer = 0; layer <= GridCS.layerCount; layer++) {
			if (layer != GridCS.coreLayer)
				LayerSwitcher.Instance.HideLayer (layer);
		}
		//Assign states, enter into the Insert phase
		turnState = TurnStates.InsertPhase;
		menuState = MenuStates.NullState;
		MainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		_EnterInsertPhase();
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
		const int GuiWidth = 95;
		const int GuiHeight = 27;
		GUI.skin = customSkin;
		phasesStyle = "Phases";

		if (guiFunction != null)
			guiFunction();

		//Displays Current Phase of Game
		if (turnState == TurnStates.InsertPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "Unit Placement Phase", phasesStyle);
		}
		
		else if(turnState == TurnStates.ActionPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "Unit Attack Phase", phasesStyle);
		}
		
		else if(turnState == TurnStates.ResolutionPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "Resolution Phase", phasesStyle);
		}
		
		else if(turnState == TurnStates.EndPhase){
			GUI.Label(new Rect( Screen.width * .47f - GuiWidth / 2f, Screen.height * .02f, 250f, GuiHeight), "The Aftermath", phasesStyle);
		}
	}	

	//A small scroll box that appears at the bottom left-hand side of the screen, holding buttons which are loaded into the optionsGUIFunction delegate by other scripts.
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
	public void _GoToPauseMenu (){
		guiFunction = pauseMenu.GUIFunction;
		pauseMenu.enabled = true;
		menuState = MenuStates.PauseMenu;
		cursorSelectionScript.enabled = false;
	}

	public void _GoToOptionsMenu (){
		optionsMenu.enabled = true;
		pauseMenu.enabled = false;
		guiFunction = optionsMenu.GUIFunction;
		menuState = MenuStates.OptionsMenu;
	}

	public void _ReturnToPauseMenu (){
		optionsMenu.enabled = false;
		pauseMenu.enabled = true;
		guiFunction = pauseMenu.GUIFunction;
		menuState = MenuStates.PauseMenu;
	}

	public void _ExitMenu(){
		pauseMenu.enabled = false;
		guiFunction = phaseGUIFunction;
		menuState = MenuStates.NullState;
		cursorSelectionScript.enabled = true;
	}
	#endregion

	#region Phase State transitions
	public void _EnterInsertPhase(){
		if (OnTurnBegin != null)
			OnTurnBegin ();
		CameraPerspective ();
		cursorSelectionScript.mouseFunction = cursorSelectionScript.InsertMouseFunction;
		switchButton.EnableSpawnableArea ();
		turnState = TurnStates.InsertPhase;
		SelectionBoxSize = unitChoice.NumberOfButtons * 200 + 50;
		buttonsGUIFunction += unitChoice.GUISelectionBoxInsert;
		guiFunction += actionOrderHandler.UndoGUI;
		guiFunction -= switchButton.GUIFunction;
		guiFunction += GUISelectionBox;
		guiFunction += switchButton.GUIPlayerStats;
		guiFunction += unitChoice.SpawnButton;
		guiFunction += unitChoice.GUI_EnterActionPhase;
		phaseGUIFunction = guiFunction;
	}

	public void _EnterActionPhase(){
		switchButton.DisableSpawnableArea ();
		turnState = TurnStates.ActionPhase;
		unitChoice.SpawnedUnit = null;
		buttonsGUIFunction = null;
		guiFunction -= unitChoice.SpawnButton;
		guiFunction -= unitChoice.GUI_EnterActionPhase;
		guiFunction += unitChoice.GUI_EnterResolvePhase;
		phaseGUIFunction = guiFunction;
		actionOrderHandler.InitializeList ();
		if (CursorSelection.Instance.selectedTile != null) {
			if (CursorSelection.Instance.selectedTile.LoadedUnitScript != null) 
				CursorSelection.Instance.selectedTile.LoadedUnitScript.OnActionSelect ();
		}
	}

	public void _EnterResolvePhase() {
		if (OnResolveTransitionInitial != null)
			OnResolveTransitionInitial ();
		CameraPerspective ();
		cursorSelectionScript.mouseFunction = null;
		Tile TileScript;
		TileScript = cursorSelectionScript.selectedTile;
		if (TileScript != null) {
			if (TileScript.LoadedUnitScript != null)
				TileScript.LoadedUnitScript.OnActionDeselect ();
			if (TileScript != null)
				TileScript.TileSelectionType = Tile.OverlayType.Unselected;
		}
		cursorSelectionScript.selectedTile = null;
		if (cursorSelectionScript.tileMouseOverScript != null)
			cursorSelectionScript.tileMouseOverScript.IsMouseOver = false;
		turnState = TurnStates.ResolutionPhase;
		buttonsGUIFunction = null;
		guiFunction -= LayerSwitcher.Instance.GUIFunction;
		guiFunction -= actionOrderHandler.UndoGUI;
		guiFunction -= GUISelectionBox;
		guiFunction -= switchButton.GUIPlayerStats;
		guiFunction -= unitChoice.GUI_EnterResolvePhase;
		guiFunction += unitChoice.GUItemp_EnterEndPhase;
		if (StackInsertFinalActions != null)
			StackInsertFinalActions ();
		if (StackInsertInitialActions != null)
			StackInsertInitialActions ();
		actionOrderHandler.ResolveActions ();
		actionOrderHandler.InitializeList ();
		if (OnResolveTransitionFinal != null)
			OnResolveTransitionFinal ();
	}

	public void _EnterEndPhase(){
		phaseGUIFunction = switchButton.GUIFunction;
		guiFunction += LayerSwitcher.Instance.GUIFunction;
		turnState = TurnStates.EndPhase;
		guiFunction -= unitChoice.GUItemp_EnterEndPhase;
		guiFunction += switchButton.GUIFunction;
		if (OnEndPhaseTransition != null)
			OnEndPhaseTransition ();
	}
	#endregion

	#region Camera Controls
	void CameraOrtho (){
		MainCamera.orthographic = true;
		MainCamera.transform.parent = OrthoCamera.Instance.transform;
		MainCamera.transform.position = OrthoCamera.Instance.transform.position;
		MainCamera.transform.rotation = OrthoCamera.Instance.transform.rotation;
	}

	void CameraPerspective () {
		MainCamera.orthographic = false;
		MainCamera.transform.parent = PerspectiveCamera.Instance.transform;
		MainCamera.transform.position = PerspectiveCamera.Instance.transform.position;
		MainCamera.transform.rotation = PerspectiveCamera.Instance.transform.rotation;
	}
	#endregion
}