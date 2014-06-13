using UnityEngine;
using System.Collections;

public class CursorSelection : MonoBehaviour {
	
	#region Singleton
	
	public static CursorSelection Instance { get; set; } 
	CursorSelection CursorSelectionScript;
	#endregion
	
	public Tile tileMouseOverScript;
	
	private Tile _selectedTile;
	public Tile selectedTile{
		get { return _selectedTile; }
		set { 
			if (_selectedTile == value) return;
			if (_selectedTile != null) selectedTile.TileSelectionType = Tile.OverlayType.Unselected;
			_selectedTile = value;
			if (value != null){
				_selectedTile.TileSelectionType = Tile.OverlayType.TempSelected;
			}
		}
	}
	
	GameManager GameManagerScript;
	public Ray RayCast;
	public RaycastHit HitPoint;
	public LayerMask TileLayer = 1 << 8;
	
	public delegate void del_MouseFunction();
	public del_MouseFunction mouseFunction;
	
	void Update () {
		if (mouseFunction != null) {
			mouseFunction ();
		}
	}
	
	void Awake() {
		Instance = this;
		GameManagerScript = gameObject.GetComponent<GameManager>();
	}
	
	public void InsertMouseFunction(){
		RayCast = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (RayCast, out HitPoint, Mathf.Infinity, TileLayer)) {
			
			if (tileMouseOverScript != HitPoint.transform.gameObject.GetComponent<Tile>()){
				if (tileMouseOverScript != null) tileMouseOverScript.IsMouseOver = false;
				tileMouseOverScript = HitPoint.transform.gameObject.GetComponent<Tile>();
				tileMouseOverScript.IsMouseOver = true;
			}
			
			if (!HitPoint.collider) {
				tileMouseOverScript = null;
			}
			
			if (Input.GetMouseButtonDown (0)) {
				if (tileMouseOverScript != null) {
					switch (tileMouseOverScript.TileSelectionType){
					case Tile.OverlayType.AttackAvailable:
						Unit AttackedUnit = tileMouseOverScript.LoadedUnitScript;
						AttackedUnit.OnAttackSelected(AttackedUnit.Position, selectedTile.LoadedUnitScript.Position, AttackedUnit.layer,
						                              (int) selectedTile.layerNumber.x);
						selectedTile.LoadedUnitScript.OnAttackSelect(AttackedUnit.Position, selectedTile.LoadedUnitScript.Position,
						                                             AttackedUnit.layer, (int) selectedTile.layerNumber.x);
						break;
						
					case Tile.OverlayType.SpecialAvailable:
						selectedTile.LoadedUnitScript.OnSpecialSelect(new Vector2(tileMouseOverScript.xcoord,
						                                                          tileMouseOverScript.zcoord), selectedTile.LoadedUnitScript.Position, (int) tileMouseOverScript.layerNumber.x, (int) selectedTile.layerNumber.x);
						break;
					case Tile.OverlayType.BuffAvailable:
						Unit BuffedUnit = tileMouseOverScript.LoadedUnitScript;
						selectedTile.LoadedUnitScript.OnSpecialSelect(BuffedUnit.Position, selectedTile.LoadedUnitScript.Position,
						                                              BuffedUnit.layer, (int) selectedTile.layerNumber.x);
						break;
					}
					
					if (tileMouseOverScript.TileSelectionType == Tile.OverlayType.Unselected) {
						selectedTile = tileMouseOverScript;
						if (tileMouseOverScript.LoadedUnitScript == null){
							UnitChoice.Instance.GUISelectionBoxInsert ();
						}
						else if (tileMouseOverScript.LoadedUnitScript.gameObject.GetComponent<MobileUnit> () != null &&
						         tileMouseOverScript.LoadedUnitScript.UnitOwner == SwitchButton.Instance.CurrentPlayer) {
							tileMouseOverScript.LoadedUnitScript.gameObject.GetComponent<MobileUnit> ().InsertGUI ();
						}
						else {
							foreach (GUILeftPaneButton thisButton in GameManager.Instance.LeftPaneButtons) {
								thisButton.onClick = null;
								thisButton.myLabel.text = "";
								NGUITools.SetActive(thisButton.gameObject, false);
							}
						}
					}
				}
			}
			if (Input.GetMouseButtonDown(1)){
				if (tileMouseOverScript != null) {
					switch (tileMouseOverScript.TileSelectionType){
					case Tile.OverlayType.MoveAvailable:
						selectedTile.LoadedUnitScript.OnMoveSelect(new Vector2(tileMouseOverScript.xcoord, tileMouseOverScript.zcoord),
						                                           selectedTile.LoadedUnitScript.Position, (int) tileMouseOverScript.layerNumber.x, (int) selectedTile.layerNumber.x);
						break;
						
					}
					if (tileMouseOverScript.TileSelectionType == Tile.OverlayType.Unselected) {
						selectedTile = tileMouseOverScript;
						if (tileMouseOverScript.LoadedUnitScript == null){
							UnitChoice.Instance.GUISelectionBoxInsert ();
						}
						else if (tileMouseOverScript.LoadedUnitScript.gameObject.GetComponent<MobileUnit> () != null &&
						         tileMouseOverScript.LoadedUnitScript.UnitOwner == SwitchButton.Instance.CurrentPlayer) {
							tileMouseOverScript.LoadedUnitScript.gameObject.GetComponent<MobileUnit> ().InsertGUI ();
						}
						else {
							foreach (GUILeftPaneButton thisButton in GameManager.Instance.LeftPaneButtons) {
								thisButton.onClick = null;
								thisButton.myLabel.text = "";
								NGUITools.SetActive(thisButton.gameObject, false);
							}
						}
						UnitChoice.Instance.SpawnButton();
					}
				}
			}
		}
	}
}
