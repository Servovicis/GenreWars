using UnityEngine;
using System.Collections;

public class SetPlayerGenre : MonoBehaviour {

	//public PrefabGenreScript PlayerGenreScript;
	public GameObject ThisPlayer;
	public Player PlayerScript;
	public PrefabGenreScript ObjectGenreScript;

	public void Set(){
		PlayerScript.genreScript = ObjectGenreScript;
		ObjectGenreScript.isParented = true;
		ObjectGenreScript.ParentPlayer = PlayerScript.player;
		}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
