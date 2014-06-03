using UnityEngine;
using System.Collections;

public class OrthoCamera : MonoBehaviour {

	#region Singleton
	
	public static OrthoCamera Instance { get; set; }
	
	#endregion
	
	void Awake () {
		Instance = this;
	}

	void Update () {
		
	}
}
