using UnityEngine;
using System.Collections;

public class MinotaurKind : HeavyKind {
	
	// Use this for initialization
	protected override void Awake () {
		Special1Name = "Charge!";
		Special2Name = "Pump It Up!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Minotaur";
		base.UnitTypeSet ();
	}
}
