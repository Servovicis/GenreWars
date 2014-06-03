using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UnityBuild : Editor {

	const string LevelPath = "Assets/_Scenes/";
	static string[] LevelNames = new string[]{LevelPath+"MainMenu.unity",
		LevelPath+"Player1Choice.unity",
		LevelPath+"Player2Choice.unity",
		LevelPath+"OptionsMenu.unity",
		LevelPath+"HelpScreen.unity",
		LevelPath+"PickUnit.unity",
		LevelPath+"WinScene.unity",
		LevelPath+"LevelSelect.unity"};

	public static char TextSeparator = ',';

	[MenuItem("Genre Wars/Build For Windows")]
	public static void BuildForWindows(){
		UpdateGenres ();
		UpdateGenreUnits ();
		UpdateLevels ();
		string error =BuildPipeline.BuildPlayer( LevelNames, "Builds/Genre Wars.exe", 
		                                 BuildTarget.StandaloneWindows, BuildOptions.None);
		
		if (!string.IsNullOrEmpty(error))
		{
			Debug.LogError(error);
		}
	}

	[MenuItem("Genre Wars/Update Genres")]
	public static void UpdateGenres(){
		string[] GenreNames = Directory.GetFiles (Application.dataPath + "/Resources/GenreScripts/", "*.prefab");
		string FileText = "";
		foreach (string genrefile in GenreNames) {
			string genrefilesanitized = genrefile.Replace (Application.dataPath + "/Resources/GenreScripts/", null);
			genrefilesanitized = genrefilesanitized.Replace (".prefab", null);
			FileText += genrefilesanitized + TextSeparator;
		}
		File.WriteAllText(Application.dataPath + "/Resources/Genres.txt", FileText);
	}

	[MenuItem("Genre Wars/Update Units")]
	public static void UpdateGenreUnits() {
		//Load in the units associated with each genrescript
		string[] GenreScripts = Resources.Load <TextAsset> ("Genres").text.Split (new char[] {TextSeparator}, System.StringSplitOptions.RemoveEmptyEntries);
		foreach(string GenreName in GenreScripts)
		{
			string[] UnitNames = Directory.GetFiles (Application.dataPath + "/Resources/Genre/" + GenreName + "/", "*.prefab");
			string FileText = "";
			foreach (string unitfile in UnitNames) {
				if (unitfile != null){
					string unitfilesanitized = unitfile.Replace (Application.dataPath + "/Resources/Genre/" + GenreName + "/", null);
					unitfilesanitized = unitfilesanitized.Replace (".prefab", null);
					FileText += unitfilesanitized + TextSeparator;
				}
			}
			File.WriteAllText(Application.dataPath + "/Resources/"+GenreName+".txt", FileText);
		}
	}

	[MenuItem("Genre Wars/Update Levels")]
	public static void UpdateLevels(){
		string[] LevelNames = Directory.GetFiles (Application.dataPath + "/Resources/Levels/", "*.txt");
		string FileText = "";
		foreach (string levelfile in LevelNames) {
			string levelfilesanitized = levelfile.Replace (Application.dataPath + "/Resources/Levels/", null);
			levelfilesanitized = levelfilesanitized.Replace (".txt", null);
			FileText += levelfilesanitized + TextSeparator;
		}
		File.WriteAllText(Application.dataPath + "/Resources/Levels.txt", FileText);
	}
}
