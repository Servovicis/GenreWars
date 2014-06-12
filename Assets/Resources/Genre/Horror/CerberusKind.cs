using UnityEngine;
using System.Collections;

public class CerberusKind : HeavyKind {

	// Use this for initialization
	 protected override void Awake () {
		Special1Name = "Rip and Tear!";
		Special2Name = "Buff Mode!";
		base.Awake ();
	}

	public override void UnitTypeSet (){
		MyNameOverride = "Cerberus";
		base.UnitTypeSet ();
	}
}
