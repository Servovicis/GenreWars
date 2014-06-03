using UnityEngine;
using System.Collections;

public class WerewolfKind : MeleeKind {
	
	// Use this for initialization
	protected override void Awake(){
		Special1Name = "Death Strike!";
		Special2Name = "Annihilation!";
		base.Awake ();
	}

	public override void UnitTypeSet (){
		MyNameOverride = "Werewolf";
		base.UnitTypeSet ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
