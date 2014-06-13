using UnityEngine;
using System.Collections;

public class KingKind : Unit {
	const int initHealth = 100;
	const int initDefense = 1;
	const int initDamage = 5;
	const int initMovement = 0;
	const int initMaxRange = 5;
	const int initMinRange = 1;
	const int initPopulationCost = 0;
	const int initSummoningCost = 0;
	const bool initIsMobile = false;
	const bool initIsSelectable = true;
	const bool initIsSpawnable = false;
	const int initUnitMenuItems = 3;
	private Rect GUIGroupSize = new Rect(0, 0, 0, 0);
	private const int GUIButtonWidth = 185;
	private const int GUIButtonHeight = 29;
	private int MinStrikeRange = 1;
	private int MaxStrikeRange = 5;
	private int StrikeDamage = 8;
	private int MinUltraRange = 1;
	private int MaxUltraRange = 10;
	private int UltraDamage = 15;
	public string UnitTypeNameOverride = "King";
	public string MyNameOverride;
	protected string Special1Name = "Spear/Sickle Thrust";
	protected string Special2Name = "Ultra Special";

	protected override void Awake(){
		base.Awake ();
		Health = initHealth;
		MaxHealth = initHealth;
		CritChance = initDamage;
		Defense = initDefense;
		Movement = initMovement;
		MaxRange = initMaxRange;
		MinRange = initMinRange;
		isHealable = false;
		isBuffable = false;
		summoningCost = initSummoningCost;
		populationCost = initPopulationCost;
		IsMobile = initIsMobile;
		IsSelectable = initIsSelectable;
		IsSpawnable = initIsSpawnable;
		UnitMenuItems = initUnitMenuItems;
		GUIGroupSize.height = GUIButtonHeight;
		GUIGroupSize.width = 1000;
		unitGUI = UnitGUI;
		UnitType = GridCS.UnitType.King;
		OnDeath = DeathAction;
		IsKing = true;
		OnActionSelect += InsertGUI;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
		//OnAttack = AOEAttack;
	}

	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}
	
	public void KingUnitGUI ()
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
				OnSpecial = SpearThrust;
				RemoveAbilityRange ();
				SeeIfCanThrust ();
				RemoveAbilityRange += RemoveSeeIfCanThrust;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
			}
		if (GUI.Button (new Rect (625, 0, GUIButtonWidth, GUIButtonHeight), Special2Name)) 
			{
				OnSpecial = UltraSpecial;
				RemoveAbilityRange ();
				SeeIfCanUltra ();
				RemoveAbilityRange += RemoveSeeIfCanUltra;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
			}
			GUI.EndGroup ();
		}
	}

		public virtual void SpearThrust(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
			Debug.Log (TargetPosition + " Ouch!!!");
			HasInteracted = false;
			OnActionDeselect ();
		}
		public virtual void UltraSpecial(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
			Debug.Log (TargetPosition + "What a incredible hit!!!");
			HasInteracted = false;
			OnActionDeselect ();
		}
		public virtual void SeeIfCanThrust(){
			GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinStrikeRange,MaxStrikeRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);	}
		
		public virtual void RemoveSeeIfCanThrust(){
			GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxStrikeRange,layer);
		}
		
		public virtual void SeeIfCanUltra(){
			GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinUltraRange,MaxUltraRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
		}
		
		public virtual void RemoveSeeIfCanUltra(){
			GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxUltraRange,layer);
		}
		public virtual void UndoUltraSelection (Vector2 TargetPosition, Vector2 InitiatorPosition){
			OnActionSelect ();
			HasInteracted = !HasInteracted;
			TurnInteract = new Vector2(0,0);
		}
		
		public virtual void InsertGUI(){
			GameManager.Instance.buttonsGUIFunction += KingUnitGUI;
		}
		
		public virtual void RemoveGUI(){
			GameManager.Instance.buttonsGUIFunction = null;
			OnActionSelect += InsertGUI;
			RemoveAbilityRange = RemoveAttackRange;
		}
	void DeathAction (){
		Application.LoadLevel ("WinScene");
	}
}