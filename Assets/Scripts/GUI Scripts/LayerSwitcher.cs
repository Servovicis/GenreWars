using UnityEngine;
using System.Collections;

public class LayerSwitcher : MonoBehaviour {

	#region Singleton
	
	public static LayerSwitcher Instance { get; set; }

	#endregion

	const int GuiWidth = 150;
	const int GuiHeight = 25;
	public int CurrentLayer = 0;
	public int MaxLayer;

	void Awake () {
		Instance = this;
	}

	void Start () {
		MaxLayer = GridCS.layerCount;
	}

	public void GUIFunction () 
	{
		if (GUI.Button(new Rect(Screen.width * .9f, 50, GuiWidth, GuiHeight), "Layer Up"))
		{
			if (CurrentLayer < MaxLayer){
				Debug.Log("Layer Up");
				HideLayer(CurrentLayer);
				CurrentLayer++;
				ShowLayer(CurrentLayer);
			}
			else {
				Debug.Log ("Max Layer");
			}

		}
		if (GUI.Button(new Rect(Screen.width * .9f, 95, GuiWidth, GuiHeight), "Layer Down"))
		{
			if (CurrentLayer > 0)
			{
				Debug.Log("Layer Down");
				HideLayer(CurrentLayer);
				CurrentLayer--;
				ShowLayer(CurrentLayer);
			}
			else 
			{
				Debug.Log ("Min Layer");
			}

		}
		return;
	}

	public void HideLayer (int layer) {
		for (int xspot = 0; xspot < GridCS.Instance.grid.GetLength(0); xspot++){
			for (int zspot = 0; zspot < GridCS.Instance.grid.GetLength(1); zspot++){
				if (GridCS.Instance.grid[xspot, zspot,layer] != null){
					Tile thisTile = GridCS.Instance.grid[xspot, zspot, layer];
					thisTile.gameObject.layer = 9;
					thisTile.transparency = 0.1F;
				}
			}
		}
	}

	public void ShowLayer (int layer) {
		for (int xspot = 0; xspot < GridCS.Instance.grid.GetLength(0); xspot++){
			for (int zspot = 0; zspot < GridCS.Instance.grid.GetLength(1); zspot++){
				if (GridCS.Instance.grid[xspot, zspot,layer] != null){
					Tile thisTile = GridCS.Instance.grid[xspot, zspot, layer];
					thisTile.gameObject.layer = 8;
					thisTile.transparency = 1F;
				}
			}
		}
	}

}
