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
	//The health variable and accessor
	int _Health;
	public int Health{
		get { return _Health; }
		set {
			//If there's an action designated to happen when health changes, perform it
			if (OnHealthChange != null)
				OnHealthChange (_Health - value);
			//If the unit has been damaged and there is a damage action, perform it
			if (value < _Health && OnDamage != null)
				OnDamage(_Health - value);
			//If the unit was healed and there is a heal action, perform it
			else if (value > _Health && OnHeal != null)
				OnHeal (value - _Health);
			if (value <= MaxHealth) {
				//Set the variable to the value
				_Health = value;
			} else {
				_Health = MaxHealth;
			}
			//If the health is less than zero and there is a death action, perform it and destroy the object.
			if (_Health <= 0){
				if (OnDeath != null){
					IsDed = true;
					OnDeath ();
					Destroy (this.gameObject);
				}
			}
			//Show the unit's health.
			Debug.Log ("Unit Health = "+value);
		}
	}
	
	public int MaxHealth = 10;
	//The greatest possible bonus that can be placed on the unit
	public int MaxBuff = 4;
	//The unit's attack damage, applied on every successful attack and then modified as necessary
	public int BaseDamage = 1;
	//The extra damage done by a critical attack.
	public int CritDamage = 1;
	//The range of possible rolls (1-6) which will result in additional damage on an attack, higher is better.
	public int CritChance;
	//The range of possible rolls (1-6) which will result in less damage on an attack, lower is better.
	public int Defense;
	//The amount of damage that is reduced by a successful defense roll
	public int DamageReduction;
	//The number of spaces a unit can move per turn.
	public int Movement;
	//Ranges for basic attack abilities, unused for parabolic range generation
	public int MaxRange;
	public int MinRange;
	//The cost of summoning the unit in summoning points
	public int summoningCost;
	//The amount of population points that the unit occupies
	public int populationCost;
	//The level of the unit
	public int level = 1;
	//The x/z position of the unit, followed by what layer it's currently on
	public Vector2 Position;
	public int layer = 1;
	//What type of unit that this character is, and who owns it. TODO: Remove reliance on these variables.
	public GridCS.UnitType UnitType;
	public GridCS.PlayerNumber Owner;
	//The booleans that determine if the unit is mobile, selectable, spawnable, king, dead, healable, buffable, or attackable.
	public bool IsMobile;
	public bool IsSelectable;
	public bool IsSpawnable;
	public bool IsKing;
	public bool IsDed = false;
	public bool isHealable = true;
	public bool isBuffable = true;
	public bool isAttackable = true;
	//The particles that the unit spawns when it is attacked.
	public GameObject AttackedParticles;
	//The above particles are held here when they're instantiated.
	public GameObject SpawnedAttackedParticles;
	//How many menu items this unit has.
	public int UnitMenuItems;
	//The player that owns the unit
	public Player UnitOwner;
	//The name of the unit's type, i.e. Melee, Ranged, Sorcerer, Heavy, Wall, King, etc.
	public string UnitTypeName;
	//The unit's name
	public string MyName;
	
	//The nodes that the unit is held in in the LinkedLists held at Player.cs and GameManager.cs, respectively
	public LinkedListNode<Unit> PlayerUnitListNode;
	public LinkedListNode<Unit> UniversalUnitListNode;
	
	//The unit's associated action GUI
	public GameManager.GUIFunction UnitGUI;
	
	//Generic delegates associated only with units.
	public delegate void UnitEvent ();
	//when the unit dies
	public UnitEvent OnDeath;
	//when the unit spawns
	public UnitEvent OnSpawn;
	//when the unit defends
	public UnitEvent OnDefense;
	//when the unit is selected in the Action phase TODO: Figure out how to combine this and OnInsertSelect
	public UnitEvent OnActionSelect;
	//If the unit is selected in the Insert phase
	public UnitEvent OnInsertSelect;
	//Actions that happen after ordinary Action phase deselection
	public UnitEvent OnActionDeselectExtra;
	//Actions that happen after insert phase deselection TODO: Figure out how to remove this
	public UnitEvent OnInsertDeselect;
	//Undoing the unit's insertion
	public UnitEvent OnInsertUndo;
	//Unit abilities load their removal scripts here
	public UnitEvent RemoveAbilityRange;
	
	//Health events. Recieve an integer that indicates the amount of change in health.
	public delegate void UnitHealthEvent (int Delta);
	public UnitHealthEvent OnDamage;
	public UnitHealthEvent OnHeal;
	public UnitHealthEvent OnHealthChange;
	
	//Unit interaction events are any even that one unit targets another unit or space with.
	public delegate void UnitInteractionEvent (Vector2 targetPosition, Vector2 initiatorPosition, int TargetLayer, int InitiatorLayer);
	//The unit's currently-loaded special ability resolution
	public UnitInteractionEvent OnSpecial;
	//The unit's loaded special ability undo delegate
	public UnitInteractionEvent OnSpecialUndo;
	//The unit's loaded special ability selection delegate
	public UnitInteractionEvent OnSpecialSelect;
	//When the unit resolves a move, this delegate procs.
	public UnitInteractionEvent OnMove;
	//When the unit undoes a move, this delegate procs.
	public UnitInteractionEvent OnMoveUndo;
	//When the unit selects a move, this delegate procs.
	public UnitInteractionEvent OnMoveSelect;
	//Attack resolution
	public UnitInteractionEvent OnAttack;
	//Unit is selected to be attacked by another unit
	public UnitInteractionEvent OnAttackSelected;
	//Attack selection
	public UnitInteractionEvent OnAttackSelect;
	//Attack undo
	public UnitInteractionEvent OnAttackUndo;
	//Other unit undoes selection of this unit for attack
	public UnitInteractionEvent OnAttackSelectedUndo;
	//When the unit is hit, this delegate procs.
	public UnitInteractionEvent OnHit;
	
	//A general GUI function that holds the unit's GUI stuff. TODO: Remove this.
	public GameManager.GUIFunction unitGUI;
	
	#endregion
	
	#region Move and Attack variables
	
	//The boolean and accessor that determines whether a unit has acted during a turn. This accessor is overridden in the MobileUnit.cs subclass.
	protected bool _HasInteracted = true;
	public virtual bool HasInteracted {
		get { return _HasInteracted; }
		set { _HasInteracted = value; }
	}
	
	//Indicates whether the unit has moved in this turn.
	public bool HasMoved;
	
	//The space that the unit moved to on this turn. I don't know why I made an accessor for this.
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
	
	//The space that the unit selected to interact on this turn. I don't know why I made an accessor for this.
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
	
	//Whether or not the unit is selected. Unnecessary switch statements galore.
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
	
	#endregion
	#region Basic functions
	
	//Calculate attack and move range. Usually called on unit selection.
	protected void CalculateAttackMoveRange (){
		//The unit will calculate its attack range when it is selected if it has not already interacted.
		if (!HasInteracted) CalculateAttackRange();
		//The unit will calculate its move range if it has not already acted or moved, and it if it is mobile.
		if (!HasMoved && !HasInteracted && IsMobile) CalculateMoveRange ();
	}
	
	//When the unit is deselected, this occurs.
	public void OnActionDeselect (){
		RemoveMoveRange ();
		if (RemoveAbilityRange != null)
			RemoveAbilityRange ();
		if (OnActionDeselectExtra != null)
			OnActionDeselectExtra ();
	}
	
	// Use this for initialization
	protected virtual void Awake () {
		//The unit is automatically selected when it spawns
		selectionState = UnitSelectionState.Selected;
		//It hasn't moved, but it has interacted (i.e. it cannot act on its first turn)
		HasMoved = false;
		HasInteracted = true;
		//The unit is removed on death
		OnDeath = RemoveUnit;
		//the standard OnActionSelect is to calculate the attack and move range of the unit. The standard ability is the attack - this
		//part of the function sets all associated functions to be called by the correct delegates.
		OnActionSelect += CalculateAttackMoveRange;
		RemoveAbilityRange = RemoveAttackRange;
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
	
	//The standard function that sets the name and type of the unit. This is overridden in subclasses.
	public virtual void UnitTypeSet () {}
	
	#endregion
	
	#region Move functions
	
	//Steps the move range outward and activates each tile to show that the unit can be moved there. The function calls here reference ActionHelper.cs, not GridCS.cs.
	public virtual void CalculateMoveRange() {
		Tile ThisTile = GridCS.Instance.GetTile (Position, layer);
		GridCS.Instance.MoveRangeAdvance (ThisTile, Movement, (int) ThisTile.layerNumber.y);
	}
	
	//Erases the unit's move area.
	public virtual void RemoveMoveRange(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.MoveAvailable, Movement, layer);
	}
	
	//When the unit selects the move, this is the ordinary action that it performs.
	public virtual void UnitHasSelectedMove(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		//Deselect to remove all other ranges in play.
		OnActionDeselect ();
		//Set variables associated with movement
		HasMoved = true;
		TurnMove = TargetPosition;
		//Put the unit in the target spot. TODO: Edit this to just place it there in the unit's code, not the tile.
		GridCS.Instance.PlaceUnitInGrid (this, InitiatorPosition, layer, TargetPosition, TargetLayer);
		//Add a move action into the last slot of the action stack.
		TurnActionOrderHandler.Instance.actionList.AddLast (new Move (Position, TargetPosition,InitiatorLayer,TargetLayer));
		//Set the unit's position to the target spot.
		Position = TargetPosition;
		//Re-select the unit for attacking.
		OnActionSelect ();
	}
	
	//Undo the unit's move selection.
	public virtual void UnitUndoMoveSelection(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		//Deselect to remove ranges.
		OnActionDeselect ();
		//Set movement vars
		HasMoved = false;
		TurnMove = new Vector2 (0,0);
		//Place the unit in its original position
		GridCS.Instance.PlaceUnitInGrid (this, TargetPosition, TargetLayer, InitiatorPosition,InitiatorLayer);
		//Reset the position
		Position = InitiatorPosition;
		//Re-select the unit
		OnActionSelect ();
	}
	
	//When the unit's move is resolved, it just reverses its HasMoved boolean.
	public virtual void UnitResolveMove (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		HasMoved = false;
	}
	
	#endregion
	
	#region Unit Attack
	
	//This calculates where the unit can attack.
	//Calculates a circular range by default. See ActionHelper.cs for info on CalculateCircularRange. This is overridden in SorcerorKind and Rangedkind.
	public virtual void CalculateAttackRange(){
		GridCS.Instance.CalculateCircularRange (Position, Tile.OverlayType.AttackAvailable, MinRange, MaxRange, SwitchButton.Instance.CurrentPlayer.player, true, false, false, layer);
	}
	
	//Erases the unit's attack range from the grid.
	public virtual void RemoveAttackRange(){
		GridCS.Instance.EraseRange (Position, Tile.OverlayType.AttackAvailable, MaxRange, layer);
	}
	
	//When the unit has been selected for attack, this function calls. It instantiates particles to indicate that the unit was attacked.
	public virtual void UnitSelectedForAttack(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		if (SpawnedAttackedParticles == null) {
			SpawnedAttackedParticles = (Instantiate (AttackedParticles, new Vector3 (Position.x, 1, Position.y), Quaternion.identity)) as GameObject;
			SpawnedAttackedParticles.transform.rotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
		}
	}
	
	//When you undo a selected attack on the unit, it destroys its particles.
	public virtual void UndoAttackSelected(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		if (SpawnedAttackedParticles != null) {
			Destroy (SpawnedAttackedParticles);
		}
	}
	
	//When the unit has selected its attacked, it deselects, set itself to have interacted, and adds an attack into the action stack.
	public virtual void UnitHasSelectedAttack(Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionDeselect ();
		HasInteracted = !HasInteracted;
		TurnInteract = TargetPosition;
		TurnActionOrderHandler.Instance.actionList.AddLast (new Attack (Position, TargetPosition,InitiatorLayer,TargetLayer));
	}
	
	//When the unit undoes its attack, it selects and resets its abilities.
	public virtual void UndoAttackSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionSelect ();
		HasInteracted = false;
		TurnInteract = new Vector2(0,0);
	}
	
	//This handles how attacks resolved and damage is done.
	public virtual void UnitResolveAttack (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		//Reset HasInteracted so it can act next turn.
		HasInteracted = false;
		//Gets the target unit
		Unit TargetUnit = GridCS.Instance.GetUnitFromGrid (TargetPosition,TargetLayer);
		//If there is a valid target...
		if (TargetUnit != null) {
			//If the target has particles showing, destroy them.
			if (TargetUnit.SpawnedAttackedParticles != null)
				Destroy(TargetUnit.SpawnedAttackedParticles);
			//If the target isn't king, or is the king and a wall has been destroyed...
			if (!TargetUnit.IsKing || (TargetUnit.IsKing && TargetUnit.UnitOwner.WallIsDestroyed)){
				//Sets up the base attack damage as a temp
				int thisAttackDamage = BaseDamage;
				//Rolls dice for as many levels as the unit has.
				for (int rolls = 0; rolls < level; rolls++){
					System.Random random = new System.Random();
					int randomNumber = random.Next(1,7);
					//If the random number exceeds the unit's crit, it does extra damage.
					if(randomNumber >= CritChance){
						thisAttackDamage += CritDamage;
						print ("Attack dealt extra damage!");
					}
					//If the random number is less than the victim's defense threshold, it does less damage.
					if (randomNumber <= TargetUnit.Defense){
						thisAttackDamage -= TargetUnit.DamageReduction;
						print ("Defender prevented 1 damage!");
					}
				}
				//Adjusts the target unit's health
				TargetUnit.Health -= thisAttackDamage;
				//Activates the target unit's OnHit abilities, if any.
				if (TargetUnit.OnHit != null)
					TargetUnit.OnHit (TargetPosition, InitiatorPosition,TargetLayer,InitiatorLayer);
			}
			//If the unit is the king and there are complete walls, then the attack doesn't process.
			else print ("Deflected!");
		}
	}
	
	#endregion
	
	#region Other Actions
	
	//This removes units.
	public virtual void RemoveUnit(){
		//Get the tile associated with the unit, remove the unit from the tile.
		Tile ThisTile = GridCS.Instance.GetTile (Position,layer);
		ThisTile.LoadedUnitScript = null;
		//Removes the unit from the player's population cost and unit list, and the GameManager's master list.
		UnitOwner.Population -= populationCost;
		UnitOwner.UnitList.Remove (PlayerUnitListNode);
		GameManager.Instance.AllUnits.Remove (UniversalUnitListNode);
		//If the unit did not die to be removed, refund its summoning cost to the player.
		if (!IsDed)
			UnitOwner.SummoningPoints += summoningCost;
		//Destroy the unit fully.
		Destroy (this.gameObject);
	}
	
	//When you select a special ability
	public virtual void SpecialSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		//Deselects the unit and removes all ranges
		OnActionDeselect ();
		RemoveAbilityRange ();
		//Record that the unit has interacted.
		HasInteracted = !HasInteracted;
		//Record the target of the unit's attack.
		TurnInteract = TargetPosition;
		//Add the special ability into the last slot in the action stack
		TurnActionOrderHandler.Instance.actionList.AddLast (new SpecialAbility(InitiatorPosition, TargetPosition,InitiatorLayer,TargetLayer));
	}
	
	//Undo the unit's special ability selection. Resets the selection densities and variables associated with having interacted.
	public virtual void UndoSpecialSelection (Vector2 TargetPosition, Vector2 InitiatorPosition, int TargetLayer, int InitiatorLayer){
		OnActionSelect ();
		HasInteracted = !HasInteracted;
		TurnInteract = new Vector2(0,0);
	}
	
	//This is overridden for the GUI. TODO: Remove this.
	public virtual void UnitGUIButtons(){
		
	}
	
	#endregion
	
}