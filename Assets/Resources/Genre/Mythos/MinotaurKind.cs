using UnityEngine;
using System.Collections;

public class MinotaurKind : HeavyKind {
	
	// Use this for initialization
	protected override void Awake () {
		SpecialName1 = "Charge!";
		SpecialName2 = "Pump It Up!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Minotaur";
		base.UnitTypeSet ();
	}
}
