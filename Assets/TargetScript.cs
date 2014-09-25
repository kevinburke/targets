using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	private int count;
	private int objectsVisible;

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
			GameObject target = GameObject.CreatePrimitive(PrimitiveType.Quad); 
			target.SetActive(true);
			target.transform.position = new Vector3(-5.0f, 0.64f, 5.0f);
			Debug.Log(target.GetInstanceID());
			objectsVisible++;
		}
	}
}
