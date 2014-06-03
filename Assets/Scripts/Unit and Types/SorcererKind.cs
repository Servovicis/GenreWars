using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SorcererKind : Unit 
{
	const int initHealth = 10;
	const int initDefense = 1;
	const int initDamage = 6;
	const int initMovement = 2;
	const int initMaxRange = 15;
	const int initMinRange = 10;
	const int initPopulationCost = 1;
	const int initSummoningCost = 1;
	const bool initIsMobile = true;
	const bool initIsSelectable = true;
	const bool initIsSpawnable = true;
	const int initUnitMenuItems = 3;
	private Rect GUIGroupSize = new Rect(0, 0, 0, 0);
	private const int GUIButtonWidth = 185;
	private const int GUIButtonHeight = 29;
	private int MinSpellRange = 1;
	private int MaxSpellRange = 20;
	private int SpellDamage = 6;
	private int MaxHealRange = 2;
	public string UnitTypeNameOverride = "Sorcerer";
	public string MyNameOverride;
	protected string Special1Name = "Spell Attack";
	protected string Special2Name = "Thrust Attack";
	float MyVelocity = 8.75F;
	int WaittoHeal = 0;
	
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
		UnitType = GridCS.UnitType.Sorcerer;
		IsKing = false;
		OnActionSelect += InsertGUI;
		OnActionDeselectExtra = RemoveGUI;
		OnAttack = UnitResolveAttack;
		//OnAttack = AOEAttack;
		OnDeath += sorcererDeath;
	}
	
	public override void UnitTypeSet (){
		UnitTypeName = UnitTypeNameOverride;
		IsSpawnable = initIsSpawnable;
		MyName = MyNameOverride;
	}
	
	public virtual void sorcererDeath () {
		if (WaittoHeal > 0) {
			GameManager.Instance.OnTurnBegin -= HealCountdown;
		}
	}
	
	public virtual void SorcererGUI ()
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
				OnSpecial = SpellAttack;
				RemoveAbilityRange ();
				SeeIfCanSpell ();
				RemoveAbilityRange += RemoveSeeIfCanSpell;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
			}
			if (GUI.Button (new Rect (625, 0, GUIButtonWidth, GUIButtonHeight), Special2Name)) 
			{
				if (WaittoHeal == 0){
				OnSpecial = GiveThemHealth;
				RemoveAbilityRange ();
				SeeIfCanHeal ();
				RemoveAbilityRange += RemoveSeeIfCanHeal;
				//OnActionDeselect = RemoveMoveRange;
				//OnActionDeselect += RemoveAbilityRange;
				}
				else
					Debug.Log ("Turn until Heal = " + WaittoHeal);
			}
			GUI.EndGroup ();
		}
	}
	
	public virtual void AOEAttack(Vector2 TargetPosition, Vector2 InitiatorPosition){
		
	}
	
	public virtual void SpellAttack(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " Big hit!!!");
		if (GridCS.Instance.GetUnitFromGrid(TargetPosition, TargetLayer) != null) {
			Unit TargetUnit = GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer);
		if (!TargetUnit.IsKing || (TargetUnit.IsKing && TargetUnit.UnitOwner.WallIsDestroyed)){
			int thisAttackDamage = 1;
			for (int rolls = 0; rolls < level; rolls++){
				System.Random random = new System.Random();
				int randomNumber = random.Next(1,7);
				if(randomNumber >= SpellDamage){
					thisAttackDamage += 1;
					print ("Attack dealt extra damage!");
					}
					else if (randomNumber <= TargetUnit.Defense){
						thisAttackDamage -= 1;
						print ("Defender prevented 1 damage!");
					}
				}
				if (thisAttackDamage > 0)
					TargetUnit.Health -= thisAttackDamage;
				if (TargetUnit.OnHit != null)
					TargetUnit.OnHit (TargetPosition, InitiatorPosition,TargetLayer,InitiatorLayer);
			}
			else print ("Deflected!");
			if (TargetUnit.IsMobile) {
				double h = ActionHelper.CalculateTwoDiminsionalDistance(InitiatorPosition, TargetPosition);
				int a = (int) (TargetPosition.x - InitiatorPosition.x);
				int o = (int) (TargetPosition.y - InitiatorPosition.y);
				double sin = o/h * 1f;
				double cos = a/h * 1f;
				h += 1;
				int nO = (int) Mathf.Round((float)(sin * h));
				Debug.Log (nO);
				int nA = (int) Mathf.Round((float)(cos * h));
				Debug.Log (nA);
				Vector2 nextPosition = new Vector2 (nA + InitiatorPosition.x, nO + InitiatorPosition.y);
				Debug.Log (nextPosition);
				if (nextPosition.x < GridCS.GRIDSIZEX && nextPosition.x >= 0 && nextPosition.y < GridCS.GRIDSIZEZ && nextPosition.y >= 0
				    && GridCS.Instance.GetTile(nextPosition,TargetLayer) != null && GridCS.Instance.GetUnitFromGrid(TargetPosition,TargetLayer) != null){
					GridCS.Instance.PlaceUnitInGrid (TargetUnit, TargetPosition, TargetLayer, nextPosition, TargetLayer);
					Move MoveToSpace = new Move (TargetPosition,nextPosition,TargetLayer,TargetLayer);
					TargetUnit.Position = nextPosition;
					MoveToSpace.Resolve ();
				}
			}
		}
		HasInteracted = false;
		OnActionDeselect ();
	}
	
	public virtual void GiveThemHealth(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		Debug.Log (TargetPosition + " Were healded up!!!");
		double dist = ActionHelper.CalculateTwoDiminsionalDistance (InitiatorPosition, TargetPosition);
		if (dist > 1) {
			HealEffect (0.5f, GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer));
		}
		else{
			HealEffect (1.0f, GridCS.Instance.GetUnitFromGrid (TargetPosition, TargetLayer));
		}
		WaittoHeal = 2;
		GameManager.Instance.OnTurnBegin += this.HealCountdown;
		HasInteracted = false;
		OnActionDeselect ();
	}
	
	public virtual void HealCountdown(){
		if (SwitchButton.Instance.CurrentPlayer = UnitOwner) {
			WaittoHeal -= 1;
			Debug.Log (WaittoHeal);
			if (WaittoHeal <= 0)
				GameManager.Instance.OnTurnBegin -= HealCountdown;
		}
	}
	
	public virtual void SeeIfCanSpell(){
		GridCS.Instance.CalculateCircularRange(Position,Tile.OverlayType.SpecialAvailable,MinSpellRange,MaxSpellRange,SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);	}
	
	public virtual void RemoveSeeIfCanSpell(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.SpecialAvailable, MaxSpellRange,layer);
	}
	
	public virtual void SeeIfCanHeal(){
		StepHealDetect (GridCS.Instance.GetTile(Position,layer), MaxHealRange - 1);
	}
	
	public virtual void StepHealDetect(Tile thisTile, int Steps){
		Tile nextTile = GridCS.Instance.GetTile (new Vector2 (thisTile.xcoord + 1, thisTile.zcoord), (int) thisTile.layerNumber.x);
		if (nextTile.LoadedUnitScript != null) {
			if(nextTile.LoadedUnitScript.UnitOwner == UnitOwner && nextTile.LoadedUnitScript != this && nextTile.LoadedUnitScript.isHealable)
				nextTile.TileSelectionType = Tile.OverlayType.BuffAvailable;
		}
		if (Steps > 0)
			StepHealDetect (nextTile, Steps - 1);
		nextTile = GridCS.Instance.GetTile (new Vector2 (thisTile.xcoord - 1, thisTile.zcoord), (int)thisTile.layerNumber.x);
		if (nextTile.LoadedUnitScript != null) {
			if(nextTile.LoadedUnitScript.UnitOwner == UnitOwner && nextTile.LoadedUnitScript != this && nextTile.LoadedUnitScript.isHealable)
				nextTile.TileSelectionType = Tile.OverlayType.BuffAvailable;
		}		if (Steps > 0)
			StepHealDetect (nextTile, Steps - 1);
		nextTile = GridCS.Instance.GetTile (new Vector2 (thisTile.xcoord, thisTile.zcoord + 1),(int) thisTile.layerNumber.x);
		if (nextTile.LoadedUnitScript != null) {
			if(nextTile.LoadedUnitScript.UnitOwner == UnitOwner && nextTile.LoadedUnitScript != this && nextTile.LoadedUnitScript.isHealable)
				nextTile.TileSelectionType = Tile.OverlayType.BuffAvailable;
		}		if (Steps > 0)
			StepHealDetect (nextTile, Steps - 1);
		nextTile = GridCS.Instance.GetTile (new Vector2 (thisTile.xcoord, thisTile.zcoord - 1),(int) thisTile.layerNumber.x);
		if (nextTile.LoadedUnitScript != null) {
			if(nextTile.LoadedUnitScript.UnitOwner == UnitOwner && nextTile.LoadedUnitScript != this && nextTile.LoadedUnitScript.isHealable)
				nextTile.TileSelectionType = Tile.OverlayType.BuffAvailable;
		}		if (Steps > 0)
			StepHealDetect (nextTile, Steps - 1);
	}
	
	public void RemoveSeeIfCanHeal(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.BuffAvailable, MaxHealRange,layer);
	}
	
	public void HealEffect(float amount, Unit target){
		int HealthAmount = target.Health + (int) (target.MaxHealth * amount);
		if (HealthAmount > target.MaxHealth)
			target.Health = target.MaxHealth;
		else
			target.Health = HealthAmount;
	}
	
	public virtual void UndoHealSelection (Vector2 TargetPosition, Vector2 InitiatorPosition){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
	}
	
	public virtual void InsertGUI(){
		GameManager.Instance.buttonsGUIFunction += SorcererGUI;
	}
	
	public virtual void RemoveGUI(){
		GameManager.Instance.buttonsGUIFunction = null;
		OnActionSelect += InsertGUI;
		RemoveAbilityRange = RemoveAttackRange;
	}
	
	public override void CalculateAttackRange (){
		bool KeepGoing = true;
		LinkedListNode<Unit> thisNode = GameManager.Instance.AllUnits.First;
		//foreach (Tile targetUnit in GridCS.Instance.grid){
			//if (targetUnit != null){
				//if (targetUnit.xcoord == Position.x || targetUnit.zcoord == Position.y){
					//if (ActionHelper.IsInTrajectoryRange(this.transform, targetUnit.transform,MyVelocity, 0,0))
						//GridCS.Instance.grid[(int)targetUnit.xcoord, (int)targetUnit.zcoord,(int)targetUnit.layerNumber.x].TileSelectionType = Tile.OverlayType.AttackAvailable;
				//}
			//}
		//}
		while (KeepGoing) {
			if (thisNode != null){
				Unit targetUnit = thisNode.Value;
				if (targetUnit!= null){
					if (ActionHelper.IsInTrajectoryRange(this.transform, targetUnit.transform,MyVelocity, 30F) && targetUnit.UnitOwner != this.UnitOwner)
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
	
	public static void InstantiateHeightMarker(float x, float y, float z){
		Instantiate (GameManager.Instance.HeightMarker, new Vector3 (x, y, z), Quaternion.identity);
	}
}