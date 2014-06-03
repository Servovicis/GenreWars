using UnityEngine;
using System.Collections;

public class WallKind : Unit {
	const int initHealth = 50;
	const int initDefense = 2;
	const int initDamage = 0;
	const int initMovement = 0;
	const int initMaxRange = 0;
	const int initMinRange = 0;
	const int initPopulationCost = 0;
	const int initSummoningCost = 0;
	const bool initIsMobile = false;
	const bool initIsSelectable = false;
	const bool initIsSpawnable = false;
	const int initUnitMenuItems = 3;
	public string UnitTypeNameOverride = "Wall";
	public string MyNameOverride;
	
	protected override void Awake(){
		base.Awake ();
		this.gameObject.transform.Translate (-0.5F, 0F, -0.5F);
		Health = initHealth;
		MaxHealth = initHealth;
		Damage = initDamage;
		isHealable = false;
		isBuffable = false;
		Defense = initDefense;
		Movement = initMovement;
		MaxRange = initMaxRange;
		MinRange = initMinRange;
		summoningCost = initSummoningCost;
		populationCost = initPopulationCost;
		IsMobile = initIsMobile;
		IsSelectable = initIsSelectable;
		IsSpawnable = initIsSpawnable;
		UnitMenuItems = initUnitMenuItems;
		UnitType = GridCS.UnitType.Wall;
		OnDeath = DeathAction;
		IsKing = false;
		OnDeath += WallDeath;
	}
	
	void Start (){
		GridCS.Instance.grid [(int) Position.x, (int) Position.y, layer].isTrapped = true;
	}
	
	public virtual void WallDeath() {
		GridCS.Instance.grid [(int) Position.x, (int) Position.y, layer].isTrapped = false;
	}
	
	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}
	
	public override void UnitGUIButtons (){
		
	}
	
	void DeathAction (){
		UnitOwner.WallIsDestroyed = true;
	}
}