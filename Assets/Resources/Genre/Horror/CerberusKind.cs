using UnityEngine;
using System.Collections;

public class CerberusKind : HeavyKind {

	// Use this for initialization
	 protected override void Awake () {
		SpecialName1 = "Rip and Tear!";
		SpecialName2 = "Buff Mode!";
		base.Awake ();
	}

	public override void UnitTypeSet (){
		MyNameOverride = "Cerberus";
		base.UnitTypeSet ();
	}
}
