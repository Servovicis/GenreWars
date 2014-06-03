using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class GameInit : MonoBehaviour {

	public string[] GenreScripts;
	public Player playerClass;
	public GameObject player1Entity;
	public GameObject player2Entity;
	public int genrelistnumber = 0;
	public int genreCount = 0;
	public PrefabGenreScript genrescript;
	public GameObject GenreController;
	public GenreControl GenreControllerScript;
	public const char TextSeparator = ',';

	[SerializeField] private GUIMainMenu MainMenu;

	void Awake ()
	{
		GenreScripts = Resources.Load <TextAsset> ("Genres").text.Split (new char[]{GameInit.TextSeparator},System.StringSplitOptions.RemoveEmptyEntries);
		genreCount = GenreScripts.Length;

		if (MainMenu != null)
		{
			MainMenu.MultiplayerButtonClicked += MainMenu_OnMultiplayerButtonClicked;
		}
	}

	void OnDestroy()
	{
		if (MainMenu != null)
		{
			MainMenu.MultiplayerButtonClicked -= MainMenu_OnMultiplayerButtonClicked;
		}
	}

	public void BeginGame () {
		GenreController = GameObject.Find ("GenreControl");
		GenreControllerScript = GenreController.GetComponent<GenreControl> ();
		GenreControllerScript.genreNames = new string[genreCount];
		Instantiate (player1Entity, new Vector3 (0, 0, 0), Quaternion.identity);
		Instantiate (player2Entity, new Vector3 (0, 0, 0), Quaternion.identity);
		GenreScripts = Resources.Load <TextAsset> ("Genres").text.Split (new char[]{TextSeparator},System.StringSplitOptions.RemoveEmptyEntries);
		foreach(string genrefile in GenreScripts)
		{
			Debug.Log (genrefile);
			UnityEngine.GameObject GenreObject = Instantiate (Resources.Load("GenreScripts/"+genrefile), new Vector3(0,0,0), Quaternion.identity) as GameObject;
			//GameObject GenreObjectCast = GenreObject as GameObject;
			genrescript = GenreObject.GetComponent <PrefabGenreScript>();
			System.Array.Resize(ref GenreControllerScript.genreArray, genrelistnumber+1);
			GenreControllerScript.genreArray[genrelistnumber] = GenreObject;
			GenreControllerScript.genreNames[genrelistnumber] = genrescript.genreName;
			genrescript.loadNumber = genrelistnumber;
			//Player.Genres genreEnum = genrescript.genreName as Player.Genres;
			//genre[genrelistnumber].enumLoadValue = genrelistnumber;
			genrelistnumber += 1;
		}
		GenreControllerScript.genreNumber = genrelistnumber;
	}

	void MainMenu_OnMultiplayerButtonClicked()
	{
		BeginGame();
		Application.LoadLevel("LevelSelect");
	}
}
