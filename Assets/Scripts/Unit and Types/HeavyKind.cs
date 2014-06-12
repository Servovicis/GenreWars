using UnityEngine;
using System.Collections;

public abstract class HeavyKind : MobileUnit {
	const int initHealth = 10;
	const int initDefense = 2;
	const int initDamage = 5;
	const int initMovement = 2;
	const int initMaxRange = 2;
	const int initMinRange = 1;
	const int initPopulationCost = 4;
	const int initSummoningCost = 3;
	const bool initIsMobile = true;
	const bool initIsSelectable = true;
	const bool initIsSpawnable = true;
	const int initUnitMenuItems = 3;
	private Rect GUIGroupSize = new Rect(0, 0, 0, 0);
	private const int GUIButtonWidth = 185;
	private const int GUIButtonHeight = 29;
	public string UnitTypeNameOverride = "Heavy";
	public string MyNameOverride;
	private int MinRipRange = 1;
	private int MaxRipRange = 2;
	private int MinBuffRange = 1;
	private int MaxBuffRange = 2;
	public bool ripCrushAvailable = true;
	public bool buffAvailable = true;
	int RegDamage = 5;
	int WaittoBuff = 0;
	int WaittoRip = 0;
		
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
		UnitType = GridCS.UnitType.Heavy;
		IsKing = false;
		OnActionSelect += InsertGUI;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
		OnDeath += heavyDeath;
	}

	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}

	public virtual void heavyDeath () {
		if (WaittoBuff > 0) {
			GameManager.Instance.OnTurnBegin -= BuffCountdown;
		}
		if (WaittoRip > 0) {
			GameManager.Instance.OnTurnBegin -= RipCrushCountdown;
		}
	}
	public override void SpecButton1 () {
		if (WaittoRip == 0) {
			OnSpecial = RipCrushThem;
			RemoveAbilityRange ();
			SeeIfCanRipCrush ();
			RemoveAbilityRange += RemoveSeeIfCanRipCrush;
			ripCrushAvailable = false;
		}
	}
	
	public override void SpecButton2 () {
		if (WaittoBuff == 0) {
			OnSpecial = GiveThemBuff;
			RemoveAbilityRange ();
			SeeIfCanBuff ();
			RemoveAbilityRange += RemoveSeeIfCanBuff;
			buffAvailable = false;
		}
	}

	public virtual void RipCrushThem(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " We're Crushed, man!!");
		double dist = ActionHelper.CalculateTwoDiminsionalDistance (InitiatorPosition, TargetPosition);
		RipEffect (3, GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer));
		WaittoRip = 4;
		HasInteracted = false;
		OnActionDeselect ();
		GameManager.Instance.OnTurnBegin += RipCrushCountdown;
	}
	
	public virtual void GiveThemBuff(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " Buffed up!!!");
		double dist = ActionHelper.CalculateTwoDiminsionalDistance (InitiatorPosition, TargetPosition);
		BuffEffect (3, GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer));
		WaittoBuff = 3;
		HasInteracted = false;
		OnActionDeselect ();
		GameManager.Instance.OnTurnBegin += BuffCountdown;
		
	}
	
	public virtual void RipEffect(int amount, Unit target){
		target.Health -= RegDamage;
	}
			
	public virtual void RipCrushCountdown(){
		if (SwitchButton.Instance.CurrentPlayer == UnitOwner) {
			WaittoRip -= 1;
			Debug.Log (WaittoRip);
			if (WaittoRip <= 0){
				GameManager.Instance.OnTurnBegin -= RipCrushCountdown;
				ripCrushAvailable = true;
				WaittoRip = 0;
			}
		}
	}
		
	//public virtual void RipCrushAttack (Tile thisTile){
	//	GridCS.Instance.CalculateCircularRange (Position, Tile.OverlayType.SpecialAvailable, MinRipRange, MaxRipRange, SwitchButton.Instance.CurrentPlayer.player, true, layer);
	//}
		
	public virtual void SeeIfCanRipCrush(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinRipRange,MaxRipRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
	}

	public virtual void BuffCountdown(){
		if (SwitchButton.Instance.CurrentPlayer == UnitOwner) {
			WaittoBuff -= 1;
			Debug.Log (WaittoBuff);
			if (WaittoBuff <= 0) {
				GameManager.Instance.OnTurnBegin -= BuffCountdown;
				buffAvailable = true;
			}
		}
	}
		
	public virtual void RemoveSeeIfCanRipCrush(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxRipRange,layer);
	}

	public virtual void SeeIfCanBuff(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.BuffAvailable,MinBuffRange,MaxBuffRange,SwitchButton.Instance.CurrentPlayer.player, false, true, false, layer);
	}
		
	//public virtual void PowerBuffUp(Tile thisTile, int Steps){
	//	GridCS.Instance.CalculateCircularRange (Position, Tile.OverlayType.BuffAvailable, MinBuffRange, MaxBuffRange, SwitchButton.Instance.CurrentPlayer.player, false, layer);
	//}
		
	public virtual void RemoveSeeIfCanBuff(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.BuffAvailable, MaxBuffRange,layer);
	}
		
	public void BuffEffect(int amount, Unit target){
		int BuffAmount = target.BaseDamage + amount;
		if (BuffAmount > target.MaxBuff) 
			target.BaseDamage = target.MaxBuff;
		else
			target.BaseDamage = BuffAmount;
	}
		
	public virtual void UndoHealSelection (Vector2 TargetPosition, Vector2 InitiatorPosition){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
	}
		
}