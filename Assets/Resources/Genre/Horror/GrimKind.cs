using UnityEngine;
using System.Collections;

public class GrimKind : KingKind {

	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Rage of the Skies!";
		Special2Name = "Hell's Core!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Grim";
		base.UnitTypeSet ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
