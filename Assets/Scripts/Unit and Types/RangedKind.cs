using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RangedKind : MobileUnit {
	const int initHealth = 10;
	const int initDefense = 1;
	const int initDamage = 5;
	const int initMovement = 4;
	const int initMaxRange = 7;
	const int initMinRange = 3;
	const int initPopulationCost = 1;
	const int initSummoningCost = 1;
	const bool initIsMobile = true;
	const bool initIsSelectable = true;
	const bool initIsSpawnable = true;
	const int initUnitMenuItems = 3;
	private Rect GUIGroupSize = new Rect(0, 0, 0, 0);
	private const int GUIButtonWidth = 185;
	private const int GUIButtonHeight = 29;
	public string UnitTypeNameOverride = "Ranged";
	public string MyNameOverride;
	private int MinPowerRange = 1;
	private int MaxPowerRange = 10;
	private int PowerDamage = 8;
	private int MinSnareRange = 1;
	private int MaxSnareRange = 2;
	int SnareCounter = 0;
	float MyVelocity = 5F;
	public SnareTrap mySnare;
	bool SnareSelected = false;
	
	protected override void Awake()
	{
		base.Awake ();
		Health = initHealth;
		Damage = initDamage;
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
		GUIGroupSize.height = GUIButtonHeight;
		GUIGroupSize.width = 1000;
		unitGUI = UnitGUI;
		UnitType = GridCS.UnitType.Ranged;
		IsKing = false;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
		//OnAttack = AOEAttack;
	}
	
	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}

	public override void SpecButton1 () {
		OnSpecial = PowerShot;
		RemoveAbilityRange ();
		SeeIfCanPowerShot ();
		RemoveAbilityRange += RemoveSeeIfCanPowerShot;
		OnSpecialSelect = SpecialSelection;
		if (SnareSelected && IsMobile){
			CalculateMoveRange ();
			SnareSelected = false;
		}
	}
	
	public override void SpecButton2 () {
		OnSpecial = SetSnare;
		RemoveAbilityRange ();
		SeeIfCanSnare ();
		RemoveAbilityRange += RemoveSeeIfCanSnare;
		OnSpecialSelect = SnareSelection;
		SnareSelected = true;
	}
	
	public virtual void PowerShot(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		HasInteracted = false;
		OnActionDeselect ();
	}
	
	public virtual void SetSnare(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		HasInteracted = false;
		mySnare = new SnareTrap (TargetPosition, TargetLayer, this);
		OnActionDeselect ();
	}
	
	public virtual void SeeIfCanPowerShot(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinPowerRange,MaxPowerRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);	}
	
	public virtual void RemoveSeeIfCanPowerShot(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxPowerRange,layer);
	}
	
	public virtual void SeeIfCanSnare(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinSnareRange,MaxSnareRange,SwitchButton.Instance.CurrentPlayer.player, false, false, true, layer);
	}
	
	public virtual void RemoveSeeIfCanSnare(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxSnareRange,layer);
	}
	
	public virtual void UndoSnareSelection (Vector2 TargetPosition, Vector2 InitiatorPosition){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
	}
	
	public override void CalculateAttackRange (){
		bool KeepGoing = true;
		LinkedListNode<Unit> thisNode = GameManager.Instance.AllUnits.First;
		while (KeepGoing) {
			if (thisNode != null){
				Unit targetUnit = thisNode.Value;
				if (targetUnit!= null){
					if (ActionHelper.IsInTrajectoryRange(this.transform, targetUnit.transform,MyVelocity, 45F) && targetUnit.UnitOwner != this.UnitOwner)
						GridCS.Instance.grid[(int)targetUnit.Position.x, (int)targetUnit.Position.y,(int)targetUnit.layer].TileSelectionType = Tile.OverlayType.AttackAvailable;
					thisNode = thisNode.Next;
				}
			}
			else{
				KeepGoing = false;
			}
		}
	}
	
	public override void RemoveAttackRange () {
		bool KeepGoing = true;
		LinkedListNode<Unit> thisNode = GameManager.Instance.AllUnits.First;
		while (KeepGoing) {
			if (thisNode != null){
				Unit targetUnit = thisNode.Value;
				if (targetUnit!= null){
					if (targetUnit.UnitOwner != this.UnitOwner)
						GridCS.Instance.grid[(int)targetUnit.Position.x, (int)targetUnit.Position.y,(int)targetUnit.layer].TileSelectionType = Tile.OverlayType.Unselected;
					thisNode = thisNode.Next;
				}
			}
			else{
				KeepGoing = false;
			}
		}
	}
	
	public virtual void SnareSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		print ("The Script Started");
		OnActionDeselect ();
		RemoveAbilityRange ();
		HasInteracted = !HasInteracted;
		TurnInteract = TargetPosition;
		TurnActionOrderHandler.Instance.actionList.AddLast (new PlaceItem(InitiatorPosition, TargetPosition,InitiatorLayer,TargetLayer));
	}
}