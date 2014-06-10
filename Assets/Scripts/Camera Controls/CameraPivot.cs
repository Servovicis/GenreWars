using UnityEngine;
using System.Collections;

public class CameraPivot : MonoBehaviour {
	public float RotateSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {
		float rotationDirection = 0f;

		if (Input.GetKey(KeyCode.LeftArrow))
			rotationDirection = -1f;
		else if (Input.GetKey(KeyCode.RightArrow))
			rotationDirection = 1f;

		transform.Rotate (transform.up, rotationDirection * RotateSpeed * Time.deltaTime);	
	}


}
