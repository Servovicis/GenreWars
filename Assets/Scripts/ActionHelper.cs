using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class ActionHelper
{
	
	public static float grav = (float) (-9.81);
	
	//Finds the tile associated with a place on the grid.
	public static Tile GetTile(this GridCS GridScript, Vector2 Position, int LayerNumber){
		Tile ThisTile = GridScript.grid[(int) Position.x, (int) Position.y, LayerNumber];
		return ThisTile;
	}
	
	//Finds the unit associated with a tile.
	public static Unit GetUnitFromTile(Tile ThisTile){
		Unit ThisUnit = ThisTile.LoadedUnitScript;
		return ThisUnit;
	}
	
	//Gets a unit from a space on the grid without dealing with the tile.
	public static Unit GetUnitFromGrid (this GridCS GridScript, Vector2 Position, int LayerNumber){
		Tile ThisTile = GetTile (GridScript, Position, LayerNumber);
		Unit ThisUnit = GetUnitFromTile (ThisTile);
		return ThisUnit;
	}
	
	//Calculates if a position is inside of the grid, returns a boolean.
	public static bool CheckIfRangeIsInBounds (this GridCS GridScript, Vector2 Position, int LayerNumber){
		bool IsInBounds;
		if (Position.x >= 0 && Position.x < GridCS.GRIDSIZEX && Position.y >= 0 && Position.y < GridCS.GRIDSIZEZ)
			IsInBounds = true;
		else
			IsInBounds = false;
		return IsInBounds;
	}
	
	//Calculates the distance from one space to another on a 2d plane. Can be used for drawing circles.
	public static double CalculateTwoDiminsionalDistance (Vector2 InitialPosition, Vector2 TargetPosition) {
		double hypotenuse = Math.Sqrt (Math.Pow (InitialPosition.x - TargetPosition.x,2) + Math.Pow (InitialPosition.y - TargetPosition.y,2));
		return hypotenuse;
	}
	
	//Calculates the distance from one space to another on a 3d plane. Can be used for determining actual distance between items.
	public static double CalculateThreeDimensionalDistance (Vector3 InitialPosition, Vector3 TargetPosition) {
		double hypotenuse = Math.Sqrt (Math.Pow (InitialPosition.x - TargetPosition.x,2) + Math.Pow (InitialPosition.y - TargetPosition.y,2)
		                               + Math.Pow (InitialPosition.z - TargetPosition.z,2));
		return hypotenuse;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////                 *work on making this only call after move has been resolved*                 /////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Puts a unit in a place on the grid, selects that place.
	public static void PlaceUnitInGrid(this GridCS GridScript, Unit ThisUnit, Vector2 InitiatorPosition, int InitialLayerNumber, Vector2 TargetPosition, int TargetLayerNumber){
		Tile ThisTile = GetTile (GridScript, InitiatorPosition, InitialLayerNumber);
		ThisTile.prevLoadedUnitScript = ThisUnit;
		ThisTile.LoadedUnitScript = null;
		ThisTile = GetTile (GridScript, TargetPosition, TargetLayerNumber);
		ThisTile.LoadedUnitScript = ThisUnit;
		ThisTile.LoadedUnitScript.layer = (int) ThisTile.layerNumber.x;
		ThisUnit.transform.Translate (new Vector3 (TargetPosition.x - InitiatorPosition.x, ThisTile.transform.position.y + Tile.UNITLOADDISTANCE - ThisUnit.transform.position.y, TargetPosition.y - InitiatorPosition.y), Space.World);
		GameObject GameController = GameObject.Find ("GameControl");
		CursorSelection Cursor = GameController.GetComponent<CursorSelection> ();
		Cursor.selectedTile = ThisTile;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Finds all enemies in a circular range of a space, sets the tiles to be selectable in the desired way.
	public static void CalculateCircularRange (this GridCS GridScript, Vector2 Position, Tile.OverlayType DesiredOverlay, int MinRange, int MaxRange, GridCS.PlayerNumber CurrentPlayer, bool FindEnemies, bool findAllies, bool findEmptyTiles, int LayerNumber){
		int MinPosX = ((int) Position.x) - MaxRange;
		int MinPosZ = ((int) Position.y) - MaxRange;
		for (int thisPositionX = 0; thisPositionX <= 2 * MaxRange + 1; thisPositionX++){
			for (int thisPositionZ = 0; thisPositionZ <= 2 * MaxRange + 1; thisPositionZ++){
				Vector2 ThisPosition = new Vector2 (MinPosX + thisPositionX, MinPosZ + thisPositionZ);
				double hypotenuse = CalculateTwoDiminsionalDistance(Position, ThisPosition);
				if (hypotenuse <= MaxRange && hypotenuse >= MinRange) {
					if (CheckIfRangeIsInBounds (GridScript, ThisPosition,LayerNumber)) {
						Tile ThisTile = GetTile (GridScript, ThisPosition,LayerNumber);
						if (ThisTile != null){
							if (FindEnemies){
								if (ThisTile.LoadedUnitScript != null && ThisTile.LoadedUnitScript.Owner != CurrentPlayer)
									ThisTile.TileSelectionType = DesiredOverlay;
							}
							else if (findAllies) {
								if (ThisTile.LoadedUnitScript != null && ThisTile.LoadedUnitScript.Owner == CurrentPlayer)
									ThisTile.TileSelectionType = DesiredOverlay;
							}
							else if (findEmptyTiles){
								ThisTile.TileSelectionType = DesiredOverlay;
							}
						}
					}
				}
			}
		}
	}
	
	//Erases a particular type of ability within range of the desired position.
	public static void EraseRange (this GridCS GridScript, Vector2 Position, Tile.OverlayType DesiredErase, int MaxRange, int LayerNumber){
		int MinPosX = ((int) Position.x) - MaxRange;
		int MinPosZ = ((int) Position.y) - MaxRange;
		for (int thisPositionX = 0; thisPositionX <= 2 * MaxRange + 1; thisPositionX++){
			for (int thisPositionZ = 0; thisPositionZ <= 2 * MaxRange + 1; thisPositionZ++){
				for(int layer = 0; layer <= GridCS.Instance.grid.GetLength(2) - 1; layer++){
					Vector2 TileLocation = new Vector2 (MinPosX + thisPositionX, MinPosZ + thisPositionZ);
					if (CheckIfRangeIsInBounds (GridScript, TileLocation,layer)) {
						Tile ThisTile = GetTile(GridScript, TileLocation,layer);
						if (ThisTile != null){
							if (ThisTile.TileSelectionType == DesiredErase)
								ThisTile.TileSelectionType = Tile.OverlayType.Unselected;
						}
					}
				}
			}
		}
	}
	
	
	public static void MoveRangeAdvance(this GridCS GridScript, Tile aTile,int MoveValue, int LayerNumber) {
		if (MoveValue >0) {
			Tile ThisTile;
			int TargetLayer = LayerNumber;
			if (aTile.movedirections == "n"){
				if (aTile.zcoord + 1 < GridCS.GRIDSIZEZ){
					ThisTile = GridScript.grid [aTile.xcoord, aTile.zcoord + 1, TargetLayer];
					if (ThisTile != null){
						MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
					}
				}
				if (aTile.zcoord -1 >= 0){
					ThisTile = GridScript.grid [aTile.xcoord, aTile.zcoord - 1, TargetLayer];
					if (ThisTile != null){
						MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
					}
				}
				if (aTile.xcoord + 1 < GridCS.GRIDSIZEX){
					ThisTile = GridScript.grid [aTile.xcoord + 1, aTile.zcoord, TargetLayer];
					if (ThisTile != null){
						MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
					}
				}
				if (aTile.xcoord - 1 >= 0){
					ThisTile = GridScript.grid [aTile.xcoord - 1, aTile.zcoord, TargetLayer];
					if (ThisTile != null){
						MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
					}
				}
			}
			else{
				string[] moveoptions = aTile.movedirections.Split (new char[] {'_'},System.StringSplitOptions.RemoveEmptyEntries);
				foreach (string moveoption in moveoptions){
					string[] levelconnector = moveoption.Split (new char[] {'/'},System.StringSplitOptions.RemoveEmptyEntries);
					if (levelconnector.Length == 2)
						TargetLayer = System.Convert.ToInt32(levelconnector[1]);
					switch (levelconnector [0]){
					case "+z":
						if (aTile.zcoord + 1 < GridCS.GRIDSIZEZ){
							ThisTile = GridScript.grid [aTile.xcoord, aTile.zcoord + 1, TargetLayer];
							if (ThisTile != null){
								MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
							}
						}
						break;
					case "-z":
						if (aTile.zcoord -1 >= 0){
							ThisTile = GridScript.grid [aTile.xcoord, aTile.zcoord - 1, TargetLayer];
							if (ThisTile != null){
								MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
							}
						}
						break;
					case "+x":
						if (aTile.xcoord + 1 < GridCS.GRIDSIZEX){
							ThisTile = GridScript.grid [aTile.xcoord + 1, aTile.zcoord, TargetLayer];
							if (ThisTile != null){
								MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
							}
						}
						break;
					case "-x":
						if (aTile.xcoord - 1 >= 0){
							ThisTile = GridScript.grid [aTile.xcoord - 1, aTile.zcoord, TargetLayer];
							if (ThisTile != null){
								MoveRangeNext (GridScript, ThisTile, MoveValue, TargetLayer);
							}
						}
						break;
					}
					TargetLayer = LayerNumber;
				}
			}
		}
	}
	
	public static void MoveRangeNext(this GridCS GridScript, Tile ThisTile ,int MoveValue, int LayerNumber) {
		if (ThisTile.LoadedUnitScript == null){
			ThisTile.TileSelectionType = Tile.OverlayType.MoveAvailable;
			MoveRangeAdvance (GridScript, ThisTile, MoveValue - 1, LayerNumber);
		}
	}
	
	public static bool IsInTrajectoryRange(Transform thisUnit, Transform targetUnit, float initialVelocity, float yAngle){
		Vector3 InitialPosition = new Vector3 (thisUnit.transform.position.x, thisUnit.transform.position.y, thisUnit.transform.position.z);
		Vector3 TargetPosition = new Vector3 (targetUnit.transform.position.x, targetUnit.transform.position.y, targetUnit.transform.position.z);
		double totalDistance = CalculateThreeDimensionalDistance (InitialPosition, TargetPosition);
		double xDistance = CalculateTwoDiminsionalDistance (new Vector2(InitialPosition.x, InitialPosition.z), new Vector2(TargetPosition.x, TargetPosition.z));
		float yDistance = TargetPosition.y - InitialPosition.y;
		Vector3 initialToTarget = targetUnit.transform.position - thisUnit.transform.position;
		Vector2 newVelocity = new Vector2 ((float) Math.Cosh (Math.PI / 180 * (90 - yAngle)) * initialVelocity, (float) Math.Sinh (Math.PI / 180 * yAngle)* initialVelocity);
		float Height = GetTrajectoryYHeight (newVelocity, xDistance, thisUnit.transform.position);
		//SorcererKind.InstantiateHeightMarker (TargetPosition.x, Height, TargetPosition.z);
		if (Height + 1F > targetUnit.transform.position.y && Height - 1F < targetUnit.transform.position.y){
			//float DesiredAngle = (float)Math.Abs (Math.Sinh (grav * xDistance / Math.Pow (initialVelocity, 2)) / 2);
			//double sqrt = (initialVelocity*initialVelocity*initialVelocity*initialVelocity) + (grav*(grav*(xDistance*xDistance) + 2*yDistance*(initialVelocity*initialVelocity)));
			//sqrt = Math.Sqrt(sqrt);
			//double DesiredAngle1 = Math.Abs (Math.Atan2(((initialVelocity*initialVelocity) + sqrt), (grav*xDistance)));
			//double DesiredAngle2 = Math.Abs (Math.Atan2(((initialVelocity*initialVelocity) - sqrt), (grav*xDistance)));
			//float DesiredAngle = 0F;
			////Debug.Log ("xDistance = " + xDistance);
			//if (Math.Abs (DesiredAngle1) <= Math.Abs (DesiredAngle2))
			//	DesiredAngle = (float) DesiredAngle1;
			//else
			//	DesiredAngle = (float) DesiredAngle2;
			//Vector3 Pos = InitialPosition;
			//float timeScale = 1/10F;
			//float time = timeScale;
			//if (DesiredAngle != 0 && DesiredAngle != Math.PI/2F){
			//	float dX = (float) (TargetPosition.x - InitialPosition.x);
			//	float dZ = (float) (TargetPosition.z - InitialPosition.z);
			//	float vY0 = (float) Math.Sin (DesiredAngle) * initialVelocity;
			//	float vXZ = 0F - (float) Math.Cos (DesiredAngle) * initialVelocity;
			//	Vector2 velXZ = (new Vector2 (dX,dZ).normalized) * (vXZ * timeScale);
			//	float timeTotal = (float) Math.Abs ((2 * initialVelocity * Math.Sin (DesiredAngle))/grav);
			//	while (time < timeTotal + timeScale) {
			//		float yScale = (vY0 * time) + (float) (grav * Math.Pow (time, 2)/2F);
			//		Vector3 nextPos = new Vector3 (Pos.x + velXZ.x, yScale, Pos.z + velXZ.y);
			//		float distance = (float) CalculateThreeDimensionalDistance(Pos,nextPos);
			//		Vector3 direction = Vector3.Normalize(nextPos - Pos);
			//		Physics.Raycast (Pos, direction, distance);
			//		Debug.DrawLine(Pos, nextPos, Color.red,10F);
			//		Pos = nextPos;
			//		time += timeScale;
			//	}
			//}
			return true;
		}
		else
			return false;
	}
	
	public static float GetTrajectoryYHeight (Vector2 Velocity, double Distance, Vector3 initialPosition){
		float timeToReachTarget = Math.Abs((float) Distance/Velocity.x);
		float finalHeight = (Velocity.y * timeToReachTarget) + initialPosition.y + ((ActionHelper.grav * (float) Math.Pow(timeToReachTarget,2)) * 0.5f);
		return finalHeight;
	}
	
	
}
