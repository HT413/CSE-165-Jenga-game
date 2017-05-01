using UnityEngine;
using System.Collections;

public class BrickScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 myPos = gameObject.transform.position;
		if (Mathf.Abs (myPos.x) >= 0.83f || Mathf.Abs (myPos.z) >= 0.83f) {
			float index = TowerBuildScript.getIndex ();
			gameObject.transform.position = new Vector3 (0.0f, index, 0.0f);
			GetComponent<Rigidbody> ().useGravity = false;
			TowerBuildScript.brickFallen ();
		}
	}
}
