using UnityEngine;
using System.Collections;

public class PhoenixKind : SorcererKind {

	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Fire Wave!";
		Special2Name = "Mass Heal!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Phoenix";
		base.UnitTypeSet ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
