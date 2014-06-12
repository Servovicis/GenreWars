using UnityEngine;
using System.Collections;

public class RightPaneButton : MonoBehaviour {

	public static RightPaneButton Instance { get; set; }
	public GameManager.GenericFunction onClick;
	UILabel myLabel;
	
	void Awake () {
		onClick = GUI_EnterResolvePhase;
		myLabel = this.gameObject.GetComponentInChildren <UILabel> ();
		myLabel.text = "Next Phase";
	}
	
	void OnClick() {
		onClick ();
	}

	public void GUI_EnterResolvePhase() {
		GameManager.Instance._EnterResolvePhase();
		onClick = GUItemp_EnterEndPhase;
	}
	
	public void GUItemp_EnterEndPhase() {
		GameManager.Instance._EnterEndPhase();
		onClick = GUI_EnterStratPhase;
	}

	public void GUI_EnterStratPhase() {
		if (SwitchButton.Instance.CurrentPlayer == SwitchButton.Instance.Player1Script)
			SwitchButton.Instance.CurrentPlayer = SwitchButton.Instance.Player2Script;
		else
			SwitchButton.Instance.CurrentPlayer = SwitchButton.Instance.Player1Script;
		UnitChoice.Instance.ThisPlayer = SwitchButton.Instance.CurrentPlayer;
		GameManager.Instance._EnterInsertPhase();
		onClick = GUI_EnterResolvePhase;
	}
}
