using UnityEngine;
using System.Collections;

public class SkeletonArcherKind : RangedKind {

	// Use this for initialization
	protected override void Awake () {
		Special1Name = "Bone Shot!";
		Special2Name = "Bone Trap!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Skeleton Archer";
		base.UnitTypeSet ();
	}
}
