using UnityEngine;
using System.Collections;

public abstract class MeleeKind : Unit 
{
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
	private Rect GUIGroupSize = new Rect(0, 0, 0, 0);
	private const int GUIButtonWidth = 185;
	private const int GUIButtonHeight = 29;
	private int MinSmashRange = 1;
	private int MaxSmashRange = 2;
	//private int SmashDamage = 5;
	private int MinThrustRange = 1;
	private int MaxThrustRange = 5;
	//private int TrustDamage = 7;
	public bool HolyAnnilationAvailable = true;
	public string UnitTypeNameOverride = "Melee";
	public string MyNameOverride;
	protected string Special1Name = "Overhead Smash";
	protected string Special2Name = "Thrust Attack";
	int regDamage = 6;
	int WaittoAnnih = 0;
	
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
		UnitType = GridCS.UnitType.Melee;
		IsKing = false;
		OnActionSelect += InsertGUI;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
		//OnAttack = AOEAttack;
		OnDeath += meleeDeath;
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

	public virtual void MeleeUnitGUI ()
	{
		if (!HasInteracted) {
			GUI.BeginGroup (GUIGroupSize);
			if (GUI.Button (new Rect  (175, 0, GUIButtonWidth, GUIButtonHeight), "Attack!")) 		
			{
				RemoveAbilityRange ();
				CalculateAttackRange ();
				RemoveAbilityRange += RemoveAttackRange;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
			}
			if (GUI.Button (new Rect (400, 0, GUIButtonWidth, GUIButtonHeight), Special1Name)) 
			{
				OnSpecial = PhysicalDeath;
				RemoveAbilityRange ();
				SeeIfCanSmash ();
				RemoveAbilityRange += RemoveSeeIfCanSmash;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
			}
			if (HolyAnnilationAvailable) {
				if (GUI.Button (new Rect (625, 0, GUIButtonWidth, GUIButtonHeight), Special2Name)) {
					if ( WaittoAnnih == 0) {
					OnSpecial = HolyAnnilation;
					RemoveAbilityRange ();
					SeeIfCanThrustAttack ();
					RemoveAbilityRange += RemoveSeeIfCanThrustAttack;
					//OnActionDeselect = RemoveMoveRange;
					//OnActionDeselect += RemoveAbilityRange;
					HolyAnnilationAvailable = false;
					}
				}
			}
			else{
				GUI.Box(new Rect (625, 0, GUIButtonWidth, GUIButtonHeight), Special2Name + ": "+ WaittoAnnih);
			}
			GUI.EndGroup ();
		}
	}
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
	
	public virtual void InsertGUI(){
		GameManager.Instance.buttonsGUIFunction += MeleeUnitGUI;
	}
	
	public virtual void RemoveGUI(){
		GameManager.Instance.buttonsGUIFunction = null;
		OnActionSelect += InsertGUI;
		RemoveAbilityRange = RemoveAttackRange;
	}
}