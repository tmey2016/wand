using UnityEngine;
using System.Collections;

public class Wand : MonoBehaviour {

	/**
	 * Get number for the quadrant the wand is currently in
	 * relative to screen position
	 * 1 2 
	 * 3 4
	 */
	public int Quadrant() {
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
		if (viewportPosition.y >= 0.5f && viewportPosition.y <= 1.0f) {
			if (viewportPosition.x < 0.5f && viewportPosition.x >= 0.0f) {
				return 1;
			} else if (viewportPosition.x >= 0.5f && viewportPosition.x <= 1.0f) {
				return 2;
			}

		} else if (viewportPosition.y >= 0.0f && viewportPosition.y < 0.5f) {
			if (viewportPosition.x < 0.5f && viewportPosition.x >= 0.0f) {
				return 3;
			} else if (viewportPosition.x >= 0.5f && viewportPosition.x <= 1.0f) {
				return 4;
			}
		}

		return 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (Quadrant());
	}
}
