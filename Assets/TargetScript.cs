using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Metric {
    public float time;
    public bool hit;
    public float[] cameraRotation;
    public string target;

    public Metric(float t, bool h, float[] cr, string target_) {
        time = t;
        hit = h;
        cameraRotation = cr;
        target = target_;
    }
}

public class TargetScript : MonoBehaviour {

    private enum State {
        HEALTH_WARNING,
        RECENTER_DIALOG,
        INSTRUCTIONS,
        SINGLE_INPUT_GAME,
        MULTI_INPUT_GAME,
        GAME_OVER,
    };

    private enum Target {
        GREEN_BUTTON,
        RED_BUTTON,
        OTHER_BUTTON,
        OTHER,
    }

	OVRGUI GUIControl;
    private State state;
    private List<Metric> metrics;
	private GameObject      GUIRenderObject  = null;

    private int gamesPlayed = 0;

    private float startTime;
	private int count;
	private int objectsVisible;
	private int distance = 7;
	private GameObject target;
	private GameObject recenterDialog;
	private GameObject instructions;
	private Quaternion restingRotation;
	// lazy way to convert hex color to RGB. colors taken from twitter bootstrap
	private Color red = new Color(217.0f/256.0f, 79.0f/256.0f, 83.0f/256.0f);
	private Color green = new Color(92.0f/256.0f, 184.0f/256.0f, 92.0f/256.0f);


	// Use this for initialization
	void Start () {
		//count = 0;
		//objectsVisible = 0;
        metrics = new List<Metric>();
        state = State.HEALTH_WARNING;
        Debug.Log("Warming up...");
		// GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
	}

	// Logic to determine whether the user is currently looking at the target.
	// If true, hit will store the results of the RaycastHit.
	bool lookingAtTarget(Transform cameraTransform, out RaycastHit hit) {
		float length = 10.0f;
		Vector3 rayDirection = cameraTransform.TransformDirection (Vector3.forward);
		Vector3 rayStart = cameraTransform.position + rayDirection;
		Debug.DrawRay(rayStart, rayDirection * length, Color.green);
		return Physics.Raycast(rayStart, rayDirection, out hit, length);
	}

	GameObject createTarget(Vector3 cameraPosition) {
		GameObject t = GameObject.CreatePrimitive(PrimitiveType.Quad);
		t.transform.localScale = new Vector3 (3, 3, 1);
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

    void drawRecenterDialog(Vector3 cameraPosition) {
		// The best practice is to draw text directly to the screen.
		// I couldn't actually get this to work. Posting on the forums suggested
		// a third-party VR GUI library. 

		// Maybe one day I will become a good programmer or learn more about Unity
		// but for the moment I am going to just draw the text in the world so I don't
		// give up on the project.
		Debug.Log("Drawing Recenter dialog.");
        string loading = "Get comfy and then\npress any key.";
		recenterDialog = new GameObject();
		recenterDialog.AddComponent<TextMesh>();
		recenterDialog.transform.position = new Vector3 (-1f, 0, 4);
		recenterDialog.transform.rotation = Quaternion.LookRotation(recenterDialog.transform.position - cameraPosition);
		//f.transform.position = t.transform.position + new Vector3(-0.45f, 0.15f, 0);
		
		recenterDialog.GetComponent<TextMesh>().fontSize = 16;
		recenterDialog.GetComponent<TextMesh>().color = Color.white;
		recenterDialog.GetComponent<TextMesh>().characterSize = 0.2f;
		Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		recenterDialog.GetComponent<TextMesh>().font = font;
		recenterDialog.GetComponent<TextMesh>().renderer.material = font.material;
		recenterDialog.GetComponent<TextMesh>().text = loading;
		recenterDialog.GetComponent<TextMesh>().alignment = TextAlignment.Center;
		recenterDialog.SetActive (true);
		Debug.Log("Stereo Box should be visible.");
    }

    void clearRecenterDialog() {
        Debug.Log("Clearing recenter dialog.");
		recenterDialog.SetActive (false);
    }

    void drawInstructions(Quaternion cameraPosition) {
        Debug.Log("Drawing instructions.");
		string instructionsText = "Look at the green square and\nthen look at the red square";
		instructions = new GameObject();
		instructions.AddComponent<TextMesh>();
		instructions.transform.position = restingRotation * Vector3.forward;
		instructions.transform.rotation = cameraPosition;
		//f.transform.position = t.transform.position + new Vector3(-0.45f, 0.15f, 0);
		
		instructions.GetComponent<TextMesh>().fontSize = 16;
		instructions.GetComponent<TextMesh>().color = Color.white;
		instructions.GetComponent<TextMesh>().characterSize = 0.2f;
		Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		instructions.GetComponent<TextMesh>().font = font;
		instructions.GetComponent<TextMesh>().renderer.material = font.material;
		instructions.GetComponent<TextMesh>().text = instructionsText;
		instructions.GetComponent<TextMesh>().alignment = TextAlignment.Center;
		instructions.SetActive (true);
		Debug.Log("Stereo Box should be visible.");

    }

    void clearInstructions() {
        Debug.Log("Clearing instructions.");
    }

    void drawSingleInputGame() {

    }

    void drawMultiInputGame() {

    }

    GameObject getTarget() {
        return new GameObject();
    }

    Vector3 getCurrentCameraPosition() {
        return new Vector3(0, 0, 0);
    }

    void registerHit() {

    }

    void registerMiss() {

    }

    bool matches(GameObject hit, Target t) {
        return true;
    }

    bool isDesiredButton() {
        return true;
    }

    void registerButtonMiss() {

    }

    void updateDesiredButton() {

    }

    void updateUITarget() {

    }

    bool gameOver() {
        return false;
    }

    void publishMetrics(List<Metric> metrics) {

    }
	
	// Update is called once per frame
	void Update () {
        count++;
		if (state == State.HEALTH_WARNING) {
			drawRecenterDialog(new Vector3(0, 0, 0));
			state = State.RECENTER_DIALOG;
		} else if (state == State.RECENTER_DIALOG 
            // ignore keypresses while health & safety warning is visible 
            && Input.anyKeyDown
        ) {
			Debug.Log ("Clearing recenter dialog.");
			OVRPose ovp = OVRManager.display.GetHeadPose();
            restingRotation = ovp.orientation;
            state = State.INSTRUCTIONS;
            clearRecenterDialog();
            drawInstructions(restingRotation);
        } else if (state == State.INSTRUCTIONS && Input.anyKeyDown) {
            state = State.SINGLE_INPUT_GAME;
            drawSingleInputGame();
        } else if (state == State.SINGLE_INPUT_GAME) {
            // in game mode.
            if (Input.GetKeyDown("space")) {
                GameObject target = getTarget();
                if (matches(target, Target.GREEN_BUTTON)) {
                    startTime = Time.time;
                } else if (matches(target, Target.RED_BUTTON)) {
                    float timeElapsed = Time.time - startTime;
                    // replace null with cameraPosition
                    Metric m = new Metric(timeElapsed, true, null, "");
                    metrics.Add(m);
                } else {
                    float timeElapsed = Time.time - startTime;
                    Metric m = new Metric(timeElapsed, false, null, "");
                    metrics.Add(m);
                }
            }
            gamesPlayed++;
            if (gamesPlayed < 5) {
                drawSingleInputGame();
            } else {
                state = State.MULTI_INPUT_GAME;
                drawMultiInputGame();
            }
        } else if (state == State.MULTI_INPUT_GAME) {
            if (Input.GetKeyDown("space")) {
                GameObject target = getTarget();
                if (matches(target, Target.GREEN_BUTTON)) {
                    startTime = Time.time;
                } else {
                    if (isDesiredButton()) {
                        registerHit();
                    } else {
                        registerButtonMiss();
                    }
                    updateDesiredButton();
                    updateUITarget();
                    float timeElapsed = Time.time - startTime;
                    Metric m = new Metric(timeElapsed, false, null, "");
                    metrics.Add(m);
                    startTime = Time.time;
                    if (gameOver()) {
                        publishMetrics(metrics);
                        state = State.GAME_OVER;
                    }
                } 
            }
        } else {
            if (count % 400 == 0) {
                Debug.Log("Holding pattern...");
                Debug.Log(state);
            }
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
