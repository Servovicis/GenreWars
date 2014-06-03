using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public int SummoningPoints;
	public int Population;
	public float UnitFacingAngle;
	public int MinimumSpawnableArea;
	public int MaximumSpawnableArea;
	public GridCS.PlayerNumber player;
	public GridCS.UnitType unitType;
	public PrefabGenreScript genreScript;
	public bool KingHasSpawned;
	public bool WallIsDestroyed;
	
	public int MaximumSummoningPoints = 100;
	public int MaximumPopulation = 20;

	public LinkedList<Unit> UnitList = new LinkedList<Unit> ();

	void Awake () 
	{
		DontDestroyOnLoad (this);
		SummoningPoints = MaximumSummoningPoints;
		Population = 0;
		KingHasSpawned = false;
		WallIsDestroyed = false;
	}
}