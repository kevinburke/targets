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
		Debug.Log ("Hello from the Start script.");
	}
	
	// Update is called once per frame
	void Update () {
	    if (count < 1) {
			Debug.Log("The first frame.");
		}
		count++;
		if (objectsVisible == 0) {
			target = GameObject.CreatePrimitive(PrimitiveType.Quad);
			target.renderer.material.color = new Color(217.0f/256.0f, 79.0f/256.0f, 83.0f/256.0f);
			target.transform.position = new Vector3(Random.Range (-2.0f, 2.0f), Random.Range (-0.5f, 2.0f), Random.Range (3.0f, 5.0f));
			Debug.Log (Camera.main.transform.position);
			target.transform.rotation =  Quaternion.LookRotation(target.transform.position - Camera.main.transform.position);
			target.SetActive(true);
			Debug.Log (Camera.main.transform.forward);
			Debug.Log(target.GetInstanceID());
			objectsVisible++;
		}
		if (count % 50 == 0) {
			Debug.Log (Camera.main.transform.rotation);
		}
	}
}
