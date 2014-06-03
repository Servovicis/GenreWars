
using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	#region Constants

	private const float kSelectionAlpha = 0.5f;
	private const float kTempSelectedSelectionYOffset = 0.2f;
	private const float kMoveAvailableSelectionYOffset = 0.2f;
	private const float kAttackAvailableSelectionYOffset = 0.2f;
	private const float kBuffAvailableSelectionYOffset = .2f;
	private const float kSpecialAvailableSelectionYOffset = .2f;

	#endregion

	#region Enums
	
	public enum Type
	{
		Player1King,
		Player1Wall,
		Player1,
		Player2King,
		Player2Wall,
		Player2
	}
	
	public enum OverlayType
	{
		Unselected,
		TempSelected,
		MoveAvailable,
		AttackAvailable,
		BuffAvailable,
		SpecialAvailable
	}
	
	#endregion
	#region Initialization Values

	public GridCS.UnitType unit;
	public GridCS.PlayerNumber player;
	public GridCS.UnitType tempUnit;
	public GridCS.PlayerNumber tempPlayer;
	public GridCS.PlayerNumber playerThatCanPlaceUnits;
	public int xcoord;
	public Vector2 layerNumber = new Vector2 (0, 0);
	public int zcoord;
	public const float UNITLOADDISTANCE = 0.05F;
	public string movedirections = "n";

	public GameObject tileLight;
	public GameObject loadedTileLight;
	float _lightHeight;
	public float lightHeight{
		get { return _lightHeight; }
		set {
			if (_lightHeight == value)
				return;
			_lightHeight = value;
			if (loadedTileLight != null)
				loadedTileLight.transform.Translate(new Vector3(0,value - loadedTileLight.transform.position.y + this.gameObject.transform.position.y, 0));
		}
	}
	
	bool _lightIsLoaded;
	public bool lightIsLoaded{
		get { return _lightIsLoaded; }
		set {
			if (_lightIsLoaded == value)
				return;
			_lightIsLoaded = value;
			if (value)
				loadedTileLight = Instantiate(tileLight,new Vector3(xcoord,lightHeight+this.gameObject.transform.position.y,zcoord),Quaternion.identity) as GameObject;
			else
				Destroy(loadedTileLight);
		}
	}

	#endregion
	#region Objects And Scripts
	public unitTileEffect OnTileEntered;
	public unitTileEffect OnTileExited;
	public bool isTrapped = false;

	public GameObject Player1Object;
	public Player Player1Script;
	public GameObject Player2Object;
	public Player Player2Script;
	SwitchButton SwitchScript;
	public Player LoadedPlayerScript;

	Unit _LoadedUnitScript;
	public Unit LoadedUnitScript {
		get { return _LoadedUnitScript; }
		set {
			if (value != null) {
				unit = value.UnitType;
				tempUnit = value.UnitType;
				player = value.Owner;
				tempPlayer = value.Owner;
				if (OnTileEntered != null)
					OnTileEntered(value, this);
			}
			else {
				unit = GridCS.UnitType.None;
				tempUnit = GridCS.UnitType.None;
				player = GridCS.PlayerNumber.None;
				tempPlayer = GridCS.PlayerNumber.None;
				if (OnTileExited != null)
					OnTileExited(LoadedUnitScript, this);
			}
			_LoadedUnitScript = value;
		}
	}
	public Unit prevLoadedUnitScript;

	#endregion
	#region Tile Selection


	public GameObject MouseOverBox;
	private bool _IsMouseOver;
	public bool IsMouseOver{
		get { return _IsMouseOver;}
		set {
			if (value) MouseOverBox = (Instantiate (Resources.Load("SelectionDensity"),new Vector3 (xcoord, (this.transform.position.y + .3F), zcoord), Quaternion.identity)) as GameObject;
			else if (MouseOverBox != null) Destroy(MouseOverBox);
			_IsMouseOver = value;
		}
	}

	public Object SelectionBoxCast;
	public GameObject SelectionBox;
	public OverlayType _TileSelectionType;
	public OverlayType TileSelectionType {
		get{ return _TileSelectionType; }
		set{
			if (_TileSelectionType == value) return;
			_TileSelectionType = value;
			switch (value){
			case OverlayType.TempSelected:
				Destroy(SelectionBox);
				SelectionBoxCast = Instantiate(Resources.Load("SelectionDensity"), new Vector3 (xcoord, (this.transform.position.y + kTempSelectedSelectionYOffset), zcoord), Quaternion.identity);
				SelectionBox = SelectionBoxCast as GameObject;
				Color BoxColor = SelectionBox.renderer.material.color;
				BoxColor = Color.blue;
				BoxColor.a = kSelectionAlpha;
				SelectionBox.renderer.material.color = BoxColor;
				if (LoadedUnitScript != null){
					LoadedPlayerScript = SwitchScript.CurrentPlayer;
					if (LoadedUnitScript.Owner == LoadedPlayerScript.player) LoadedUnitScript.selectionState = UnitSelectionState.Selected;
					else LoadedUnitScript.selectionState = UnitSelectionState.SelectedEnemy;
				}
				break;
			case OverlayType.MoveAvailable:
				Destroy(SelectionBox);
				SelectionBoxCast = Instantiate(Resources.Load("SelectionDensity"), new Vector3 (xcoord, (this.transform.position.y + kMoveAvailableSelectionYOffset), zcoord), Quaternion.identity);
				SelectionBox = SelectionBoxCast as GameObject;
				BoxColor = SelectionBox.renderer.material.color;
				BoxColor = Color.yellow;
				BoxColor.a = kSelectionAlpha;
				SelectionBox.renderer.material.color = BoxColor;
				break;
			case OverlayType.AttackAvailable:
				Destroy(SelectionBox);
				SelectionBoxCast = Instantiate(Resources.Load("SelectionDensity"), new Vector3 (xcoord, (this.transform.position.y + kAttackAvailableSelectionYOffset), zcoord), Quaternion.identity);
				SelectionBox = SelectionBoxCast as GameObject;
				BoxColor = SelectionBox.renderer.material.color;
				BoxColor = Color.red;
				BoxColor.a = kSelectionAlpha;
				SelectionBox.renderer.material.color = BoxColor;
				break;
			case OverlayType.BuffAvailable:
				Destroy(SelectionBox);
				SelectionBoxCast = Instantiate(Resources.Load("SelectionDensity"), new Vector3 (xcoord, (this.transform.position.y + kBuffAvailableSelectionYOffset), zcoord), Quaternion.identity);
				SelectionBox = SelectionBoxCast as GameObject;
				BoxColor = SelectionBox.renderer.material.color;
				BoxColor = Color.green;
				BoxColor.a = kSelectionAlpha;
				SelectionBox.renderer.material.color = BoxColor;
				break;
			case OverlayType.SpecialAvailable:
				Destroy(SelectionBox);
				SelectionBoxCast = Instantiate(Resources.Load("SelectionDensity"), new Vector3 (xcoord, (this.transform.position.y + kSpecialAvailableSelectionYOffset), zcoord), Quaternion.identity);
				SelectionBox = SelectionBoxCast as GameObject;
				BoxColor = SelectionBox.renderer.material.color;
				BoxColor = Color.magenta;
				BoxColor.a = kSelectionAlpha;
				SelectionBox.renderer.material.color = BoxColor;
				break;
			case OverlayType.Unselected:
				if (LoadedUnitScript != null) LoadedUnitScript.selectionState = UnitSelectionState.Unselected;
				Destroy(SelectionBox);
				break;
			}
		}
	}

	#endregion
	#region transparency

	float _transparency;
	public float transparency {
		get { return _transparency; }
		set {
			if (_transparency == value)
				return;
			_transparency = value;
			Color color = this.gameObject.renderer.material.color;
			color.a = value;
			this.gameObject.renderer.material.color = color;
			if (SelectionBox != null){
				color = SelectionBox.renderer.material.color;
				color.a = value * .5F;
				SelectionBox.renderer.material.color = color;
			}
			if (SpawnableBox != null){
				color = SpawnableBox.renderer.material.color;
				color.a = value * .25F;
				SpawnableBox.renderer.material.color = color;
			}
			//if (LoadedUnitScript != null){
			//	color = LoadedUnitScript.gameObject.renderer.material.color;
			//	color.a = value;
			//	LoadedUnitScript.gameObject.renderer.material.color = color;
			//}
		}
	}

	#endregion
	#region Tile Type

	Material TileTexture;
	private Type _type;
	public Type type
	{
		get { return _type; }
		set
		{
			_type = value;
			UpdateColorForGenre();
		}
	}

	void UpdateColorForGenre()
	{
		switch(type){
		case Type.Player1:
			TileTexture = Player1Script.genreScript.tileMat;
			gameObject.renderer.material = TileTexture;
			break;
		case Type.Player2:
			TileTexture = Player2Script.genreScript.tileMat;
			gameObject.renderer.material = TileTexture;
			break;
		case Type.Player1King:
			TileTexture.color = Color.red;
			break;
		case Type.Player1Wall:
			TileTexture.color = Color.black;
			break;
		case Type.Player2King:
			TileTexture.color = Color.red;
			break;
		case Type.Player2Wall:
			TileTexture.color = Color.black;
			break;
		}
	}

	#endregion
	#region Spawnable Box Dimensions

	public GameObject SpawnableBox;
	public Object SpawnableBoxCast;
	private bool _SpawnableDensitySpawn;
	public bool SpawnableDensitySpawn {
		get{ return _SpawnableDensitySpawn; }
		set{
			_SpawnableDensitySpawn = value;
			if (value == true){
				SpawnableBoxCast = Instantiate (Resources.Load("SpawnableDensity"),new Vector3 (xcoord, (this.transform.position.y + .1F), zcoord), Quaternion.identity);
				SpawnableBox = SpawnableBoxCast as GameObject;
			}
			else{
				Destroy (SpawnableBoxCast);
			}
		}
	}

	#endregion
	#region Initialize

	void Awake()
	{
		TileTexture = renderer.material;
		Player1Object = GameObject.Find ("Player1(Clone)");
		Player1Script = Player1Object.GetComponent<Player> ();
		Player2Object = GameObject.Find ("Player2(Clone)");
		Player2Script = Player2Object.GetComponent<Player> ();
		GameObject GameController = GameObject.Find ("GameControl");
		SwitchScript = GameController.GetComponent<SwitchButton> ();
		LoadedPlayerScript = SwitchScript.CurrentPlayer;
	}

	#endregion
}