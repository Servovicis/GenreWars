using UnityEngine;
using System.Collections;

public class GUIPlayer1Choice : MonoBehaviour 
{
	public GUISkin customSkin;
	private GameObject PlayerObject;
	private PrefabGenreScript PlayerGenreScript;
	private Player PlayerScript;
	private GameObject PlayerGenre;
	private PrefabGenreScript ObjectGenreScript;
	private SetPlayerGenre SetGenre;
	private GameObject ThisObject;
	private int genreButtonPosition;
	private int genreButtonMaxX;
	private GameObject GenreController;
	private GenreControl GenreControllerScript;
	private string[] genreNames;
	private Vector2 scrollPosition = Vector2.zero;
	
	private const int ButtonLength = 175;
	
	void Awake()
	{
		GenreController = GameObject.Find ("GenreControl");
		GenreControllerScript = GenreController.GetComponent<GenreControl> ();
		genreButtonMaxX = (GenreControllerScript.genreNumber * ButtonLength) + 25;
		genreButtonPosition = 25;
		genreNames = GenreControllerScript.genreNames;
		ThisObject = GameObject.Find ("ChoiceController");
		PlayerObject = GameObject.Find ("Player1(Clone)");
		//PlayerGenreScript = PlayerObject.GetComponent<PrefabGenreScript> ();
		PlayerScript = PlayerObject.GetComponent<Player> ();
		SetGenre = ThisObject.GetComponent<SetPlayerGenre> ();
		SetGenre.ThisPlayer = PlayerObject;
		SetGenre.PlayerScript = PlayerScript;
	}
	
	void OnGUI () 
	{
		GUI.skin = customSkin;
		
		GUI.BeginGroup (new Rect (Screen.width * 0.5f - 400, Screen.height * 0.5f - 250, 800, 500));
		// Options Menu Background Box
		GUI.Box(new Rect(0, 0, 650, 500), "Choose Your Side");
		
		//GUI.skin.scrollView = customSkin;
		scrollPosition = GUI.BeginScrollView (new Rect (150, 50, 500, 400), scrollPosition, new Rect (0, 0, genreButtonMaxX, 325));
		
		populateGenreList ();
		GUI.EndScrollView ();
		
		GUI.EndGroup ();
	}
	
	public void populateGenreList () 
	{
		int genrecounter = 0;
		
		foreach (string button in genreNames) 
		{
			if (GUI.Button (new Rect (genreButtonPosition, 325, 100, 35), button)) 
			{
				PlayerGenre = GameObject.Find (button + "(Clone)");
				ObjectGenreScript = PlayerGenre.GetComponent<PrefabGenreScript> ();
				SetGenre.ObjectGenreScript = ObjectGenreScript;
				SetGenre.PlayerScript = PlayerScript;
				SetGenre.Set ();
				genreNames[genrecounter] = null;
				
				int downsome = 0;
				int counter = -1;
				
				foreach(string unusedvariable in genreNames) 
				{
					counter+=1;
					if (genreNames[counter] == null)
					{
						downsome+=1;
					}
					if (counter + downsome < genreNames.Length)
					{
						genreNames[counter] = genreNames[counter + downsome];
					}
					else
					{
						counter -= 1;
					}
				}
				//if(downsome!=0)
				//{
				//genreNames[counter + downsome] = null;
				//}
				
				System.Array.Resize(ref genreNames, genreNames.Length - downsome);
				GenreControllerScript.genreNames = genreNames;
				GenreControllerScript.genreNumber -= 1;
				//Destroy (PlayerGenre, 0);
				Application.LoadLevel (2);
			}
			genrecounter += 1;
			genreButtonPosition += 175;
		}
		genrecounter = 0;
		genreButtonPosition = 25;
	}
}