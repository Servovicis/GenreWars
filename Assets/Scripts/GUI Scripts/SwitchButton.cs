using UnityEngine;
using System.Collections;

public class SwitchButton : MonoBehaviour 
{

	#region Singleton
	
	public static SwitchButton Instance { get; set; } 
	
	#endregion

	const int GuiWidth = 95;
	const int GuiHeight = 27;
	public GUISkin customSkin;
	public bool endTurn;
	//public GUISkin mySkin; // assign in the inspector
	GUIStyle GStyle = new GUIStyle();
	public int myNumber;
	public Player CurrentPlayer;

	GameObject GameController;
	
	private const int GRIDSIZEZ = 10;

	public Player Player1Script;
	public Player Player2Script;
	UnitChoice UnitChoiceScript;
	GameManager gameControllerScript;
	GridCS gridScript;

	void Awake()
	{
		Instance = this;
		UnitChoiceScript = this.GetComponent<UnitChoice> ();
		Player1Script = GameObject.Find ("Player1(Clone)").GetComponent<Player> ();
		Player2Script = GameObject.Find ("Player2(Clone)").GetComponent<Player> ();
		gameControllerScript = gameObject.GetComponent<GameManager> ();
		GameController = GameObject.Find ("GameControl");
		gridScript = (GameObject.Find ("Grid Controller")).GetComponent<GridCS> ();
		}

	void Start () 
	{
		CurrentPlayer = Player1Script;
	}

	public void GUIFunction() 
	{
		GUI.skin = customSkin;
		if (GUI.Button(new Rect ( Screen.width * .47f - GuiWidth / 2f, Screen.height * .05f, 180f, GuiHeight), "End Turn"))
		{
			if (CurrentPlayer == Player1Script)
				CurrentPlayer = Player2Script;
			else
				CurrentPlayer = Player1Script;
			UnitChoiceScript.ThisPlayer = CurrentPlayer;
			gameControllerScript._EnterInsertPhase();
		}
		return;
	}

	public void GUIPlayerStats()
	{
		GUI.Label(new Rect(15, 15, 500, 30), "Summoning Points: "+CurrentPlayer.SummoningPoints);
		GUI.Label(new Rect(15, 37, 500, 30), "Unit Count: "+CurrentPlayer.Population);
	}

	public void DisableSpawnableArea()
	{
		for (int xcoord = 0; xcoord < GridCS.GRIDSIZEX; xcoord++) 
		{
			for (int zcoord = 0; zcoord < GridCS.GRIDSIZEZ; zcoord++) 
			{
				for (int ycoord = 0; ycoord + 1 <= gridScript.grid.GetLength(2); ycoord++) 
				{
					Tile ThisTile = gridScript.grid[xcoord, zcoord, ycoord];
					if (ThisTile != null)
					{
						if (ThisTile.playerThatCanPlaceUnits == SwitchButton.Instance.CurrentPlayer.player)
							ThisTile.SpawnableDensitySpawn = false;
					}
				}
			}
		}
	}

	public void EnableSpawnableArea()
	{
		for (int xcoord = 0; xcoord < GridCS.GRIDSIZEX; xcoord++) 
		{
			for (int zcoord = 0; zcoord < GridCS.GRIDSIZEZ; zcoord++) 
			{
				for (int ycoord = 0; ycoord + 1 <= gridScript.grid.GetLength(2); ycoord++) 
				{
					Tile ThisTile = gridScript.grid[xcoord, zcoord, ycoord];
					if (ThisTile != null)
					{
						if (ThisTile.playerThatCanPlaceUnits == SwitchButton.Instance.CurrentPlayer.player && ThisTile.LoadedUnitScript == null)
							ThisTile.SpawnableDensitySpawn = true;
					}
				}
			}
		}
	}
}