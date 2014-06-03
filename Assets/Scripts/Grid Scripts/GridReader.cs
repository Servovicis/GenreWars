using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class GridReader : GridCS {

	bool IsReadingInfo = false;
	public int PlayerCount = 2;
	string[] MapTo;
	int IterationsPerLevel = 0;
	string[] LevelCreateOrder;
	int CurrentIterations = 0;
	int currentLevelLayer = 0;

	// Use this for initialization
	protected override void Awake () {
		Instance = this;
		DontDestroyOnLoad (this);
	}

	protected override void Start () {

	}
	
	// Update is called once per frame
	public override void CreateGrid () {
		Instance = this;
		tilePrefab = Resources.Load<Transform> ("Tile");
		string[] SelectedLevel = Resources.Load<TextAsset> ("Levels/"+chosenLevel).text.Replace("\r", "").Replace("\n", "").Split (new char[] {'|'}, System.StringSplitOptions.RemoveEmptyEntries);
		int xcounter = 0;
		int zcounter = 0;
		string[] levelX;
		string[] levelZ;
		Tile thisTile;
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
		foreach (string LevelData in SelectedLevel) {
			bool KeepGoing = true;
			if (LevelData.Contains ("Level Properties:")){
				IsReadingInfo = true;
				KeepGoing = false;
			}
			else if (LevelData.Contains ("End Level Properties")){
				IsReadingInfo = false;
				grid = new Tile[GRIDSIZEX,GRIDSIZEZ,layerCount + 1];
				KeepGoing = false;
			}
			if (KeepGoing){
				if (IsReadingInfo){
					ReadFileInfo (LevelData);
				}
				else {
					switch (LevelCreateOrder[CurrentIterations]){
					case "type":
						xcounter = 0;
						zcounter = 0;
						levelX = LevelData.Split (new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
						foreach (string zRow in levelX){
							levelZ = zRow.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (string tileUnit in levelZ){
								if (tileUnit != "x" && zcounter < GRIDSIZEZ && xcounter < GRIDSIZEX){
									if (grid[xcounter,zcounter,currentLevelLayer] == null){
										Transform aTileCast = Instantiate (tilePrefab, new Vector3 (xcounter, 0, zcounter), Quaternion.identity) as Transform;
										GameObject aTile = aTileCast.gameObject;
										Tile aTileScript = aTile.GetComponent <Tile> ();
										grid[xcounter,zcounter,currentLevelLayer] = aTileScript;
										aTileScript.xcoord = xcounter;
										aTileScript.zcoord = zcounter;
										aTileScript.layerNumber.x = currentLevelLayer;
										aTileScript.layerNumber.y = currentLevelLayer;
									}
									thisTile = grid[xcounter,zcounter,currentLevelLayer];
									switch (tileUnit){
									case "1":
										thisTile.type = Tile.Type.Player1;
										break;
									case "2":
										thisTile.type = Tile.Type.Player2;
										break;
									case "1k":
										thisTile.type = Tile.Type.Player1King;
										if (!Player1Script.KingHasSpawned){
											Player1Script.KingHasSpawned = true;
											InsertUnit (thisTile, Player1King, SwitchScript.Player1Script);
										}
										thisTile.LoadedUnitScript = Player1King;
										thisTile.type = Tile.Type.Player1King;
										break;
									case "1w":
										InsertUnit (thisTile, Player1Wall, SwitchScript.Player1Script);
										thisTile.type = Tile.Type.Player1Wall;
										break;
									case "2k":
										if (!Player2Script.KingHasSpawned){
											Player2Script.KingHasSpawned = true;
											InsertUnit (thisTile, Player2King, SwitchScript.Player2Script);
										}
										thisTile.LoadedUnitScript = Player2King;
										thisTile.type = Tile.Type.Player2King;
										break;
									case "2w":
										InsertUnit (thisTile, Player2Wall, SwitchScript.Player2Script);
										thisTile.type = Tile.Type.Player2Wall;
										break;
									case "p":
										int randomNumber = UnityEngine.Random.Range (0,2);
										if(randomNumber > 0){
											thisTile.type = Tile.Type.Player1;
										}
										else {
											thisTile.type = Tile.Type.Player2;
										}
										break;
									}
								}
								zcounter++;
							}
							zcounter = 0;
							xcounter++;
						}
						break;
					case "moveconnector":
						xcounter = 0;
						zcounter = 0;
						levelX = LevelData.Split (new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
						foreach (string zRow in levelX){
							levelZ = zRow.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (string tileUnit in levelZ){
								if (tileUnit != "x" && zcounter < GRIDSIZEZ && xcounter < GRIDSIZEX){
									print (currentLevelLayer);
									if (grid[xcounter, zcounter, currentLevelLayer] == null){
										Transform aTileCast = Instantiate (tilePrefab, new Vector3 (xcounter, 0, zcounter), Quaternion.identity) as Transform;
										GameObject aTile = aTileCast.gameObject;
										Tile aTileScript = aTile.GetComponent <Tile> ();
										grid[xcounter,zcounter,currentLevelLayer] = aTileScript;
										aTileScript.xcoord = xcounter;
										aTileScript.zcoord = zcounter;
										aTileScript.layerNumber.x = currentLevelLayer;
										aTileScript.layerNumber.y = currentLevelLayer;
									}
									thisTile = grid[xcounter,zcounter,currentLevelLayer];
									thisTile.movedirections = tileUnit;
								}
								zcounter++;
							}
							zcounter = 0;
							xcounter++;
						}
						break;
					case "height":
						xcounter = 0;
						zcounter = 0;
						levelX = LevelData.Split (new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
						foreach (string zRow in levelX){
							levelZ = zRow.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (string tileUnit in levelZ){
								if (tileUnit != "x" && zcounter < GRIDSIZEZ && xcounter < GRIDSIZEX){
									if (grid[xcounter,zcounter,currentLevelLayer] == null){
										Transform aTileCast = Instantiate (tilePrefab, new Vector3 (xcounter, 0, zcounter), Quaternion.identity) as Transform;
										GameObject aTile = aTileCast.gameObject;
										Tile aTileScript = aTile.GetComponent <Tile> ();
										grid[xcounter,zcounter,currentLevelLayer] = aTileScript;
										aTileScript.xcoord = xcounter;
										aTileScript.zcoord = zcounter;
										aTileScript.layerNumber.x = currentLevelLayer;
										aTileScript.layerNumber.y = currentLevelLayer;
									}
									thisTile = grid[xcounter,zcounter,currentLevelLayer];
									int heightModifier = System.Convert.ToInt32 (tileUnit);
									thisTile.transform.Translate(new Vector3 (0,heightModifier,0));
								}
								zcounter++;
							}
							zcounter = 0;
							xcounter++;
						}
						break;
					case "place":
						xcounter = 0;
						zcounter = 0;
						levelX = LevelData.Split (new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
						foreach (string zRow in levelX){
							levelZ = zRow.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (string tileUnit in levelZ){
								if (tileUnit != "x" && zcounter < GRIDSIZEZ && xcounter < GRIDSIZEX){
									if (grid[xcounter,zcounter,currentLevelLayer] == null){
										Transform aTileCast = Instantiate (tilePrefab, new Vector3 (xcounter, 0, zcounter), Quaternion.identity) as Transform;
										GameObject aTile = aTileCast.gameObject;
										Tile aTileScript = aTile.GetComponent <Tile> ();
										grid[xcounter,zcounter,currentLevelLayer] = aTileScript;
										aTileScript.xcoord = xcounter;
										aTileScript.zcoord = zcounter;
										aTileScript.layerNumber.x = currentLevelLayer;
										aTileScript.layerNumber.y = currentLevelLayer;
									}
									thisTile = grid[xcounter,zcounter,currentLevelLayer];
									int playerModifier = System.Convert.ToInt32 (tileUnit);
										switch(playerModifier){
									case 0:
										thisTile.playerThatCanPlaceUnits = PlayerNumber.None;
										break;
									case 1:
										thisTile.playerThatCanPlaceUnits = PlayerNumber.Player1;
										break;
									case 2:
										thisTile.playerThatCanPlaceUnits = PlayerNumber.Player2;
										break;
									case 3:
										thisTile.playerThatCanPlaceUnits = PlayerNumber.Player3;
										break;
									case 4:
										thisTile.playerThatCanPlaceUnits = PlayerNumber.Player4;
										break;
									}
								}
								zcounter++;
							}
							zcounter = 0;
							xcounter++;
						}
						break;
					case "light":
						xcounter = 0;
						zcounter = 0;
						levelX = LevelData.Split (new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
						foreach (string zRow in levelX){
							levelZ = zRow.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (string tileUnit in levelZ){
								if (tileUnit != "x" && zcounter < GRIDSIZEZ && xcounter < GRIDSIZEX){
									if (grid[xcounter,zcounter,currentLevelLayer] == null){
										Transform aTileCast = Instantiate (tilePrefab, new Vector3 (xcounter, 0, zcounter), Quaternion.identity) as Transform;
										GameObject aTile = aTileCast.gameObject;
										Tile aTileScript = aTile.GetComponent <Tile> ();
										grid[xcounter,zcounter,currentLevelLayer] = aTileScript;
										aTileScript.xcoord = xcounter;
										aTileScript.zcoord = zcounter;
										aTileScript.layerNumber.x = currentLevelLayer;
										aTileScript.layerNumber.y = currentLevelLayer;
									}
									thisTile = grid[xcounter,zcounter,currentLevelLayer];
									string[] lightData = tileUnit.Split (new char[] {'/'},System.StringSplitOptions.RemoveEmptyEntries);
									thisTile.lightHeight = (float) System.Convert.ToDouble(lightData[0]);
									if (lightData[1] != "n"){
										switch (lightData[1]){
										case "1":
											thisTile.tileLight = SwitchButton.Instance.Player1Script.genreScript.genreLight;
											break;
										case "2":
											thisTile.tileLight = SwitchButton.Instance.Player2Script.genreScript.genreLight;
											break;
										}
									}
									thisTile.lightIsLoaded = true;
								}
								zcounter++;
							}
							zcounter = 0;
							xcounter++;
						}
						break;
					}
					CurrentIterations++;
					if (CurrentIterations >= IterationsPerLevel){
					currentLevelLayer++;
					CurrentIterations = 0;
					}
					if (currentLevelLayer > layerCount){
						return;
					}
				}
			}
		}
	}
	
	void ReadFileInfo(string LevelData){
		if (LevelData.Contains ("Layers:")){
			string layerLevelData = LevelData.Replace ("Layers:","");
			layerCount = System.Convert.ToInt32(layerLevelData) - 1;
		}
		else if (LevelData.Contains ("Players:")){
			string playerLevelData = LevelData.Replace ("Players:","");
			PlayerCount = System.Convert.ToInt32(playerLevelData);
		}
		else if (LevelData.Contains ("X:")){
			string xLevelData = LevelData.Replace ("X:","");
			GRIDSIZEX = System.Convert.ToInt32(xLevelData);
		}
		else if (LevelData.Contains ("Z:")){
			string zLevelData = LevelData.Replace ("Z:","");
			GRIDSIZEZ = System.Convert.ToInt32(zLevelData);
		}
		else if (LevelData.Contains ("MapTo:")){
			string mapToLevelData = LevelData.Replace ("MapTo:","");
			LevelCreateOrder = mapToLevelData.Split (new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);
			IterationsPerLevel = LevelCreateOrder.Length;
		}
	}
	
	public override void FindLevelFiles (){
		
	}
	
	protected override void OnDestroy (){
		base.OnDestroy ();
	}
}