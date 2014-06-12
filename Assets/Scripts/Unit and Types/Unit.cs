using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum UnitSelectionState{
	Unselected,
	Selected,
	SelectedForMove,
	SelectedForAttack,
	SelectedForSpecial1,
	SelectedForSpecial2,
	SelectedEnemy
}

public abstract class Unit : MonoBehaviour {
	
	#region Initialization Values
	int _Health;
	public int Health{
		get { return _Health; }
		set {
			if (OnHealthChange != null)
				OnHealthChange (_Health - value);
			if (value < _Health && OnDamage != null)
				OnDamage(_Health - value);
			else if (value > _Health && OnHeal != null)
				OnHeal (value - _Health);
			_Health = value;
			if (_Health <= 0){
				if (OnDeath != null){
					IsDed = true;
					OnDeath ();
					Destroy (this.gameObject);
				}
			}
			Debug.Log ("Unit Health = "+value);
		}
	}
	public int MaxHealth = 10;
	public int MaxBuff = 4;
	public int BaseDamage = 1;
	public int Damage;
	public int Defense;
	public int Movement;
	public int MaxRange;
	public int MinRange;
	public int summoningCost;
	public int populationCost;
	public Vector2 Position;
	public bool IsMobile;
	public bool IsSelectable;
	public bool IsSpawnable;
	public GameObject AttackedParticles;
	public GameObject SpawnedAttackedParticles;
	public GridCS.UnitType UnitType;
	public GridCS.PlayerNumber Owner;
	public int UnitMenuItems;
	public Player UnitOwner;
	public bool IsKing;
	public bool IsDed = false;
	public bool isHealable = true;
	public bool isBuffable = true;
	public bool isAttackable = true;
	public string UnitTypeName;
	public string MyName;
	public int level = 1;
	public int layer = 1;
	
	public LinkedListNode<Unit> PlayerUnitListNode;
	public LinkedListNode<Unit> UniversalUnitListNode;
	
	public GameManager.GUIFunction UnitGUI;
	
	public delegate void UnitEvent ();
	public UnitEvent OnDeath;
	public UnitEvent OnSpawn;
	public UnitEvent OnDefense;
	public UnitEvent OnActionSelect;
	public UnitEvent OnInsertSelect;
	public UnitEvent OnActionDeselectExtra;
	public UnitEvent OnInsertDeselect;
	public UnitEvent OnInsertUndo;
	public UnitEvent RemoveAbilityRange;
	
	public delegate void UnitHealthEvent (int Delta);
	public UnitHealthEvent OnDamage;
	public UnitHealthEvent OnHeal;
	public UnitHealthEvent OnHealthChange;
	
	public delegate void UnitInteractionEvent (Vector2 targetPosition, Vector2 initiatorPosition, int TargetLayer, int InitiatorLayer);
	public UnitInteractionEvent OnSpecial;
	public UnitInteractionEvent OnSpecialUndo;
	public UnitInteractionEvent OnSpecialSelect;
	public UnitInteractionEvent OnMove;
	public UnitInteractionEvent OnMoveUndo;
	public UnitInteractionEvent OnMoveSelect;
	public UnitInteractionEvent OnAttack;
	public UnitInteractionEvent OnAttackSelected;
	public UnitInteractionEvent OnAttackSelect;
	public UnitInteractionEvent OnAttackUndo;
	public UnitInteractionEvent OnAttackSelectedUndo;
	public UnitInteractionEvent OnHit;
	
	#endregion
	
	#region Move and Attack variables
	int _MoveValue;

	protected bool _HasInteracted = true;
	public virtual bool HasInteracted {
		get { return _HasInteracted; }
		set { _HasInteracted = value; }
	}

	public bool HasMoved;
	
	Vector2 _TurnMove;
	public Vector2 TurnMove{
		get { return _TurnMove; }
		set {
			if (_TurnMove == value) return;
			if (_TurnMove != null){
				
			}
			_TurnMove = value;
		}
	}
	
	Vector2 _TurnInteract;
	public Vector2 TurnInteract{
		get { return _TurnInteract; }
		set {
			if (_TurnInteract == value) return;
			if (_TurnInteract != null){
				
			}
			_TurnInteract = value;
		}
	}
	#endregion
	
	#region Selection State Method
	UnitSelectionState _selectionState;
	public UnitSelectionState selectionState{
		get { return _selectionState; }
		set {
			if (_selectionState == value) return;
			else if (GameManager.Instance.turnState == TurnStates.InsertPhase && IsSelectable){
				switch (_selectionState){
				case UnitSelectionState.Selected:
					OnActionDeselect ();
					break;
				}
				_selectionState = value;
				switch (value) {
				case UnitSelectionState.Selected:
					OnActionSelect ();
					break;
				}
			}
			else if (GameManager.Instance.turnState == TurnStates.InsertPhase && IsSelectable){
				switch (_selectionState){
				case UnitSelectionState.Selected:
					if (OnInsertDeselect != null)
						OnInsertDeselect ();
					break;
				}
				_selectionState = value;
				switch (value) {
				case UnitSelectionState.Selected:
					if (OnInsertSelect != null)
						OnInsertSelect ();
					break;
				}
			}
			else _selectionState = value;
		}
	}
	
	protected void CalculateAttackMoveRange (){
		if (!HasInteracted) CalculateAttackRange();
		if (!HasMoved && !HasInteracted && IsMobile) CalculateMoveRange ();
	}
	
	public void OnActionDeselect (){
		RemoveMoveRange ();
		if (RemoveAbilityRange != null)
			RemoveAbilityRange ();
		if (OnActionDeselectExtra != null)
			OnActionDeselectExtra ();
	}
	
	#endregion
	
	public GameManager.GUIFunction unitGUI;
	
	// Use this for initialization
	protected virtual void Awake () {
		selectionState = UnitSelectionState.Selected;
		HasMoved = false;
		HasInteracted = true;
		OnDeath = RemoveUnit;
		OnActionSelect += CalculateAttackMoveRange;
		RemoveAbilityRange = RemoveAttackRange;
		//OnActionDeselect = RemoveMoveRange;
		//OnActionDeselect += RemoveAbilityRange;
		OnAttack = UnitResolveAttack;
		OnAttackSelect = UnitHasSelectedAttack;
		OnAttackSelected = UnitSelectedForAttack;
		OnAttackUndo = UndoAttackSelection;
		OnAttackSelectedUndo = UndoAttackSelected;
		OnMove = UnitResolveMove;
		OnMoveUndo = UnitUndoMoveSelection;
		OnMoveSelect = UnitHasSelectedMove;
		OnInsertUndo = RemoveUnit;
		OnSpecialSelect = SpecialSelection;
		OnSpecialUndo = UndoSpecialSelection;
	}
	
	public virtual void UnitTypeSet () {}
	
	#region Unit Move
	public virtual void CalculateMoveRange() {
		_MoveValue = Movement;
		Tile ThisTile = GridCS.Instance.GetTile (Position, layer);
		//Tile ThisTile = GridCS.Instance.GetTile (Position);
		GridCS.Instance.MoveRangeAdvance (ThisTile, _MoveValue, (int) ThisTile.layerNumber.y);
	}
	
	public virtual void RemoveMoveRange(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.MoveAvailable, Movement, layer);
	}
	
	public virtual void UnitHasSelectedMove(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionDeselect ();
		HasMoved = true;
		TurnMove = TargetPosition;
		GridCS.Instance.PlaceUnitInGrid (this, InitiatorPosition, layer, TargetPosition, TargetLayer);
		TurnActionOrderHandler.Instance.actionList.AddLast (new Move (Position, TargetPosition,InitiatorLayer,TargetLayer));
		Position = TargetPosition;
		OnActionSelect ();
	}
	
	public virtual void UnitUndoMoveSelection(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionDeselect ();
		HasMoved = false;
		TurnMove = new Vector2 (0,0);
		GridCS.Instance.PlaceUnitInGrid (this, TargetPosition, TargetLayer, InitiatorPosition,InitiatorLayer);
		Position = InitiatorPosition;
		OnActionSelect ();
	}
	
	public virtual void UnitResolveMove (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		HasMoved = false;
	}
	#endregion
	
	#region Unit Attack
	public virtual void CalculateAttackRange(){
		GridCS.Instance.CalculateCircularRange (Position, Tile.OverlayType.AttackAvailable, MinRange, MaxRange, SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
	}
	
	public virtual void RemoveAttackRange(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.AttackAvailable, MaxRange, layer);
	}
	
	public virtual void UnitSelectedForAttack(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		if (SpawnedAttackedParticles == null) {
			SpawnedAttackedParticles = (Instantiate (AttackedParticles, new Vector3 (Position.x, 1, Position.y), Quaternion.identity)) as GameObject;
			SpawnedAttackedParticles.transform.rotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
		}
	}
	
	public virtual void UndoAttackSelected(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		if (SpawnedAttackedParticles != null) {
			Destroy (SpawnedAttackedParticles);
		}
	}
	
	public virtual void UnitHasSelectedAttack(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionDeselect ();
		HasInteracted = !HasInteracted;
		TurnInteract = TargetPosition;
		TurnActionOrderHandler.Instance.actionList.AddLast (new Attack (Position, TargetPosition,InitiatorLayer,TargetLayer));
	}
	
	public virtual void UndoAttackSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionSelect ();
		HasInteracted = false;
		TurnInteract = new Vector2(0,0);
	}
	
	public virtual void UnitResolveAttack (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		HasInteracted = false;
		Unit TargetUnit = GridCS.Instance.GetUnitFromGrid (TargetPosition,TargetLayer);
		if (TargetUnit != null) {
			if (TargetUnit.SpawnedAttackedParticles != null)
				Destroy(TargetUnit.SpawnedAttackedParticles);
			if (!TargetUnit.IsKing || (TargetUnit.IsKing && TargetUnit.UnitOwner.WallIsDestroyed)){
				int thisAttackDamage = 1;
				for (int rolls = 0; rolls < level; rolls++){
					System.Random random = new System.Random();
					int randomNumber = random.Next(1,7);
					if(randomNumber >= Damage){
						thisAttackDamage += 1;
						print ("Attack dealt extra damage!");
					}
					else if (randomNumber <= TargetUnit.Defense){
						thisAttackDamage -= 1;
						print ("Defender prevented 1 damage!");
					}
				}
				TargetUnit.Health -= thisAttackDamage;
				if (TargetUnit.OnHit != null)
					TargetUnit.OnHit (TargetPosition, InitiatorPosition,TargetLayer,InitiatorLayer);
			}
			else print ("Deflected!");
		}
	}
	#endregion
	
	#region Other Actions
	public virtual void RemoveUnit(){
		Tile ThisTile = GridCS.Instance.GetTile (Position,layer);
		ThisTile.LoadedUnitScript = null;
		UnitOwner.Population -= populationCost;
		UnitOwner.UnitList.Remove (PlayerUnitListNode);
		GameManager.Instance.AllUnits.Remove (UniversalUnitListNode);
		if (!IsDed)
			UnitOwner.SummoningPoints += summoningCost;
	}
	
	public virtual void SpecialSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		print ("The Script Started");
		OnActionDeselect ();
		RemoveAbilityRange ();
		HasInteracted = !HasInteracted;
		TurnInteract = TargetPosition;
		TurnActionOrderHandler.Instance.actionList.AddLast (new SpecialAbility(InitiatorPosition, TargetPosition,InitiatorLayer,TargetLayer));
	}
	
	public virtual void UndoSpecialSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
		//TurnActionOrderHandler.Instance.RemoveLastAction ();
	}
	
	public virtual void UnitGUIButtons(){
		
	}
	#endregion
}