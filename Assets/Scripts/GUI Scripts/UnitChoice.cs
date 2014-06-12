using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitChoice : MonoBehaviour 
{

	#region Singleton
	
	public static UnitChoice Instance { get; set; } 
	
	#endregion

	public GUISkin customSkin;
	public const int GUIButtonWidth = 185;
	const int GuiWidth = 95;
	public const int GuiHeight = 27;
	public const int P2GuiX = 650;
	public int NumberOfButtons = 4;
	private Rect GUIGroupSize = new Rect(0, 0, 1225, 45);
	public const float UNITSPAWNHEIGHT = 0.05F;

	private GameObject PlayerControl;
	private SwitchButton PlayerControlScript;
	private GameManager GameControllerScript;

	public Player ThisPlayer;

	public GameObject SpawnedUnit;
	public Vector3 SpawnVector;

	public List<Unit> AllSpawnableUnits = new List<Unit> ();
	
	CursorSelection CursorSelectionScript;
	TurnActionOrderHandler actionOrderHandler;

	void Awake()
	{
		Instance = this;
		PlayerControl = GameObject.Find ("GameControl");
		PlayerControlScript = PlayerControl.GetComponent<SwitchButton> ();
		GameControllerScript = PlayerControl.GetComponent<GameManager> ();
		CursorSelectionScript = PlayerControl.GetComponent<CursorSelection> ();
		actionOrderHandler = PlayerControl.GetComponent<TurnActionOrderHandler> ();
	}

	void Start () 
	{
		ThisPlayer = PlayerControlScript.CurrentPlayer;
	}

	public void SpawnUnit()
	{
		Unit InstantiatedObjectScript = SpawnedUnit.GetComponent<Unit> ();
		if (InstantiatedObjectScript.populationCost + ThisPlayer.Population <= ThisPlayer.MaximumPopulation &&
			ThisPlayer.SummoningPoints - InstantiatedObjectScript.summoningCost >= 0)
		{
			InstantiatedObjectScript = ((Instantiate (SpawnedUnit, SpawnVector, Quaternion.identity)) as GameObject).GetComponent<Unit>();
			InstantiatedObjectScript.transform.Rotate(0, ThisPlayer.UnitFacingAngle, 0);
			InstantiatedObjectScript.UnitOwner = ThisPlayer;
			CursorSelectionScript.selectedTile.LoadedUnitScript = InstantiatedObjectScript;
			ThisPlayer.SummoningPoints -= InstantiatedObjectScript.summoningCost;
			ThisPlayer.Population += InstantiatedObjectScript.populationCost;
			float xcoord = SpawnVector.x;
			float ycoord = (int) CursorSelectionScript.selectedTile.layerNumber.x;
			float zcoord = SpawnVector.z;
			GameManager.Instance.AllUnits.AddLast (InstantiatedObjectScript);
			InstantiatedObjectScript.UniversalUnitListNode = GameManager.Instance.AllUnits.Last;
			ThisPlayer.UnitList.AddLast (InstantiatedObjectScript);
			InstantiatedObjectScript.PlayerUnitListNode = ThisPlayer.UnitList.Last;
			ObjectBakeToTile(xcoord, ycoord, zcoord, InstantiatedObjectScript);
			actionOrderHandler.actionList.AddLast (new UnitInsert(new Vector2 (xcoord, zcoord), (int) CursorSelectionScript.selectedTile.layerNumber.x));
		}
	}
	
	public void GUISelectionBoxInsert () 
	{
		int buttoncount = 0;
		int counter = 0;
		foreach (GameObject ThisUnitObject in ThisPlayer.genreScript.UnitsList) 
		{
			Unit ThisUnit = ThisUnitObject.GetComponent<Unit>();
			if (ThisUnit.IsSpawnable){	
				AllSpawnableUnits.Add (ThisUnit);
				GameManager.Instance.LeftPaneButtons[buttoncount].onClick = GameManager.Instance.LeftPaneButtons[buttoncount].SpawnedUnitButton;
				GUILeftPaneButton thisButton = GameManager.Instance.LeftPaneButtons[buttoncount];
				UILabel buttonLabel = thisButton.myLabel;
				NGUITools.SetActive(thisButton.gameObject, true);
				buttonLabel.text = ThisUnit.MyName;
				//if (GUI.Button (new Rect (GUIGroupSize.width*(buttoncount*.20f) + 25f, 0, GUIButtonWidth, GuiHeight), ThisUnit.MyName)) 
				//{
				//	SpawnedUnit = ThisUnitObject;
				//}
				buttoncount++;
			}
			counter++;
			}
		counter = 0;
		buttoncount = 0;
	}

	public void SpawnButton() 
	{

		if (GUI.Button(new Rect(Screen.width * .8f, 225, 150, 27), "Spawn Unit")) 
		{
			if (CursorSelectionScript.selectedTile != null)
			{
				SpawnVector = new Vector3 (CursorSelectionScript.selectedTile.xcoord, CursorSelectionScript.selectedTile.transform.position.y + UNITSPAWNHEIGHT, CursorSelectionScript.selectedTile.zcoord);
				if(CursorSelectionScript.selectedTile.playerThatCanPlaceUnits == ThisPlayer.player)
				{
					if (CursorSelectionScript.selectedTile.unit == GridCS.UnitType.None){
						if (SpawnedUnit != null){
							SpawnUnit();
						}
						else print ("No unit was selected");
					}
					else {
						print("You cannot place units in an occupied tile");
					}
				}
				else{
					print(ThisPlayer.player + " cannot place units here.");
				}
			}
			else {
				print ("No tile selected");
			}
		}
	}
	
	public void ObjectDetachFromTile(float xcoord, float ycoord, float zcoord, GameObject UnitObject)
	{
		Tile SelectedTile = GridCS.Instance.grid[(int) xcoord, (int) ycoord,(int) zcoord];
		SelectedTile.LoadedUnitScript = null;
	}
	
	public void ObjectBakeToTile (float xcoord, float ycoord, float zcoord, Unit UnitObject)
	{
		Tile SelectedTile = GridCS.Instance.grid [(int) xcoord, (int) zcoord,(int) ycoord];
		SelectedTile.LoadedUnitScript = UnitObject;
		Unit UnitScript = UnitObject.GetComponent<Unit> ();
		UnitScript.Position = new Vector3(SelectedTile.xcoord, SelectedTile.zcoord);
		UnitScript.layer = (int) SelectedTile.layerNumber.x;
	}
}