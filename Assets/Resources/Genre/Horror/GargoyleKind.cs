using UnityEngine;
using System.Collections;

public class GargoyleKind : SorcererKind {

	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Mystical Rock Spell!";
		Special2Name = "Intense Healing!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Gargoyle";
		base.UnitTypeSet ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
