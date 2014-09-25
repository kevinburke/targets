using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	private int count;
	private int objectsVisible;
	private GameObject target;
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
		t.transform.position = new Vector3(Random.Range (-2.0f, 2.0f), Random.Range (-0.5f, 2.0f), Random.Range (3.0f, 5.0f));
		Debug.Log (cameraPosition);
		
		// orient the quad so it's facing at the user
		t.transform.rotation = Quaternion.LookRotation(t.transform.position - cameraPosition);
		
		// make it visible
		t.SetActive(true);
		return t;
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		if (objectsVisible == 0) {
			target = createTarget(Camera.main.transform.position);
			objectsVisible++;
		}

		// Check if user is pressing the spacebar. Otherwise we don't care what they're looking at.
		if (Input.GetKeyDown ("space")) {
			RaycastHit hit;
			if (lookingAtTarget(Camera.main.transform, out hit)) {
				target.SetActive (false);
				target = createTarget(Camera.main.transform.position);
				Debug.Log ("a hit!");
			} else {
				if (count % 50 == 0) {
					Debug.Log ("no hit");
				}
			}
		}

		if (count % 50 == 0) {
			Debug.Log (Camera.main.transform.rotation);
		}
	}
}
