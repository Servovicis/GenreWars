using UnityEngine;
using System.Collections;

public abstract class MeleeKind : MobileUnit {
	const int initHealth = 10;
	const int initDefense = 1;
	const int initDamage = 4;
	const int initMovement = 3;
	const int initMaxRange = 1;
	const int initMinRange = 1;
	const int initPopulationCost = 1;
	const int initSummoningCost = 1;
	const bool initIsMobile = true;
	const bool initIsSelectable = true;
	const bool initIsSpawnable = true;
	const int initUnitMenuItems = 3;
	private int MinSmashRange = 1;
	private int MaxSmashRange = 2;
	//private int SmashDamage = 5;
	private int MinThrustRange = 1;
	private int MaxThrustRange = 5;
	//private int TrustDamage = 7;
	public bool HolyAnnilationAvailable = true;
	public string UnitTypeNameOverride = "Melee";
	public string MyNameOverride;
	int regDamage = 6;
	int WaittoAnnih = 0;

	protected override void Awake()
	{
		base.Awake ();
		Health = initHealth;
		CritChance = initDamage;
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
		unitGUI = UnitGUI;
		UnitType = GridCS.UnitType.Melee;
		IsKing = false;
		//OnAttack = AOEAttack;
		OnDeath += meleeDeath;
		MyButtons [0] = AttackButton;
		MyButtons [1] = DefendButton;
		MyButtons [2] = SpecButton1;
		MyButtons [3] = SpecButton2;
	}

	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}

	public virtual void meleeDeath () {
		if (WaittoAnnih > 0) {
			GameManager.Instance.OnTurnBegin -= HolyAnnilationCountdown;
		}
	}

	#region GUI Buttons

	public override void SpecButton1 () {
		OnSpecial = PhysicalDeath;
		RemoveAbilityRange ();
		SeeIfCanSmash ();
		RemoveAbilityRange += RemoveSeeIfCanSmash;
	}

	public override void SpecButton2 () {
		if ( WaittoAnnih == 0) {
			OnSpecial = HolyAnnilation;
			RemoveAbilityRange ();
			SeeIfCanThrustAttack ();
			RemoveAbilityRange += RemoveSeeIfCanThrustAttack;
			HolyAnnilationAvailable = false;
		}
	}

	#endregion

	public virtual void PhysicalDeath(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " We got hit bad!!!");
		HasInteracted = false;
		OnActionDeselect ();
	}
	public virtual void HolyAnnilation(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " Man that hurt!!!");
		RipEffect (3, GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer));
		HasInteracted = false;
		WaittoAnnih = 4;
		GameManager.Instance.OnTurnBegin += HolyAnnilationCountdown;
		OnActionDeselect ();
	}
	
	public virtual void HolyAnnilationCountdown(){
		if (SwitchButton.Instance.CurrentPlayer == UnitOwner) {
			WaittoAnnih -= 1;
			Debug.Log (WaittoAnnih);
			if (WaittoAnnih <= 0){
				GameManager.Instance.OnTurnBegin -= HolyAnnilationCountdown;
				HolyAnnilationAvailable = true;
				WaittoAnnih = 0;
			}
		}
	}
	
	public virtual void RipEffect(int amount, Unit target){
		target.Health -= regDamage;
	}
	
	public virtual void SeeIfCanSmash(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinSmashRange,MaxSmashRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
	}
	
	public virtual void RemoveSeeIfCanSmash(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxSmashRange,layer);
	}
	
	//public virtual void HolyAnnilAttack(Tile thisTile){
	//	GridCS.Instance.CalculateCircularRange (new Vector2 (thisTile.xcoord, thisTile.zcoord), Tile.OverlayType.SpecialAvailable, 0, MaxThrustRange, Owner, true, layer);
	//}
	
	public virtual void SeeIfCanThrustAttack(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinThrustRange,MaxThrustRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
	}
	
	public virtual void RemoveSeeIfCanThrustAttack(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxThrustRange,layer);
	}
	public virtual void UndoThrustAttackSelection (Vector2 TargetPosition, Vector2 InitiatorPosition){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
	}
}