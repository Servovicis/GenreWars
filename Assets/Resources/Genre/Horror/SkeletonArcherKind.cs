using UnityEngine;
using System.Collections;

public class SkeletonArcherKind : RangedKind {

	// Use this for initialization
	protected override void Awake () {
		SpecialName1 = "Bone Shot!";
		SpecialName2 = "Bone Trap!";
		base.Awake ();
	}
	
	public override void UnitTypeSet (){
		MyNameOverride = "Skeleton Archer";
		base.UnitTypeSet ();
	}
}
