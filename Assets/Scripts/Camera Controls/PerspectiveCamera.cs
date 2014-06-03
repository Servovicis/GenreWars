using UnityEngine;
using System.Collections;

public class PerspectiveCamera : MonoBehaviour {

	#region Singleton
	
	public static PerspectiveCamera Instance { get; set; }
	
	#endregion
	
	void Awake () {
		Instance = this;
	}
	
	void Update () {
		
	}
}
