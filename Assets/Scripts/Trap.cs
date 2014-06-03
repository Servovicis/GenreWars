using UnityEngine;
using System.Collections;

public delegate void unitTileEffect (Unit target, Tile thisTile);

public abstract class Trap {

	public Trap (Vector2 myPosition, int myLayer, Unit myOwner){
		position = myPosition;
		layer = myLayer;
		Owner = myOwner;
		parentTile = GridCS.Instance.GetTile(position,layer);
	}

	public readonly Tile parentTile;
	public readonly int layer;
	public readonly Vector2 position;
	public readonly Unit Owner;
}

public class SnareTrap : Trap {

	public GameObject trapObject;
	Unit snaredUnit;
	int SnareCounter = 0;
	RangedKind _RangedOwner;

	public SnareTrap (Vector2 myPosition, int myLayer, RangedKind myOwner) : base (myPosition, myLayer, myOwner){
		parentTile.OnTileEntered += snareOnHit;
		trapObject = GameObject.Instantiate (
					 Resources.Load<GameObject> ("SnareTrap"),
		             new Vector3 (parentTile.transform.position.x,
		             parentTile.transform.position.y + 0.3f,
		             parentTile.transform.position.z),
		             Quaternion.identity)
					 as GameObject;
		_RangedOwner = myOwner;
	}

	public virtual void snareOnHit (Unit target, Tile thisTile){
		if (target.UnitOwner != Owner){
			snaredUnit = target;
			snaredUnit.Health -= 2;
			if (snaredUnit != null){
				GameManager.Instance.OnTurnBegin += snareCountdown;
				SnareCounter = 2;
				snaredUnit.IsMobile = false;
			}
		}
	}
	
	public virtual void snareCountdown (){
		if (SwitchButton.Instance.CurrentPlayer == snaredUnit.UnitOwner){
			SnareCounter -= 1;
			Color trapColor = trapObject.renderer.material.color;
			trapColor = new Color (trapColor.r, trapColor.g, trapColor.b, 0);
			trapObject.renderer.material.color = trapColor;
			if (SnareCounter <= 0) {
				GameManager.Instance.OnTurnBegin -= snareCountdown;
				SnareCounter = 0;
				snaredUnit.IsMobile = true;
				MonoBehaviour.Destroy(trapObject);
				_RangedOwner.mySnare = null;
			}
		}
		else {
			Color trapColor = trapObject.renderer.material.color;
			trapColor = new Color (trapColor.r, trapColor.g, trapColor.b, 1);
			trapObject.renderer.material.color = trapColor;
		}
	}
}