using UnityEngine;
using System.Collections;

public class StandardGrid : GridCS {

	protected override void Awake ()
	{
		Instance = this;
		DontDestroyOnLoad (this);
		tilePrefab = Resources.Load<Transform> ("Tile");
	}

	protected override void Start ()
	{
		base.Start ();
	}

	public override void CreateGrid ()
	{
		base.CreateGrid ();
	}

	public override void FindLevelFiles ()
	{
		base.FindLevelFiles ();
	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
