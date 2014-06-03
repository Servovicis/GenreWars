using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActionType {
	NullActionType,
	Move,
	Attack,
	Special,
	Defense,
	UnitWasPlaced
}

public class TurnActionOrderHandler : MonoBehaviour 
{

	#region Singleton
	
	public static TurnActionOrderHandler Instance { get; set; } 
	
	#endregion

	public LinkedList<Action> actionList = new LinkedList<Action>();

	void Awake (){
		Instance = this;
	}

	public void RemoveLastAction(){
		if (actionList.Count > 0) {
			actionList.Last.Value.Undo ();
			actionList.RemoveLast();
		}
	}

	public void ResolveActions(){
		if (actionList.Count > 0) {
			Action ThisAction = actionList.First.Value;
			actionList.RemoveFirst ();
			ThisAction.Resolve ();
		}
	}

	public void UndoGUI()
	{
		if (GUI.Button(new Rect(Screen.width * .9f, 180, 150, 27), "Undo")) 
		{
			RemoveLastAction ();
		}
	}

	public void InitializeList(){
		int listCount = actionList.Count;
		while (listCount > 0){
			actionList.RemoveLast ();
			listCount = actionList.Count;
		}
	}
}
