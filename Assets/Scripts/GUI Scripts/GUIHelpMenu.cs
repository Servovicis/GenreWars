using UnityEngine;
using System.Collections;

public class GUIHelpMenu : MonoBehaviour 
{
	public GUISkin customSkin;
	public GameObject tutorial;
	public Material unitPlacement;
	public Material resolution;
	public Material attacking;
	public Material unitTypes;

	GameObject tut;

	void Start()
	{
		tut = Instantiate(tutorial) as GameObject;
	}

	void OnGUI () 
	{
		GUI.skin = customSkin;
				
		GUI.BeginGroup(new Rect (Screen.width * 0.25f - 150, Screen.height * 0.5f - 250, 300, 500));
		
		// Help Background Box
		GUI.Box(new Rect (0, 0, 300, 500), "Help Menu");
		
		// If Pressed, Display Unit Cards
		if(GUI.Button(new Rect (75, 75, 160, 35), "Unit Types")) 
		{
//			tut.renderer.material = unitTypes;
		}

		// If Pressed, Display Unit Placement Tutorial
		if(GUI.Button(new Rect (75, 165, 160, 35), "Unit Placement"))
		{
			tut.renderer.material = unitPlacement;
		}

		// If Pressed, Display Attacking Tutorial
		if(GUI.Button(new Rect(75, 255, 160, 35), "Attacking")) 
		{
			tut.renderer.material = attacking;
		}

		// If Pressed, Display Resolution Tutorial
		if(GUI.Button(new Rect(75, 345, 160, 35), "Resolution")) 
		{
			tut.renderer.material = resolution;
		}
		
		// If Pressed, Return to Main Menu
		if(GUI.Button(new Rect(75, 435, 160, 35), "Back")) 
		{
			Application.LoadLevel(0);
		}
		
		GUI.EndGroup ();
	}
}