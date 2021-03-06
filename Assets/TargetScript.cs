﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;
using RestSharp;

public class Metric {
    public float time;
    public bool hit;
    public Quaternion cameraRotation;
	public Vector3 greenTargetPosition;
	public Vector3 redTargetPosition;
	public float targetSize;

	public Metric(float t, bool h, Quaternion cr, Vector3 gtp, Vector3 rtp, float ts) {
        time = t;
        hit = h;
        cameraRotation = cr;
		redTargetPosition = rtp;
		greenTargetPosition = gtp;
		targetSize = ts;
    }
}

public class MetricsResponse {
	public bool success;
	public int status;
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
	private GameObject redTarget;
	private GameObject recenterDialog;
	private GameObject instructions;
	private Quaternion restingRotation;
	System.Random rnd;
	float size;
	
	private GameObject greenTarget;

	// lazy way to convert hex color to RGB. colors taken from twitter bootstrap
	private Color red = new Color(217.0f/256.0f, 79.0f/256.0f, 83.0f/256.0f);
	private Color green = new Color(92.0f/256.0f, 184.0f/256.0f, 92.0f/256.0f);


	// Use this for initialization
	void Start () {
		//count = 0;
		//objectsVisible = 0;
        metrics = new List<Metric>();
        state = State.HEALTH_WARNING;
		rnd = new System.Random ();
		List<float> targetSizes = new List<float>();
		targetSizes.Add (0.25f);
		targetSizes.Add (0.45f);
		targetSizes.Add (0.75f);
		targetSizes.Add (1f);
		int idx = rnd.Next (targetSizes.Count);
		size = targetSizes[idx];
		Debug.Log ("target size is ");
		Debug.Log (size);
		Debug.Log("Warming up...");
		// GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
	}

	// Logic to determine whether the user is currently looking at the target.
	// If true, hit will store the results of the RaycastHit.
	bool lookingAtSomeTarget(Quaternion cameraTransform, out RaycastHit hit) {
		float length = 20.0f;
		Vector3 rayDirection = cameraTransform * Vector3.forward;
		Vector3 rayStart = Vector3.zero;
		Debug.DrawRay(rayStart, rayDirection * length, Color.green, 1000);
		return Physics.Raycast(rayStart, rayDirection, out hit, length);
	}


    void drawRecenterDialog(Vector3 cameraPosition) {
		// The best practice is to draw text directly to the screen.
		// I couldn't actually get this to work. Posting on the forums suggested
		// a third-party VR GUI library. 

		// Maybe one day I will become a good programmer or learn more about Unity
		// but for the moment I am going to just draw the text in the world so I don't
		// give up on the project.
        string loading = "Get comfy and then\npress any key.";
		recenterDialog = new GameObject();
		recenterDialog.AddComponent<TextMesh>();
		recenterDialog.transform.position = new Vector3 (-1f, 0, 8);
		recenterDialog.transform.rotation = Quaternion.LookRotation(recenterDialog.transform.position - cameraPosition);
		//f.transform.position = t.transform.position + new Vector3(-0.45f, 0.15f, 0);
		
		recenterDialog.GetComponent<TextMesh>().fontSize = 24;
		recenterDialog.GetComponent<TextMesh>().color = Color.white;
		recenterDialog.GetComponent<TextMesh>().characterSize = 0.2f;
		Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		recenterDialog.GetComponent<TextMesh>().font = font;
		recenterDialog.GetComponent<TextMesh>().renderer.material = font.material;
		recenterDialog.GetComponent<TextMesh>().text = loading;
		recenterDialog.GetComponent<TextMesh>().alignment = TextAlignment.Center;
		recenterDialog.SetActive (true);
    }

    void clearRecenterDialog() {
		recenterDialog.SetActive (false);
    }

    void drawInstructions(Quaternion cameraPosition) {
		string instructionsText = "Look at the green square and press spacebar.\nThen look at the red square and press spacebar.";
		instructions = new GameObject();
		instructions.AddComponent<TextMesh>();
		instructions.transform.position = restingRotation * Vector3.forward;
		instructions.transform.rotation = cameraPosition;
		//f.transform.position = t.transform.position + new Vector3(-0.45f, 0.15f, 0);

		instructions.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleCenter;
		instructions.GetComponent<TextMesh> ().offsetZ = 5;
		instructions.GetComponent<TextMesh>().fontSize = 16;
		instructions.GetComponent<TextMesh>().color = Color.white;
		instructions.GetComponent<TextMesh>().characterSize = 0.2f;
		Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		instructions.GetComponent<TextMesh>().font = font;
		instructions.GetComponent<TextMesh>().renderer.material = font.material;
		instructions.GetComponent<TextMesh>().text = instructionsText;
		instructions.GetComponent<TextMesh>().alignment = TextAlignment.Center;
		instructions.SetActive (true);

    }

    void clearInstructions() {
		instructions.SetActive (false);
    }

	/*
	 * find a point on the sphere that's roughly in the user's FOV
	 * lookDirection should be a vector with the same distance as the sphere radius
	 */
	private Vector3 getPointOnSphere(int sphereRadius, Vector3 lookDirection){
		Vector3 s = RandomSphere.PointOnSphere (sphereRadius);
		// reject points which are (roughly) outside the FOV
		Vector3 diff = s - lookDirection;
		// XXX these values should depend on the sphere radius & correspond to angles
		count = 0;

		while(count < 50 && (Vector3.Angle (s, lookDirection) > 30)) {
			s = RandomSphere.PointOnSphere (sphereRadius);
			diff = s.normalized - lookDirection;
			count++;
		}
		return s;
	}

	/*
	 * Create a target at a random position in front of the user
	 */
	GameObject createTarget(Quaternion startRotation) {
		GameObject t = GameObject.CreatePrimitive(PrimitiveType.Quad);
		t.transform.localScale = new Vector3 (3, 3, 1);
		//t.renderer.material.color = red;

		Vector3 s = getPointOnSphere (distance, (startRotation * Vector3.forward)*distance);
		t.transform.position = s;
		
		// orient the quad so it's facing at the user
		t.transform.rotation = Quaternion.LookRotation(t.transform.position - (startRotation * Vector3.forward));
		return t;
	}

    void drawSingleInputGame(Quaternion startRotation, float targetSize) {
		Debug.Log ("Drawing single input game.");
		greenTarget = createTarget(startRotation);
		greenTarget.renderer.material.color = green;
		greenTarget.transform.localScale = new Vector3 (3, 3, 1);

		redTarget = GameObject.CreatePrimitive (PrimitiveType.Quad);

		redTarget.transform.localScale = new Vector3 (size, size, 1);
		redTarget.renderer.material.color = red;

		// XXX surely there is a better way to write this. Put the red square randomly to 
		// the left or the right of the green target 
		int r = rnd.Next(0, 2);
		int arc;
		if (r == 0) {
			arc = -5;
		} else {
			arc = 5;
		}
		redTarget.transform.position = rotateByArc (greenTarget.transform.position, arc);
		redTarget.transform.rotation = Quaternion.LookRotation (redTarget.transform.position - (startRotation * Vector3.forward));
    }

    void drawMultiInputGame() {

    }

	/*
	 * Travel along the circumference of a circle
	 * 
	 * Via http://stackoverflow.com/a/25401122/329700
	 */
	Vector3 rotateByArc(Vector3 existingTarget, float arc) {
		// camera is at 0, 0, 0
		float radius = Vector3.Distance (existingTarget, Vector3.zero);
		float angle = arc / radius;
		return rotateByRadians(existingTarget, angle);
	}

	public Vector3 rotateByRadians(Vector3 existingTarget, float angle)
	{
		//Move calculation to 0,0
		Vector3 v = existingTarget - Vector3.zero;
		
		//rotate x and z
		float x = v.x * Mathf.Cos(angle) + v.z * Mathf.Sin(angle);
		float z = v.z * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);
		
		//move back to center
		return new Vector3(x, existingTarget.y, z) + Vector3.zero;
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

    bool matches(RaycastHit hit, GameObject t) {
		return hit.collider.gameObject.GetInstanceID () == t.GetInstanceID ();
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

    void displayGameOver() {
		instructions.SetActive (true);
		instructions.GetComponent<TextMesh> ().text = "Game over! Thanks for playing!";
	}
	
	void publishMetrics(List<Metric> metrics) {
		// MemoryStream stream1 = new MemoryStream();
		string output = JsonConvert.SerializeObject (metrics, new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
		// Xxx don't have this block everything
		var client = new RestClient("https://www.twentymilliseconds.com");
		var request = new RestRequest(Method.POST);
		request.RequestFormat = DataFormat.Json;
		request.Resource = "api/targets/v1/metrics";
		request.AddParameter("application/json", output, ParameterType.RequestBody);
		// GlobalProxySelection.Select = new WebProxy("127.0.0.1", 8888);
		// XXX security is hard, let's go shopping
		ServicePointManager.ServerCertificateValidationCallback +=
			(sender, certificate, chain, sslPolicyErrors) => true;
		IRestResponse<MetricsResponse> resp = client.Execute<MetricsResponse>(request);
    }

	void clearTargets() {
		if (greenTarget) {
			greenTarget.SetActive (false);
		}
		if (redTarget) {
			redTarget.SetActive (false);
		}
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
			Debug.Log("Clearing instructions. Moving to single state game.");
			clearInstructions();
            state = State.SINGLE_INPUT_GAME;

            drawSingleInputGame(restingRotation, size);
        } else if (state == State.SINGLE_INPUT_GAME) {
            // in game mode.
            if (Input.GetKeyDown("space")) {
				bool clearTarget = false;
                GameObject target = getTarget();
				RaycastHit hit;
				OVRPose ovp = OVRManager.display.GetHeadPose();
				if (lookingAtSomeTarget(ovp.orientation, out hit)) {
	                if (matches (hit, greenTarget)) {
						Debug.Log ("Matched Green button.");
	                    startTime = Time.time;
	                } else if (matches(hit, redTarget)) {
						Debug.Log ("Matched Red button");
	                    float timeElapsed = Time.time - startTime;
	                    // replace null with cameraPosition
	                    Metric m = new Metric(timeElapsed, true, Quaternion.identity, greenTarget.transform.position, redTarget.transform.position, size);
	                    metrics.Add(m);
						clearTarget = true;
	                } 
				} else {
					// You only get one chance at each target :(
					Debug.Log ("Missed!");
	                float timeElapsed = Time.time - startTime;
	                Metric m = new Metric(timeElapsed, false, ovp.orientation, greenTarget.transform.position, redTarget.transform.position, size);
	                metrics.Add(m);	
					clearTarget = true;
	            }

				if (clearTarget) {
					clearTargets();

					gamesPlayed++;
					if (gamesPlayed < 5) {
						drawSingleInputGame(restingRotation, size);
					} else {
						publishMetrics(metrics);
						displayGameOver();
					}
					/* XXX
					 * else {
						state = State.MULTI_INPUT_GAME;
						drawMultiInputGame();
					}*/
				}
			}
			
		} else if (state == State.MULTI_INPUT_GAME) {
            if (Input.GetKeyDown("space")) {
                /*GameObject target = getTarget();
				RaycastHit hit;
                if (matches(hit, Target.GREEN_BUTTON)) {
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

                        state = State.GAME_OVER;
                    }
                } */
            }
        } else {
            if (count % 5000 == 0) {
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
