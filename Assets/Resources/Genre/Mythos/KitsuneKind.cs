using UnityEngine;
using System.Collections;

public class KitsuneKind : RangedKind {

	// Use this for initialization
	protected override void Awake () {
		Special1Name = "Power Shot!";
		Special2Name = "Snare Trap!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Kitsune";
		base.UnitTypeSet ();
	}
}
