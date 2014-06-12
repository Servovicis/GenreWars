using UnityEngine;
using System.Collections;

public class MobileUnit : Unit {

	public GameManager.GenericFunction[] MyButtons;

	protected string Special1Name = "Special 1";
	protected string Special2Name = "Special 2";

	protected override void Awake() {
		MyButtons = new GameManager.GenericFunction[4];
		MyButtons [0] = AttackButton;
		MyButtons [1] = DefendButton;
		MyButtons [2] = SpecButton1;
		MyButtons [3] = SpecButton2;
		GameManager.Instance.OnEndPhaseTransition += SummonSickness;
		base.Awake ();
		OnActionSelect += InsertGUI;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
	}

	public virtual void AttackButton () {
		RemoveAbilityRange ();
		CalculateAttackRange ();
		RemoveAbilityRange += RemoveAttackRange;
	}
	
	public virtual void DefendButton () {
	}
	
	public virtual void SpecButton1 () {
	}
	
	public virtual void SpecButton2 () {
	}

	public virtual void InsertGUI(){
		if (!HasInteracted){
			int thisButtonNum = 0;
			foreach (GUILeftPaneButton thisButton in GameManager.Instance.LeftPaneButtons) {
				thisButton.onClick = MyButtons [thisButtonNum];
				thisButtonNum++;
				NGUITools.SetActive(thisButton.gameObject, true);
			}
		}
		else {
			foreach (GUILeftPaneButton thisButton in GameManager.Instance.LeftPaneButtons) {
				thisButton.onClick = null;
				thisButton.myLabel.text = "";
				NGUITools.SetActive(thisButton.gameObject, false);
			}
		}
		GameManager.Instance.LeftPaneButtons [0].myLabel.text = "Attack";
		GameManager.Instance.LeftPaneButtons [1].myLabel.text = "Defend";
		GameManager.Instance.LeftPaneButtons [2].myLabel.text = Special1Name;
		GameManager.Instance.LeftPaneButtons [3].myLabel.text = Special2Name;
	}
	
	public virtual void RemoveGUI(){
		GameManager.Instance.buttonsGUIFunction = null;
		OnActionSelect += InsertGUI;
		RemoveAbilityRange = RemoveAttackRange;
	}

	public override bool HasInteracted {
		get { return _HasInteracted; }
		set {
			_HasInteracted = value;
			InsertGUI ();
		}
	}

	public void SummonSickness () {
		_HasInteracted = false;
		GameManager.Instance.OnEndPhaseTransition -= SummonSickness;
	}
}