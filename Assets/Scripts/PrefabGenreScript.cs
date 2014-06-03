using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabGenreScript : MonoBehaviour {

	public int loadNumber;
	public string genreName;
	public bool isParented;


	public Material tileMat;
	public GameObject Melee;
	public GameObject Ranged;
	public GameObject Sorceror;
	public GameObject Heavy;
	public GameObject Wall;
	public GameObject King;
	public string MeleeName;
	public string RangedName;
	public string SorcerorName;
	public string HeavyName;
	public string KingName;
	public GameObject genreLight;

	public List<GameObject> UnitsList;
	public List<string> UnitTypes;

	GridCS.PlayerNumber _ParentPlayer;
	public GridCS.PlayerNumber ParentPlayer{
		get { return _ParentPlayer; }
		set {
			_ParentPlayer = value;
			Unit UnitScript = Melee.GetComponent<Unit>();
			UnitScript.Owner = value;
			UnitScript = Ranged.GetComponent<Unit>();
			UnitScript.Owner = value;
			UnitScript = Sorceror.GetComponent<Unit>();
			UnitScript.Owner = value;
			UnitScript = Heavy.GetComponent<Unit>();
			UnitScript.Owner = value;
			UnitScript = Wall.GetComponent<Unit>();
			UnitScript.Owner = value;
			UnitScript = King.GetComponent<Unit>();
			UnitScript.Owner = value;
		}
	}

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (this);
		GenreScriptInitializer.LoadItemsIntoGenre (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DestroyIfNotParented(){

	}

}