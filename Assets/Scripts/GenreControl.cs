using UnityEngine;
using System.Collections;

public class GenreControl : MonoBehaviour {

	public int genreNumber;
	public string[] genreNames;
	public GameObject[] genreArray;
	public PrefabGenreScript genreScript;
	int genrenumber = 0;

	void Awake (){
		DontDestroyOnLoad (this);
	}

	public void DestroyUnusedGenres(){
		foreach (GameObject genre in genreArray) {
			genreScript = genreArray[genrenumber].GetComponent<PrefabGenreScript>();
			if (genreScript.isParented == false) {
				Destroy (genreArray[genrenumber]);
			}
			genrenumber++;
		}
	}
}
