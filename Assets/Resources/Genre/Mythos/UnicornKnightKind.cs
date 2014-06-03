using UnityEngine;
using System.Collections;

public class UnicornKnightKind : MeleeKind {

	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Physical Strike!";
		Special2Name = "Holy Flash!";
		base.Awake ();
	}

	public override void UnitTypeSet (){
		MyNameOverride = "Unicorn Knight";
		base.UnitTypeSet ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}