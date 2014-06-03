using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Action{
	public abstract void Undo ();
	public abstract void Resolve ();
}

public class Move : Action  {
	private readonly Vector2 targetPosition;
	private readonly int targetLayer;
	private readonly Vector2 initialPosition;
	private readonly int initialLayer;
	private readonly Unit InitiatorUnit;
	
	public Move (Vector2 initiatorPosition, Vector2 targetPosition, int initiatorLayer, int targetLayer)
		: base ()
	{
		this.targetPosition = targetPosition;
		this.targetLayer = targetLayer;
		this.initialPosition = initiatorPosition;
		this.initialLayer = initiatorLayer;
		InitiatorUnit = GridCS.Instance.GetUnitFromGrid (targetPosition, targetLayer);
	}
	
	public override void Undo (){
		InitiatorUnit.OnMoveUndo(targetPosition, initialPosition,targetLayer,initialLayer);
	}
	
	public override void Resolve (){
		InitiatorUnit.OnMove(targetPosition, initialPosition,targetLayer,initialLayer);
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}

public class Attack : Action{
	private readonly Unit TargetUnit;
	private readonly Unit InitiatorUnit;
	
	public Attack (Vector2 initiatorPosition, Vector2 targetPosition, int initiatorLayer, int targetLayer)
		: base ()
	{
		InitiatorUnit = GridCS.Instance.GetUnitFromGrid (initiatorPosition, initiatorLayer);
		TargetUnit = GridCS.Instance.GetUnitFromGrid (targetPosition, targetLayer);
	}
	
	public override void Undo (){
		InitiatorUnit.OnAttackSelectedUndo (TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
		TargetUnit.OnAttackUndo (TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
	}
	
	public override void Resolve (){
		InitiatorUnit.OnAttack(TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
		if (TargetUnit != null) {
			if (TargetUnit.OnHit != null)
				TargetUnit.OnHit (TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
		}
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}

public class SpecialAbility : Action{
	protected readonly Unit TargetUnit;
	protected readonly Unit InitiatorUnit;
	
	public SpecialAbility (Vector2 initiatorPosition, Vector2 targetPosition, int initiatorLayer, int targetLayer)
		: base ()
	{
		InitiatorUnit = GridCS.Instance.GetUnitFromGrid (initiatorPosition, initiatorLayer);
		TargetUnit = GridCS.Instance.GetUnitFromGrid (targetPosition, targetLayer);
	}
	
	public override void Undo (){
		InitiatorUnit.OnSpecialUndo (TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
	}
	
	public override void Resolve (){
		InitiatorUnit.OnSpecial (TargetUnit.Position, InitiatorUnit.Position,TargetUnit.layer,InitiatorUnit.layer);
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}

public class Defense : Action{
	private readonly Unit InitiatorUnit;
	
	public Defense(Vector2 initiatorPosition, int initiatorLayer)
	: base () {
		InitiatorUnit = GridCS.Instance.GetUnitFromGrid (initiatorPosition, initiatorLayer);
	}
	
	public override void Undo (){
		//ActingUnit.OnSpecialUndo (new Vector2 (0,0) , initiatorPosition);
	}
	
	public override void Resolve (){
		InitiatorUnit.OnDefense ();
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}

public class UnitInsert : Action {
	private readonly Unit TargetUnit;
	
	public UnitInsert (Vector2 targetPosition, int targetLayer)
		: base () {
		TargetUnit = GridCS.Instance.GetUnitFromGrid (targetPosition, targetLayer);
	}
	
	public override void Undo (){
		TargetUnit.RemoveUnit ();
	}
	
	public override void Resolve (){
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}

public class PlaceItem : SpecialAbility  {
	private readonly Vector2 targetPosition;
	private readonly int targetLayer;
	private readonly Vector2 initialPosition;
	private readonly int initialLayer;
	
	public PlaceItem (Vector2 initiatorPosition, Vector2 targetPosition, int initiatorLayer, int targetLayer)
		: base (initiatorPosition, targetPosition, initiatorLayer, targetLayer)
	{
		this.targetPosition = targetPosition;
		this.targetLayer = targetLayer;
		this.initialPosition = initiatorPosition;
		this.initialLayer = initiatorLayer;
	}
	
	public override void Undo (){
		InitiatorUnit.OnSpecialUndo(targetPosition, initialPosition,targetLayer,initialLayer);
	}
	
	public override void Resolve (){
		InitiatorUnit.OnSpecial(targetPosition, initialPosition,targetLayer,initialLayer);
		TurnActionOrderHandler.Instance.ResolveActions ();
	}
}