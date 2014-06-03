using UnityEngine;
using System.Collections;

public class KitsuneKind : RangedKind {

	// Use this for initialization
	protected override void Awake () {
		SpecialName1 = "Power Shot!";
		SpecialName2 = "Snare Trap!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Kitsune";
		base.UnitTypeSet ();
	}
}
