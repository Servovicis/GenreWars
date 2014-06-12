using UnityEngine;
using System.Collections;

public class GUILeftPaneButton : MonoBehaviour {

	public int MyButtonNumber;
	public GameManager.GenericFunction onClick;
	public UILabel myLabel;

	void Awake () {
		myLabel = this.gameObject.GetComponentInChildren <UILabel> ();
		GameManager.Instance.LeftPaneButtons[MyButtonNumber] = this;
	}

	void Start () {
	}

	void OnClick() {
		if (onClick != null) {
			onClick ();
		}
	}

	public void SpawnedUnitButton () {
		UnitChoice.Instance.SpawnedUnit = UnitChoice.Instance.AllSpawnableUnits[MyButtonNumber].gameObject;
	}
}
