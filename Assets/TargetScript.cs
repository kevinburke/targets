using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

    private int gamesPlayed = 0;
    private int totalGames = 10;

	private int count;
	private int objectsVisible;
	private int distance = 7;
	private GameObject target;
	// lazy way to convert hex color to RGB
	private Color red = new Color(217.0f/256.0f, 79.0f/256.0f, 83.0f/256.0f);

	// Use this for initialization
	void Start () {
		count = 0;
		objectsVisible = 0;
	}

	// Logic to determine whether the user is currently looking at the target.
	// If true, hit will store the results of the RaycastHit.
	bool lookingAtTarget(Transform cameraTransform, out RaycastHit hit) {
		float length = 10.0f;
		Vector3 rayDirection = cameraTransform.TransformDirection (Vector3.forward);
		Vector3 rayStart = cameraTransform.position + rayDirection;
		Debug.DrawRay (rayStart, rayDirection * length, Color.green);
		return Physics.Raycast(rayStart, rayDirection, out hit, length);
	}

	GameObject createTarget(Vector3 cameraPosition) {
		GameObject t = GameObject.CreatePrimitive(PrimitiveType.Quad);
		t.renderer.material.color = red;
		Vector3 s = RandomSphere.PointOnSphere (distance);
		// reject points which are (roughly) outside the FOV
		while(s.x < -2.5 || s.x > 2.5 || s.y < -1.5 || s.y > 2.5 || s.z <= 0) {
			s = RandomSphere.PointOnSphere (distance);
		}
		Debug.Log (s);
		t.transform.position = s;
		Debug.Log (cameraPosition);
		
		// orient the quad so it's facing at the user
		t.transform.rotation = Quaternion.LookRotation(t.transform.position - cameraPosition);
		GameObject f = new GameObject();
		//f.gameObject.AddComponent<MeshRenderer> ();
		f.AddComponent<TextMesh>();
		f.transform.rotation = t.transform.rotation;
		f.transform.position = t.transform.position + new Vector3(-0.45f, 0.15f, 0);

		f.GetComponent<TextMesh>().fontSize = 16;
		f.GetComponent<TextMesh>().color = Color.white;
		f.GetComponent<TextMesh>().characterSize = 0.2f;
		Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		f.GetComponent<TextMesh>().font = font;
		f.GetComponent<TextMesh>().renderer.material = font.material;
		f.GetComponent<TextMesh>().text = "1000";
		f.SetActive (true);
		return t;
	}
	
	// Update is called once per frame
	void Update () {

        if (state == STATE_INIT) {
            drawRecenterDialog();
            state = STATE_RECENTER_DIALOG;
        } else if (state == STATE_RECENTER_DIALOG && Input.anyKeyDown()) {
            defaultPosition = getCurrentCameraPosition();
            state = STATE_INSTRUCTIONS;
            drawInstructions();
        } else if (state == STATE_INSTRUCTIONS && Input.anyKeyDown()) {
            state = STATE_SINGLE_INPUT_GAME;
            drawSingleInputGame();
        } else if (state == STATE_SINGLE_INPUT_GAME) {
            // in game mode.
            if (Input.GetKeyDown("space")) {
                target = getTarget();
                if (target == TARGET_GREEN_BUTTON) {
                    startTimer();
                } else if (target == TARGET_RED_BUTTON) {
                    stopTimer();
                } else {
                    if (closeToTarget()) {
                        registerMiss();
                    }
                }
            }
            gamesPlayed++;
            if (gamesPlayed < 5) {
                drawSingleInputGame();
            } else {
                state = STATE_MULTI_INPUT_GAME;
                drawMultiInputGame();
            }
        } else {
            // multi input game
        }

		//count++;
		//if (objectsVisible == 0) {
			//target = createTarget(Camera.main.transform.position);
			//objectsVisible++;
		//}

		//// Check if user is pressing the spacebar. Otherwise we don't care what they're looking at.
		//if (Input.GetKeyDown ("space")) {
			//RaycastHit hit;
			//if (lookingAtTarget(Camera.main.transform, out hit)) {
				//GameObject targetCandidate = createTarget(Camera.main.transform.position);
				//// Make sure the new candidate is far enough away
				//while (Vector3.Distance (targetCandidate.transform.position, target.transform.position) < 0.4) {
					//targetCandidate = createTarget(Camera.main.transform.position);
				//}
				//// Not sure this activate dance is necessary
				//target.SetActive (false);
				//target = targetCandidate;
				//Destroy (targetCandidate);
				//target.SetActive (true);
				//Debug.Log ("a hit!");
			//} else {
				//if (count % 50 == 0) {
					//Debug.Log ("no hit");
				//}
			//}
		//}
	}
}
