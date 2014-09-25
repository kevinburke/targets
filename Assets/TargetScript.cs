using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	private int count;
	private int objectsVisible;
	private GameObject target;

	// Use this for initialization
	void Start () {
		count = 0;
		objectsVisible = 0;
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		if (objectsVisible == 0) {
			target = GameObject.CreatePrimitive(PrimitiveType.Quad);
			target.renderer.material.color = new Color(217.0f/256.0f, 79.0f/256.0f, 83.0f/256.0f);
			target.transform.position = new Vector3(Random.Range (-2.0f, 2.0f), Random.Range (-0.5f, 2.0f), Random.Range (3.0f, 5.0f));
			Debug.Log (Camera.main.transform.position);

			// orient the quad so it's facing at the user
			target.transform.rotation = Quaternion.LookRotation(target.transform.position - Camera.main.transform.position);

			// make it visible
			target.SetActive(true);
			objectsVisible++;
		}
		// check if user is looking at the object
		float length = 10.0f;
		RaycastHit hit;
		Vector3 rayDirection = Camera.main.transform.TransformDirection (Vector3.forward);
		Vector3 rayStart = Camera.main.transform.position + rayDirection;
		Debug.DrawRay (rayStart, rayDirection * length, Color.green);
		if (Physics.Raycast (rayStart, rayDirection, out hit, length)) {
			Debug.Log (hit);
			Debug.Log ("a hit!");
		} else {
			if (count % 50 == 0) {
				Debug.Log ("no hit");
			}
		}
		if (count % 50 == 0) {
			Debug.Log (Camera.main.transform.rotation);
		}
	}
}
