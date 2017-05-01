using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {
	Button MODE, MORE, LESS, START, QUIT;
	bool isMode, isMore, isLess, isStart, isQuit;
	Text layersText, modeText;
	int amount;
	const string SP = "SINGLE PLAYER";
	const string MP = "TWO PLAYERS";
	GameObject gameSettings;
	bool wasPressed;

	const float MINX = -200, MINY = 50, MAXX = 600, MAXY = 550;

	// Use this for initialization
	void Start () {
		// Initialize the number of layers (default 18)
		layersText = GameObject.Find ("LayersText").GetComponent<Text> ();
		layersText.text = "18";
		amount = 18;

		// Intialize game mode text (default 1 player)
		modeText = GameObject.Find("ModeText").GetComponent<Text>();
		modeText.text = SP;

		// Search for the buttons
		MODE = GameObject.Find("MODEBUTTON").GetComponent<Button>();
		MORE = GameObject.Find("INCRBTN").GetComponent<Button>();
		LESS = GameObject.Find("DECRBTN").GetComponent<Button>();
		START = GameObject.Find("STARTGAME").GetComponent<Button>();
		QUIT = GameObject.Find("QUITGAME").GetComponent<Button>();

		MODE.onClick.AddListener( () => {toggleMode();} );
		MORE.onClick.AddListener( () => {modifyLayers(true);} );
		LESS.onClick.AddListener( () => {modifyLayers(false);} );
		QUIT.onClick.AddListener( () => {quitApp();});
		START.onClick.AddListener( () => {gameStart();});

		// Set certain game objects to not be destroyed
		gameSettings = GameObject.Find("GameSettings");
		DontDestroyOnLoad (gameSettings);
		DontDestroyOnLoad(GameObject.Find("SixenseInput"));

		// Misc initializations
		isMode = isMore = isLess = isStart = isQuit = wasPressed = false;
	}

	// Fixed frame update func
	void FixedUpdate(){
		// Test if hydra is connected
		if (!SixenseInput.IsBaseConnected (0)) {
			print ("SIXENSE DEVICE NOT CONNECTED");
		} else {
			// Obtain the position information for this and update accordingly
			Vector3 pos = SixenseInput.Controllers[1].Position;
			if (pos.y >= MINY + (MAXY - MINY) / 4 * 3) {
				highlightButton ("MODE");
			} else if (pos.y >= MINY + (MAXY - MINY) / 2) {
				if (pos.x >= MINX + (MAXX - MINX) / 2) {
					highlightButton ("MORE");
				} else {
					highlightButton ("LESS");
				}
			} else if (pos.y >= MINY + (MAXY - MINY) / 4) {
				highlightButton ("START");
			} else {
				highlightButton ("QUIT");
			}
			// Determine if there was a RT press
			if (SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
				wasPressed = true;
			}
			else{
				if (wasPressed) {
					wasPressed = true; // Prevent unwanted modifies
					if (isMode) {
						toggleMode ();
					} else if (isMore) {
						modifyLayers (true);
					} else if (isLess) {
						modifyLayers (false);
					} else if (isStart) {
						gameStart ();
					} else if (isQuit) {
						quitApp ();
					}
					wasPressed = false;
				}
			}
		}
	}
		
	// Toggle the game mode
	void toggleMode(){
		if (modeText.text.Equals (SP))
			modeText.text = MP;
		else
			modeText.text = SP;
	}

	// Modify the number of layers
	void modifyLayers(bool isIncrease){
		if (isIncrease) {
			amount++;
			if (amount > 50)
				amount = 50;
		} else {
			amount--;
			if (amount < 2)
				amount = 2;
		}
		layersText.text = "" + amount;
	}

	// Quit the application
	void quitApp(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit ();
		#endif
	}

	// Start the game
	void gameStart(){
		SettingsScript.height = amount;
		if (modeText.text.Equals(SP)){
			SettingsScript.players = 1;
		}
		else{
			SettingsScript.players = 2;
		}
		SceneManager.LoadScene ("JengaGame");
	}

	// Highlights a button
	void highlightButton(string btn){
		MODE.image.color = Color.white;
		MORE.image.color = Color.white;
		LESS.image.color = Color.white;
		START.image.color = Color.white;
		QUIT.image.color = Color.white;
		isMode = isMore = isLess = isStart = isQuit = false;
		switch (btn) {
		case "MODE":
			MODE.image.color = Color.yellow;
			isMode = true;
			break;
		case "MORE":
			MORE.image.color = Color.yellow;
			isMore = true;
			break;
		case "LESS":
			LESS.image.color = Color.yellow;
			isLess = true;
			break;
		case "START":
			START.image.color = Color.yellow;
			isStart = true;
			break;
		case "QUIT":
			QUIT.image.color = Color.yellow;
			isQuit = true;
			break;
		}
	}
}
