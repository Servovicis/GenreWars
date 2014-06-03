using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public abstract class GridCS : MonoBehaviour
{
	#region Singleton

	public static GridCS Instance { get; protected set; } 

	#endregion

	#region Types

	//UnitType type = UnitType.UNITKING;

	public enum UnitType
	{
		None,
		King,
		Wall,
		Heavy,
		Melee,
		Ranged,
		Sorcerer
	}

	public enum PlayerNumber
	{
		None,
		Player1,
		Player2,
		Player3,
		Player4
	}

	#endregion


	#region Constants

	//The space taken up by each King tile
	private const int KINGMAXZ = 5;
	private const int KINGMINZ = 4;
	private const int KINGPLAYER1MAXX = 1;
	private const int KINGPLAYER2MINX = 18;

	//Where each player can place units
	private const int PLAYER1PLACEABLEAREA = 4;
	private const int PLAYER2PLACEABLEAREA = 15;

	//Grid Size
	public static int GRIDSIZEX = 20;
	public static int GRIDSIZEZ = 10;
	public static int layerCount = 1;
	public static int coreLayer = 0;

	#endregion

	#region Public Variables

	public Transform tilePrefab;

	#endregion

	#region Private Variables

	//Make the grid at size 20x10
	public Tile[,,] grid;
	public Unit Player1King;
	public Unit Player2King;
	public Unit Player1Wall;
	public Unit Player2Wall;

	protected string[] levelNames;
	public int levelCount = 0;
	public string chosenLevel;

	#endregion

	protected virtual void Awake () {
		Instance = this;
		DontDestroyOnLoad (this);
	}

	protected virtual void Start () {
		FindLevelFiles ();
	}

	public virtual void CreateGrid ()
	{
		Instance = this;
		grid = new Tile[GRIDSIZEX, GRIDSIZEZ,layerCount];
		GameObject GameController = GameObject.Find ("GameControl");
		SwitchButton SwitchScript = GameController.GetComponent <SwitchButton> ();
		//Go down the grid on the X plane, setting up parameters for each tile on the Z plane
		Player Player1Script = GameObject.Find ("Player1(Clone)").GetComponent<Player> ();
		Player Player2Script = GameObject.Find ("Player2(Clone)").GetComponent<Player> ();
		int unitcounter = 0;
		foreach (string unittype in Player1Script.genreScript.UnitTypes){
			if (unittype == "King"){
				Player1King = Player1Script.genreScript.UnitsList[unitcounter].GetComponent<Unit>();
				break;
			}
			else {unitcounter++;}
		}
		unitcounter = 0;
		foreach (string unittype in Player1Script.genreScript.UnitTypes){
			if (unittype == "Wall"){
				Player1Wall = Player1Script.genreScript.UnitsList[unitcounter].GetComponent<Unit>();
				break;
			}
			else {unitcounter++;}
		}
		unitcounter = 0;
		foreach (string unittype in Player2Script.genreScript.UnitTypes){
			if (unittype == "King"){
				Player2King = Player2Script.genreScript.UnitsList[unitcounter].GetComponent<Unit>();
				break;
			}
			else {unitcounter++;}
		}
		unitcounter = 0;
		foreach (string unittype in Player2Script.genreScript.UnitTypes){
			if (unittype == "Wall"){
				Player2Wall = Player2Script.genreScript.UnitsList[unitcounter].GetComponent<Unit>();
				break;
			}
			else {unitcounter++;}
		}
		for (int xposition = 0; xposition < GRIDSIZEX; xposition++)
		{
			for (int zposition = 0; zposition < GRIDSIZEZ; zposition++)
			{
				//Instantiate a tile as a GameObject, pull the Tile component from it, set it to the "tile" variable
				Object tileObject = Instantiate (tilePrefab.gameObject, new Vector3(xposition, 0, zposition), Quaternion.identity);
				GameObject tileGameObject = tileObject as GameObject;
				Tile thisTile = tileGameObject.GetComponent<Tile>();
				//Reset the tile edited counter
				int tileEdited = 0;
				//Set the position in the grid to be the tile class
				grid[xposition,zposition,0] = thisTile;
				thisTile.xcoord = xposition;
				thisTile.zcoord = zposition;
				thisTile.layerNumber.x = 0;
				//The following nested if statement checks if the tile is in a position that needs to be a king or wall. If it does not, then it sets the tile to be an ordinary tile.
				if (xposition <= KINGPLAYER1MAXX){
					if (zposition == KINGMINZ - 1 || zposition == KINGMAXZ + 1){
						InsertUnit (thisTile, Player1Wall, SwitchScript.Player1Script);
						thisTile.type = Tile.Type.Player1Wall;
						tileEdited = 1;
					}
					else if (zposition == KINGMINZ || zposition == KINGMAXZ){
						if (!Player1Script.KingHasSpawned){
							Player1Script.KingHasSpawned = true;
							InsertUnit (thisTile, Player1King, SwitchScript.Player1Script);
						}
						thisTile.LoadedUnitScript = Player1King;
						thisTile.type = Tile.Type.Player1King;
						tileEdited = 1;
					}
				}
				else if (xposition == KINGPLAYER1MAXX + 1){
					if (KINGMINZ - 1 <= zposition && zposition <= KINGMAXZ + 1){
						InsertUnit (thisTile, Player1Wall, SwitchScript.Player1Script);
						thisTile.type = Tile.Type.Player1Wall;
						tileEdited = 1;
					}
				}
				else if (xposition >= KINGPLAYER2MINX){
					if (zposition == KINGMINZ - 1 || zposition == KINGMAXZ + 1){
						InsertUnit (thisTile, Player2Wall, SwitchScript.Player2Script);
						thisTile.type = Tile.Type.Player2Wall;
						tileEdited = 1;
						}
					else if (zposition == KINGMINZ || zposition == KINGMAXZ){
						if (!Player2Script.KingHasSpawned){
							Player2Script.KingHasSpawned = true;
							InsertUnit (thisTile, Player2King, SwitchScript.Player2Script);
						}
						thisTile.LoadedUnitScript = Player2King;
						thisTile.type = Tile.Type.Player2King;
						tileEdited = 1;
					}
				}
				else if (xposition == KINGPLAYER2MINX - 1){
					if (KINGMINZ - 1 <= zposition && zposition <= KINGMAXZ + 1){
						InsertUnit (thisTile, Player2Wall, SwitchScript.Player2Script);
						thisTile.type = Tile.Type.Player2Wall;
						tileEdited = 1;
					}
				}
				if (tileEdited == 0){
					thisTile.unit = UnitType.None;
					thisTile.player = PlayerNumber.None;
					thisTile.tempUnit = UnitType.None;
					thisTile.tempPlayer = PlayerNumber.None;
					if (xposition <= PLAYER1PLACEABLEAREA){
						thisTile.type = Tile.Type.Player1;
					}
					else if (xposition >= PLAYER2PLACEABLEAREA){
						thisTile.type = Tile.Type.Player2;
					}
					else{
						int randomNumber = Random.Range (-1 ,PLAYER2PLACEABLEAREA-PLAYER1PLACEABLEAREA) + (xposition - PLAYER1PLACEABLEAREA);
						if(randomNumber < PLAYER2PLACEABLEAREA - PLAYER1PLACEABLEAREA - 1){
							thisTile.type = Tile.Type.Player1;
						}
						else {
							thisTile.type = Tile.Type.Player2;
						}
					}
				}
				//The if/elif statement sets all tiles within the units placeable area of each player to be placeable for them.
				if(xposition <= PLAYER1PLACEABLEAREA){
					thisTile.playerThatCanPlaceUnits = PlayerNumber.Player1;
				}
				else if (xposition >= PLAYER2PLACEABLEAREA){
					thisTile.playerThatCanPlaceUnits = PlayerNumber.Player2;
				}
			}
		}		
		//SwitchScript.EnableSpawnableArea ();
	}

	public virtual void FindLevelFiles() {
		levelNames = Resources.Load <TextAsset> ("Levels").text.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string LevelName in levelNames) {
			levelCount += 1;
			LevelSelect.Instance.LevelNames.Add(LevelName);
		}
		LevelSelect.Instance.numberOfLevels = levelCount;
	}

	public virtual void SpawnCustomLevel() {
		GameObject gridController = GameObject.Find ("Grid Controller");
		gridController.AddComponent<GridReader> ();
		Destroy (this);
	}

	protected virtual void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	protected virtual void InsertUnit(Tile targetTile, Unit prefab, Player owner){
		Unit ThisUnit = (Instantiate (prefab.gameObject, new Vector3 (targetTile.xcoord +.5F, Tile.UNITLOADDISTANCE, targetTile.zcoord +.5f),
		                             	Quaternion.identity) as GameObject).GetComponent<Unit> ();
		ThisUnit.transform.Rotate(0, owner.UnitFacingAngle, 0);
		ThisUnit.Position = new Vector2(targetTile.xcoord, targetTile.zcoord);
		ThisUnit.layer = (int) targetTile.layerNumber.x;
		targetTile.LoadedUnitScript = ThisUnit;
		ThisUnit.UnitOwner = owner;
		GameManager.Instance.AllUnits.AddLast (ThisUnit);
		owner.UnitList.AddLast (ThisUnit);
	}
}