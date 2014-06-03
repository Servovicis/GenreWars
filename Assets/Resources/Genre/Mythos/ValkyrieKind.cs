using UnityEngine;
using System.Collections;

public class ValkyrieKind : KingKind {

	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Strong Ethreal Spear!";
		Special2Name = "Divine Missile!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Valkyrie";
		base.UnitTypeSet ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
