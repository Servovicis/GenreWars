using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelect : MonoBehaviour 
{

	#region Singleton
	
	public static LevelSelect Instance { get; protected set; } 
	
	#endregion

	public GUISkin customSkin;
	int _numberOflevels = 1;
	public int numberOfLevels
	{
		get { return _numberOflevels;}
		set {
			_numberOflevels = value + 1;
			GUIGroupSize = new Rect (Screen.width / 2f, Screen.height / 2f, buttonWidth, (value + 1)*buttonHeight * 2);
		}
	}
	
	public List<string> LevelNames = new List<string> ();
	
	public static int buttonHeight = 25;
	public static int buttonWidth = Screen.height / 4;
	
	Rect GUIGroupSize = new Rect(Screen.width / 2f, Screen.height / 2f, buttonWidth, buttonHeight);

	bool HasSelectedLevel = false;

	void Awake () 
	{
		DontDestroyOnLoad (this);
		Instance = this;
		GUIGroupSize = new Rect(Screen.width / 2f, Screen.height / 2f, buttonWidth, buttonHeight);
		buttonWidth = (int) GUIGroupSize.width;
	}

	void OnGUI () 
	{
		GUI.skin = customSkin;

		if (!HasSelectedLevel) 
		{
			int counter = 0;
			GUI.BeginGroup (GUIGroupSize);
			if (GUI.Button (new Rect (0, 0, buttonWidth, buttonHeight), "Standard Level")) 
			{
				Application.LoadLevel ("Player1Choice");
				HasSelectedLevel = true;
			}
			foreach (string LevelName in LevelNames) 
			{
				counter++;
				if (GUI.Button (new Rect (0, buttonHeight * counter * 1.5F, buttonWidth, buttonHeight), LevelName)) 
				{
					GridCS.Instance.SpawnCustomLevel ();
					GridCS.Instance.chosenLevel = LevelName;
					Application.LoadLevel ("Player1Choice");
					HasSelectedLevel = true;
				}
			}
			GUI.EndGroup ();
			counter = 0;
		}
	}

	public void SetLevelSelect ()
	{

		Destroy (this);
	}

	void OnDestroy () {
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
