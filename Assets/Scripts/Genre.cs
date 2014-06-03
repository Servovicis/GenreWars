using UnityEngine;
using System.Collections;

public class Genre : MonoBehaviour {
	
	private string _genreConcatenate;
	public string genreConcatenate {
		get { return _genreConcatenate; }
		set { _genreConcatenate = value; }
	}

	public Material TileMat;

	public int enumLoadValue;
	
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (this);
	}
	
	void updateGenre()
	{
		
	}
}
