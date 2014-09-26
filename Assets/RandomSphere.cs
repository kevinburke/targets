using UnityEngine;
using System.Collections;

public class RandomSphere {
    public static Vector3 PointOnSphere(float scale) {
		float x0 = Random.Range (-1f, 1f);
		float x1 = Random.Range (-1f, 1f);
		float x2 = Random.Range (-1f, 1f);
		float x3 = Random.Range (-1f, 1f);
		float squareSum = Mathf.Pow(x0, 2) + Mathf.Pow(x1, 2) + Mathf.Pow(x2, 2) + Mathf.Pow(x3, 2);
		while (squareSum >= 1) {
			x0 = Random.Range (-1f, 1f);
			x1 = Random.Range (-1f, 1f);
			x2 = Random.Range (-1f, 1f);
			x3 = Random.Range (-1f, 1f);
			squareSum = Mathf.Pow(x0, 2) + Mathf.Pow(x1, 2) + Mathf.Pow(x2, 2) + Mathf.Pow(x3, 2);
		}
		Vector3 s = new Vector3();
		s.x = 2f * (x1 * x3 + x0 * x2) / squareSum * scale;
		s.y = 2f * (x2 * x3 - x0 * x1) / squareSum * scale;
		s.z = (Mathf.Pow(x0, 2) + Mathf.Pow(x3, 2) - Mathf.Pow(x1, 2) - Mathf.Pow(x2, 2)) / squareSum * scale;
		return s;
	}
}
