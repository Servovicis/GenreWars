using UnityEngine;
using System.Collections;

public class GUIOptionsMenu : MonoBehaviour 
{
	public GUISkin customSkin;
	public GameObject background;
	public GameObject optionsText;


	
	public GUIText musicText;
	public GUIText soundText;
	public GUIText musicValueText;
	public GUIText soundValueText;
	private float mSliderValue = 100.0f;
	private float newMusicValue;
	private float sSliderValue = 100.0f;
	private float newSoundValue;
	private GameObject bg;
	private GameObject ot;
	private GUIPauseMenu pauseMenu;
	private GameManager gameManager;
	
	void Awake()
	{
		audio.volume = PlayerPrefs.GetFloat("Music Volume");
		mSliderValue = audio.volume * 100.0f;
		gameManager = gameObject.GetComponent<GameManager> ();
	}
	
	void Start()
	{
		pauseMenu = gameObject.GetComponent<GUIPauseMenu> (); // as GUIPauseMenu;
		gameManager = gameObject.GetComponent<GameManager> ();
	}
	
	void OnEnable ()
	{
		// Creates Overlay on Pause
		bg = Instantiate(background) as GameObject;
		bg.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
		bg.transform.localPosition = bg.transform.position;
		bg.transform.localRotation = Quaternion.identity;
		ot = Instantiate(optionsText) as GameObject;
		
		//musicText = GameObject.Find("GUITextMusic").guiText;
		//musicValueText = GameObject.Find("GUITextMusicValue").guiText;
		//soundText = GameObject.Find("GUITextSoundFX").guiText;
		//soundValueText = GameObject.Find("GUITextSoundFXValue").guiText;
		mSliderValue = audio.volume * 100f;
		newMusicValue = mSliderValue;
		//		sSliderValue = audio.volume * 100f;
		//		newSoundValue = sSliderValue;
	}
	
	void OnDisable ()
	{
		// Destroys Overlay
		Destroy(bg);
		Destroy(ot);
	}
	
	public void GUIFunction () 
	{
		GUI.skin = customSkin;
		
		GUI.BeginGroup(new Rect (Screen.width * 0.5f - 400, Screen.height * 0.5f - 250, 800, 500));
		
		// Options Menu Background Box
		GUI.Box(new Rect (0, 0, 800, 500), "Options");
		
		// Music Text
		musicText.text = "Music";
		//GUI.Box (new Rect (325, 100, 150, 35), "Music");
		
		//Music Slider (Set at Max)
		newMusicValue = (int)GUI.HorizontalSlider (new Rect (250, 150, 300, 35), newMusicValue, 0f, 100.0f);
		audio.volume = newMusicValue / 100.0f;
		
		//Music Value Display
		musicValueText.text = newMusicValue.ToString ();
		//		GUI.Box (new Rect (350, 175, 100, 35), mSliderValue.ToString ());
		
		// Sound Effects Text
		soundText.text = "Sound Effects";
		//		GUI.Box (new Rect (325, 275, 150, 35), "Sound");
		
		//Sound Effects Slider (Set at Max)
		sSliderValue = (int)GUI.HorizontalSlider (new Rect (250, 325, 300, 35), sSliderValue, 0, 100.0f);
		//audio.volume = sSliderValue / 100.0f;
		
		//Sound Effect Value Display 
		soundValueText.text = sSliderValue.ToString ();
		//		GUI.Box (new Rect (350, 350, 100, 35), sSliderValue.ToString ());
		
		// If Pressed, Save Options 
		if(GUI.Button(new Rect(305, 450, 85, 35), "Save")) 
		{
			PlayerPrefs.SetFloat("Music Volume", audio.volume);
			pauseMenu.enabled = true;
			this.enabled = false;
			//			Application.LoadLevel(0);
		}
		
		// If Pressed, Cancel Changes
		if(GUI.Button(new Rect(410, 450, 85, 35), "Cancel")) 
		{
			audio.volume = mSliderValue / 100f;
			//			audio.volume = sSliderValue / 100f;
			//pauseMenu.enabled = true;
			this.enabled = false;
			gameManager._GoToPauseMenu();
			//			Application.LoadLevel(0);
		}
		
		// If Pressed, Reset Options to Defaults
		if(GUI.Button(new Rect(700, 450, 85, 35), "Defaults")) 
		{
			audio.volume = 1.0f;
			newMusicValue = 100.0f;
			sSliderValue = 100.0f;
		}
		
		GUI.EndGroup ();
	}
}