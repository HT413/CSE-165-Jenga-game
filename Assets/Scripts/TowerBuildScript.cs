using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Diagnostics;

public class TowerBuildScript : MonoBehaviour {
	int height, players;
	Text playerText, fullStackText;
	Camera cam, cam2, camMenu;

	ArrayList initPositions;
	ArrayList initOrientations;

	Quaternion initCam1, initCam2;
	Vector3 initCamPos, initCam2Pos;

	Vector3 prevCamPos, prevCam2Pos;
	Quaternion prevCam1, prevCam2;

	LinkedList<Vector3> fingerList;
	LinkedList<Vector3> tempFingerList;

	ArrayList bricks;

	ArrayList positions;
	ArrayList orientations;

	ArrayList intermediatePositions;
	ArrayList intermediateOrientations;

	ArrayList tempPositions;
	ArrayList tempOrientations;

	Stopwatch clickTimer;
	bool isPush, isMenu, isPushing, isReplaying;
	Button UNDO, REDO, RESET, QUIT, CONT;
	bool isUndo, isRedo, isReset, isQuit, isCont;
	bool wasPressed;
	RawImage brickFrame;
	GameObject finger;
	Vector3 toCenter, toRight;

	Vector3 prevFinger;

	const float MINX = -200, MINY = 0, MAXX = 600, MAXY = 600, MINZ = -300, MAXZ = 150;

	Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 upVect = new Vector3(0.0f, 1.0f, 0.0f);
	Vector3 minorUp = new Vector3(0.0f, 0.05f, 0.0f);
	Vector3 minorDown = new Vector3(0.0f, -0.05f, 0.0f);

	static bool brickFell;
	static Stopwatch brickTimer;

	int turn, lastTurn;

	static bool isGameOver;

	static float brickTag = 0.0f;

	static bool wasGameOver;

	// Use this for initialization
	void Start () {
		turn = -1;
		lastTurn = -2;
		isPush = isMenu = wasPressed = isPushing = isGameOver = false;
		isUndo = isRedo = isReset = isQuit = isCont = false;
		brickFell = false;
		isReplaying = false;

		height = SettingsScript.height;
		players = SettingsScript.players;

		// Get the text for player turns and set it accordingly
		playerText = GameObject.Find ("playerTurn").GetComponent<Text> ();
		if (players == 1)
			playerText.text = "SINGLE PLAYER MODE";
		else
			playerText.text = "CURRENT TURN: PLAYER 1";

		fullStackText = GameObject.Find ("stackText").GetComponent<Text> ();

		// Get the cameras
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		cam2 = GameObject.Find ("Secondary Camera").GetComponent<Camera> ();
		camMenu = GameObject.Find ("Tertiary Camera").GetComponent<Camera> ();
		camMenu.gameObject.SetActive (false);
		initCam1 = cam.transform.rotation;
		initCamPos = cam.transform.position;
		initCam2 = cam2.transform.rotation;
		initCam2Pos = cam2.transform.position;

		// Build the tower
		positions = new ArrayList();
		orientations = new ArrayList();
		bricks = new ArrayList();
		initPositions = new ArrayList ();
		initOrientations = new ArrayList ();
		tempPositions = new ArrayList ();
		tempOrientations = new ArrayList ();
		for (int i = 0; i < height; i++) {
			float offX1 = Random.value * 0.04f - 0.02f;
			float offX2 = Random.value * 0.04f - 0.02f;
			float offX3 = Random.value * 0.04f - 0.02f;
			float offZ1 = Random.value * 0.04f - 0.02f;
			float offZ2 = Random.value * 0.04f - 0.02f;
			float offZ3 = Random.value * 0.04f - 0.02f;
			if (i % 2 == 0) {
				GameObject jengaBrick1 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick1.transform.position = new Vector3 (0.0f + offX1, 0.09f + ((float)i) * 0.18f, 0.0f + offZ1);
				GameObject jengaBrick2 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick2.transform.position = new Vector3 (0.0f + offX2, 0.09f + ((float)i) * 0.18f, -0.3f + offZ2);
				GameObject jengaBrick3 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick3.transform.position = new Vector3 (0.0f + offX3, 0.09f + ((float)i) * 0.18f, 0.3f + offZ3);
				// Add these bricks to the brick info lists
				bricks.Add(jengaBrick1);
				bricks.Add(jengaBrick2);
				bricks.Add(jengaBrick3);
				positions.Add (jengaBrick1.transform.position);
				positions.Add (jengaBrick2.transform.position);
				positions.Add (jengaBrick3.transform.position);
				orientations.Add (jengaBrick1.transform.rotation);
				orientations.Add (jengaBrick2.transform.rotation);
				orientations.Add (jengaBrick3.transform.rotation);
				initPositions.Add (jengaBrick1.transform.position);
				initPositions.Add (jengaBrick2.transform.position);
				initPositions.Add (jengaBrick3.transform.position);
				initOrientations.Add (jengaBrick1.transform.rotation);
				initOrientations.Add (jengaBrick2.transform.rotation);
				initOrientations.Add (jengaBrick3.transform.rotation);
			} else {
				GameObject jengaBrick1 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick1.transform.rotation = new Quaternion (Mathf.Sqrt(0.5f), 0.0f, Mathf.Sqrt(0.5f), 0.0f);
				jengaBrick1.transform.position = new Vector3 (0.0f + offX1, 0.09f + ((float)i) * 0.18f, 0.0f + offZ1);
				GameObject jengaBrick2 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick2.transform.rotation = new Quaternion (Mathf.Sqrt(0.5f), 0.0f, Mathf.Sqrt(0.5f), 0.0f);
				jengaBrick2.transform.position = new Vector3 (-0.3f + offX2, 0.09f + ((float)i) * 0.18f, 0.0f + offZ2);
				GameObject jengaBrick3 = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/jengaBlock.prefab", typeof(GameObject))) as GameObject;
				jengaBrick3.transform.rotation = new Quaternion (Mathf.Sqrt(0.5f), 0.0f, Mathf.Sqrt(0.5f), 0.0f);
				jengaBrick3.transform.position = new Vector3 (0.3f + offX3, 0.09f + ((float)i) * 0.18f, 0.0f + offZ3);
				// Add these bricks to the brick info lists
				bricks.Add(jengaBrick1);
				bricks.Add(jengaBrick2);
				bricks.Add(jengaBrick3);
				positions.Add (jengaBrick1.transform.position);
				positions.Add (jengaBrick2.transform.position);
				positions.Add (jengaBrick3.transform.position);
				orientations.Add (jengaBrick1.transform.rotation);
				orientations.Add (jengaBrick2.transform.rotation);
				orientations.Add (jengaBrick3.transform.rotation);
				initPositions.Add (jengaBrick1.transform.position);
				initPositions.Add (jengaBrick2.transform.position);
				initPositions.Add (jengaBrick3.transform.position);
				initOrientations.Add (jengaBrick1.transform.rotation);
				initOrientations.Add (jengaBrick2.transform.rotation);
				initOrientations.Add (jengaBrick3.transform.rotation);
			}
		}

		// Other intializations
		clickTimer = new Stopwatch();
		brickTimer = new Stopwatch();
		// Don't measure time yet
		clickTimer.Stop ();
		brickTimer.Stop ();

		// Get the buttons
		UNDO = GameObject.Find("undoBtn").GetComponent<Button>(); UNDO.gameObject.SetActive(false);
		REDO = GameObject.Find("redoBtn").GetComponent<Button>(); REDO.gameObject.SetActive(false);
		RESET = GameObject.Find("resetBtn").GetComponent<Button>(); RESET.gameObject.SetActive(false);
		QUIT = GameObject.Find("quitBtn").GetComponent<Button>(); QUIT.gameObject.SetActive(false);
		CONT = GameObject.Find("contBtn").GetComponent<Button>(); CONT.gameObject.SetActive(false);

		// Get the transparent frame
		brickFrame = GameObject.Find("frame").GetComponent<RawImage>();
		Vector3 ps = brickFrame.transform.position;
		brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);

		finger = null;

		toCenter = new Vector3(1.5f, 0.0f, 1.5f).normalized;
		toRight = new Vector3(-1.5f, 0.0f, 1.5f).normalized;
		isPushing = false;
	}
	
	// Fixed frame update func
	void Update () {
		// Check if game over
		if (isGameOver) {
			isPush = false;
			if (finger != null) {
				finger.SetActive (false);
				finger = null;
			}
			// Force update the brick info lists
			if (wasGameOver) {
				wasGameOver = false;
				brickTimer.Stop ();
				brickTimer.Reset ();
				for (int i = 0; i < height * 3; ++i) {
					positions [i] = tempPositions [i];
					orientations [i] = tempOrientations [i];
				}
				lastTurn = turn - 1; // Change the turn count
			}
		}

		// Check if a brick fell
		if (brickFell) {
			brickFell = false;
			// Start the timer if is hasn't started
			if (brickTimer.ElapsedMilliseconds == 0 && !isGameOver) {
				brickTimer.Start ();
				// Now switch turns
				turn++;
				if (!isReplaying) {
					// Save the state of the cameras and bricks
					prevCam1 = cam.transform.rotation;
					prevCamPos = cam.transform.position;
					prevCam2 = cam2.transform.rotation;
					prevCam2Pos = cam2.transform.position;
					fingerList = new LinkedList<Vector3> (tempFingerList);
					intermediatePositions = new ArrayList (tempPositions);
					intermediateOrientations = new ArrayList (tempOrientations);
				}
				if (players == 2) {
					if (turn % 2 == 0) {
						playerText.text = "CURRENT TURN: PLAYER 2";
					} else {
						playerText.text = "CURRENT TURN: PLAYER 1";
					}
				}
			}
		}
		// Check if "safe"
		if (brickTimer.ElapsedMilliseconds >= 6000) {
			// Reset the timer
			brickTimer.Stop ();
			brickTimer.Reset ();
			// Now copy over all the previously stored transformation data
			if (!isGameOver) {
				for (int i = 0; i < height * 3; ++i) {
					positions [i] = intermediatePositions [i];
					orientations [i] = intermediateOrientations [i];
				}
				lastTurn = turn - 1; // Change the turn count
			}
		}

		if (isReplaying) {
			Vector3 front = tempFingerList.First.Value;
			tempFingerList.RemoveFirst();
			finger.transform.position = front;
			if (tempFingerList.Count == 0) {
				finger.SetActive (false);
				finger = null;
				isReplaying = false;
			}
			return;
		}
		// Test if hydra is connected
		if (!SixenseInput.IsBaseConnected (0)) {
			print ("SIXENSE DEVICE NOT CONNECTED");
		} else {
			if (isMenu) {
				Vector3 pos = SixenseInput.Controllers [1].Position;
				// Disable the text objects and cameras
				playerText.gameObject.SetActive (false);
				fullStackText.gameObject.SetActive (false);
				cam.gameObject.SetActive (false);
				cam2.gameObject.SetActive (false);
				brickFrame.gameObject.SetActive (false);
				// Enable full-screen camera and the buttons
				camMenu.gameObject.SetActive (true);
				UNDO.gameObject.SetActive(true);
				REDO.gameObject.SetActive(true);
				RESET.gameObject.SetActive(true);
				QUIT.gameObject.SetActive(true);
				CONT.gameObject.SetActive(true);
				// Determine which button to highlight
				if (pos.y <= MINY + (MAXY - MINY) / 5) {
					highlightButton ("QUIT");
				} 
				else if (pos.y <= MINY + (MAXY - MINY) * 2 / 5) {
					highlightButton ("CONT");
				}
				else if (pos.y <= MINY + (MAXY - MINY) * 3 / 5) {
					highlightButton ("RESET");
				}
				else if (pos.y <= MINY + (MAXY - MINY) * 4 / 5) {
					highlightButton ("REDO");
				}
				else{
					highlightButton ("UNDO");
				}
				// Now determine if a button was pressed
				if (SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
					wasPressed = true;
				} else {
					if (wasPressed) {
						wasPressed = false;
						isMenu = false;
						// Back to game
						if (isCont) {
							isCont = false;
							CONT.image.color = Color.white;
							// Enable the text objects and cameras
							playerText.gameObject.SetActive (true);
							fullStackText.gameObject.SetActive (true);
							cam.gameObject.SetActive (true);
							cam2.gameObject.SetActive (true);
							brickFrame.gameObject.SetActive (true);
							// Disable full-screen camera and the buttons
							camMenu.gameObject.SetActive (false);
							UNDO.gameObject.SetActive (false);
							REDO.gameObject.SetActive (false);
							RESET.gameObject.SetActive (false);
							QUIT.gameObject.SetActive (false);
							CONT.gameObject.SetActive (false);
							return;
						}
						// Back to main menu
						if (isQuit) {
							SceneManager.LoadScene ("MainMenu");
						}
						// Reset the game
						if (isReset) {
							positions.Clear ();
							orientations.Clear ();
							// Deactive all current bricks and clear brick info lists
							for (int i = 0; i < height * 3; i++) {
								((GameObject)bricks [i]).SetActive (true);
								((GameObject)bricks [i]).transform.position = (Vector3)initPositions [i];
								positions.Add ((Vector3)initPositions [i]);
								((GameObject)bricks [i]).transform.rotation = (Quaternion)initOrientations [i];
								orientations.Add ((Quaternion)initOrientations [i]);
							}
							// Shift camera back to init settings
							cam.transform.position = initCamPos;
							cam.transform.rotation = initCam1;
							cam2.transform.position = initCam2Pos;
							cam2.transform.rotation = initCam2;

							toCenter = new Vector3(1.5f, 0.0f, 1.5f).normalized;
							toRight = new Vector3(-1.5f, 0.0f, 1.5f).normalized;
							if (finger != null) {
								finger.SetActive (false);
								finger = null;
							}

							// Back to game
							isReset = isMenu = isGameOver = false;
							CONT.image.color = Color.white;
							// Enable the text objects and cameras
							playerText.gameObject.SetActive (true);
							fullStackText.gameObject.SetActive (true);
							cam.gameObject.SetActive (true);
							cam2.gameObject.SetActive (true);
							brickFrame.gameObject.SetActive (true);
							brickFrame = GameObject.Find("frame").GetComponent<RawImage>();
							Vector3 ps = brickFrame.transform.position;
							brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);
							turn = -1; 
							lastTurn = -2;

							if (players == 2) {
								if (turn % 2 == 0) {
									playerText.text = "CURRENT TURN: PLAYER 2";
								} else {
									playerText.text = "CURRENT TURN: PLAYER 1";
								}
							}
							// Disable full-screen camera and the buttons
							camMenu.gameObject.SetActive (false);
							UNDO.gameObject.SetActive (false);
							REDO.gameObject.SetActive (false);
							RESET.gameObject.SetActive (false);
							QUIT.gameObject.SetActive (false);
							CONT.gameObject.SetActive (false);
							return;
						}
						// Undo last move
						if (isUndo) {
							isUndo = false;
							// Just go back to game if first move
							if (turn == -1 || turn == lastTurn) {
								// Enable the text objects and cameras
								playerText.gameObject.SetActive (true);
								playerText.text = "NO MOVES TO UNDO!";
								fullStackText.gameObject.SetActive (true);
								cam.gameObject.SetActive (true);
								cam2.gameObject.SetActive (true);
								brickFrame.gameObject.SetActive (true);
								brickFrame = GameObject.Find ("frame").GetComponent<RawImage> ();
								Vector3 ps = brickFrame.transform.position;
								brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);
								// Disable full-screen camera and the buttons
								camMenu.gameObject.SetActive (false);
								UNDO.gameObject.SetActive (false);
								REDO.gameObject.SetActive (false);
								RESET.gameObject.SetActive (false);
								QUIT.gameObject.SetActive (false);
								CONT.gameObject.SetActive (false);
								return;
							} else {
								// Undo last turn
								turn -= 1;
								for (int i = 0; i < height * 3; i++) {
									GameObject brick = (GameObject)bricks [i];
									brick.transform.position = (Vector3)positions [i];
									brick.transform.rotation = (Quaternion)orientations [i];
									brick.gameObject.GetComponent<Rigidbody> ().useGravity = true;
								}
								// Set the turn text accordingly
								if (players == 2) {
									if (turn % 2 == 0) {
										playerText.text = "CURRENT TURN: PLAYER 2";
									} else {
										playerText.text = "CURRENT TURN: PLAYER 1";
									}
								}
								// Undo game over state
								isGameOver = false;

								// Now go back to game
								// Enable the text objects and cameras
								playerText.gameObject.SetActive (true);
								fullStackText.gameObject.SetActive (true);
								cam.gameObject.SetActive (true);
								cam2.gameObject.SetActive (true);
								brickFrame.gameObject.SetActive (true);
								brickFrame = GameObject.Find("frame").GetComponent<RawImage>();
								Vector3 ps = brickFrame.transform.position;
								brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);
								// Disable full-screen camera and the buttons
								camMenu.gameObject.SetActive (false);
								UNDO.gameObject.SetActive (false);
								REDO.gameObject.SetActive (false);
								RESET.gameObject.SetActive (false);
								QUIT.gameObject.SetActive (false);
								CONT.gameObject.SetActive (false);
								return;
							}
						}
						// Replay last move
						if (isRedo) {
							isRedo = false;
							// Just go back to game if first move
							if (turn == -1) {
								// Enable the text objects and cameras
								playerText.gameObject.SetActive (true);
								playerText.text = "NO MOVES TO REPLAY!";
								fullStackText.gameObject.SetActive (true);
								cam.gameObject.SetActive (true);
								cam2.gameObject.SetActive (true);
								brickFrame.gameObject.SetActive (true);
								brickFrame = GameObject.Find ("frame").GetComponent<RawImage> ();
								Vector3 ps = brickFrame.transform.position;
								brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);
								// Disable full-screen camera and the buttons
								camMenu.gameObject.SetActive (false);
								UNDO.gameObject.SetActive (false);
								REDO.gameObject.SetActive (false);
								RESET.gameObject.SetActive (false);
								QUIT.gameObject.SetActive (false);
								CONT.gameObject.SetActive (false);
								return;
							} else {
								// Undo last turn
								turn -= 1;
								for (int i = 0; i < height * 3; i++) {
									GameObject brick = (GameObject)bricks [i];
									brick.transform.position = (Vector3)positions [i];
									brick.transform.rotation = (Quaternion)orientations [i];
									brick.gameObject.GetComponent<Rigidbody> ().useGravity = true;
								}
								// Set the turn text accordingly
								if (players == 2) {
									if (turn % 2 == 0) {
										playerText.text = "CURRENT TURN: PLAYER 2";
									} else {
										playerText.text = "CURRENT TURN: PLAYER 1";
									}
								}
								// Undo game over state
								isGameOver = false;
								isReplaying = true;
								finger = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/Fingertip.prefab", typeof(GameObject))) as GameObject;
								tempFingerList = new LinkedList<Vector3>(fingerList);

								// Now go back to game
								// Enable the text objects and cameras
								playerText.gameObject.SetActive (true);
								fullStackText.gameObject.SetActive (true);
								cam.gameObject.SetActive (true);
								cam2.gameObject.SetActive (true);
								cam.transform.position = prevCamPos;
								cam.transform.rotation = prevCam1;
								cam2.transform.position = prevCam2Pos;
								cam2.transform.rotation = prevCam2;
								brickFrame.gameObject.SetActive (true);
								brickFrame = GameObject.Find("frame").GetComponent<RawImage>();
								Vector3 ps = brickFrame.transform.position;
								brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, Screen.height * 0.165f, ps.z);
								// Disable full-screen camera and the buttons
								camMenu.gameObject.SetActive (false);
								UNDO.gameObject.SetActive (false);
								REDO.gameObject.SetActive (false);
								RESET.gameObject.SetActive (false);
								QUIT.gameObject.SetActive (false);
								CONT.gameObject.SetActive (false);
								return;
							}
						}
					}
				}
			} else {
				if (isPush) {
					if (isPushing) {
						// Determine if there's a button release
						if (!SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
							wasPressed = false;
							clickTimer.Stop ();
							long timePassed = clickTimer.ElapsedMilliseconds;
							clickTimer.Reset ();
							isPushing = false;
							// Player probably gave up on push. Back to normal.
							if (timePassed < 700) {
								isPush = isPushing = false;
								finger.SetActive (false);
								finger = null;
								return;
							}
							if (isGameOver) {
								isPush = isPushing = false;
								return;
							}
						}
						// Otherwise, push as normal
						Vector3 pos = SixenseInput.Controllers [1].Position;
						Vector3 dPos = pos - prevFinger;
						Vector3 fingerPos = finger.transform.position;
						Vector3 newFingerPos = new Vector3 (fingerPos.x - (dPos).x / (MAXX - MINX) * toRight.z / 7.0f,
							                       			fingerPos.y + (dPos).y / (MAXY - MINY) / 7.0f,
															fingerPos.z - (dPos).x / (MAXX - MINX) * toRight.z / 7.0f)
							+ toCenter * (dPos).z / (MAXZ - MINZ) * 3.0f;
						finger.transform.position = newFingerPos;
						prevFinger = pos;
						tempFingerList.AddLast (newFingerPos);
					} else {
						// Get the controller position
						Vector3 pos = SixenseInput.Controllers [1].Position;
						// The camera position
						Vector3 camPosition = cam.transform.position;
						// Create the "finger" if it's null
						if (finger == null) {
							finger = GameObject.Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Models/Fingertip.prefab", typeof(GameObject))) as GameObject;
							finger.transform.position = new Vector3 (camPosition.x * 0.6f, camPosition.y, camPosition.z * 0.6f);
						}
						Vector3 fingerPos = new Vector3 (camPosition.x * 0.6f - ((pos.x - MINX - (MAXX - MINX) / 2.0f) / (MAXX - MINX) * 0.6f * toRight).x,
							camPosition.y + (pos.y - MINY - (MAXY - MINY) / 2.0f) / (MAXY - MINY) * 0.5f,
							camPosition.z * 0.6f - ((pos.x - MINX - (MAXX - MINX) / 2.0f) / (MAXX - MINX) * 0.6f * toRight).z);
						finger.transform.position = fingerPos;
						UnityEngine.Debug.DrawRay (fingerPos, toCenter, Color.red);
						// Determine if there's a button press
						if (SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
							clickTimer.Reset ();
							clickTimer.Start ();
							if (!isPushing) {
								// Save temp state of bricks if not pushing
								tempPositions.Clear ();
								tempOrientations.Clear ();
								for (int i = 0; i < height * 3; ++i) {
									GameObject brick = (GameObject)bricks [i];
									tempPositions.Add (brick.transform.position);
									tempOrientations.Add (brick.transform.rotation);
								}
							}
							wasPressed = true;
							isPushing = true;
							prevFinger = pos;
							tempFingerList = new LinkedList<Vector3> ();
						}
					}
				} else {
					// Obtain position information for the right controller
					Vector3 pos = SixenseInput.Controllers [1].Position;

					// Determine camera operations
					// Rotate camera left
					if (pos.x <= MINX + (MAXX - MINX) / 6) {
						cam.transform.RotateAround (origin, upVect, 1.0f);
						cam2.transform.RotateAround (origin, upVect, 1.0f);
						toRight = Quaternion.Euler (0, 1, 0) * toRight;
						toCenter = Quaternion.Euler (0, 1, 0) * toCenter;
					} 
					// Rotate camera right
					else if (pos.x >= MAXX - (MAXX - MINX) / 6) {
						cam.transform.RotateAround (origin, upVect, -1.0f);
						cam2.transform.RotateAround (origin, upVect, -1.0f);
						toRight = Quaternion.Euler (0, -1, 0) * toRight;
						toCenter = Quaternion.Euler (0, -1, 0) * toCenter;
					}
					// Shift camera down
					if (pos.y <= MINY + (MAXY - MINY) / 6) {
						if (cam.transform.position.y >= 0.30f) {
							cam.transform.Translate (minorDown);
							Vector3 ps = brickFrame.transform.position;
							brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, ps.y - 0.00253f * Screen.width, ps.z);
						}
					}
					// Shift camera up
					else if (pos.y >= MAXY - (MAXY - MINY) / 6) {
						cam.transform.Translate (minorUp);
						Vector3 ps = brickFrame.transform.position;
						brickFrame.transform.position = new Vector3 (Screen.width * 0.8f, ps.y + 0.00253f * Screen.width, ps.z);
					}

					if (isGameOver) {
						if (players == 1) {
							playerText.text = "Game Over!";
						} else {
							if (turn % 2 == 0) {
								playerText.text = "Game Over! Player 2 wins!";
							} else {
								playerText.text = "Game Over! Player 1 wins!";
							}
						}
						if (SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
							wasPressed = true;
						} else { // Bumper unpressed or released
							if (wasPressed) {
								wasPressed = false;
								isMenu = true;
							}
						}
					}

					// Now determine if a button was pressed
					if (SixenseInput.Controllers [1].GetButton (SixenseButtons.BUMPER)) {
						if (wasPressed == false) {
							// Start the timer
							clickTimer.Reset ();
							clickTimer.Start ();
						}
						wasPressed = true;
					} else { // Bumper unpressed or released
						if (wasPressed && !isPushing) {
							wasPressed = false;
							clickTimer.Stop ();
							long timePassed = clickTimer.ElapsedMilliseconds;
							clickTimer.Reset ();
							if (timePassed < 700) {
								isPush = true;
								isMenu = false;
							} else {
								isPush = false;
								isMenu = true;
							}
						}
					}
				}
			}
		}
	}

	// Highlights a button
	void highlightButton(string btn){
		UNDO.image.color = Color.white;
		REDO.image.color = Color.white;
		RESET.image.color = Color.white;
		QUIT.image.color = Color.white;
		CONT.image.color = Color.white;
		isUndo = isRedo = isReset = isQuit = isCont = false;
		switch (btn) {
			case "UNDO":
				UNDO.image.color = Color.yellow;
				isUndo = true;
				break;
			case "REDO":
				REDO.image.color = Color.yellow;
				isRedo = true;
				break;
			case "RESET":
				RESET.image.color = Color.yellow;
				isReset = true;
				break;
			case "QUIT":
				QUIT.image.color = Color.yellow;
				isQuit = true;
				break;
			case "CONT":
				CONT.image.color = Color.yellow;
				isCont = true;
				break;
		}
	}

	public static void brickFallen(){
		brickFell = true;
		if (brickTimer.ElapsedMilliseconds > 0 && wasGameOver == false) {
			isGameOver = wasGameOver = true;
			brickTimer.Stop ();
		}
	}

	public static float getIndex(){
		brickTag -= 1.0f;
		return brickTag;
	}
}
