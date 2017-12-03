using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Generate : MonoBehaviour
{
	/** Public variables **/

	public GameObject spaceObject;			/* Object to instantiate when there is a collision */
	public int N = 15;						/* Number of sample points along extents */
	public float extentsX = 2.5f;			/* Extents from origin in both positive and negative directions */
	public float extentsY = 2.5f;			/* Extents from origin in both positive and negative directions */

	/** Private variables **/

	private Transform _transform;			/* Transform for the robot */
	private List<Collider2D> _obstacles; 	/* Obstacles we can collide with */
	private Collider2D _collider; 			/* Collider for obstacles */
	private Transform _configParent;		/* Parent under which to insert new spaceObjects */

	/** Private functions **/

	void Awake ()
	{
		// Acquire the robot's transform component
		_transform = GetComponent<Transform> ();
		if (_transform == null)
			Debug.Log ("Unable to acquire transform component");
		_collider = GetComponent<Collider2D> ();

		// Acquire the list of obstacles
		GameObject[] gos = GameObject.FindGameObjectsWithTag ("Obstacles");
		if (gos == null)
			Debug.Log ("Unable to acquire obstacles");
		_obstacles = new List<Collider2D> ();
		foreach (GameObject go in gos) {
			Collider2D coll = go.GetComponent<Collider2D> ();
			if (coll)
				_obstacles.Add (coll);
		}

		// Acquire the configuration space parent
		GameObject config = GameObject.FindGameObjectWithTag ("ConfigSpace");
		if (config == null)
			Debug.Log ("Unable to acquire configuration space");
		_configParent = config.GetComponent<Transform> ();
		if (_configParent == null)
			Debug.Log ("Unable to acquire Transform for config");

		// Start the generation routine
		StartCoroutine (GenerateConfigSpace ());
	}

	IEnumerator GenerateConfigSpace ()
	{
		bool[,,] obsSpace = new bool[N, N, N];
		// Step amounts for x, y, and theta
		float deltaX = 2.0f * extentsX / N;
		float deltaY = 2.0f * extentsY / N;
		float deltaTheta = 360.0f / N;
		// Run the long time loop
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				for (int k = 0; k < N; k++) {
					// Compute our current sampling point
					float x = i * deltaX - extentsX;
					float y = j * deltaY - extentsY;
					float theta = k * deltaTheta;
					_transform.position = new Vector3 (x, y, 0);
					Quaternion rotation = new Quaternion ();
					rotation.eulerAngles = new Vector3 (0, 0, theta);
					_transform.rotation = rotation;
					yield return new WaitForFixedUpdate ();
					// Check if there is an obstacle here
					obsSpace [i, j, k] = IsObstacleSpace ();
				}
			}
		}
		// Prune surrounded elements
		bool[,,] oldObsSpace = new bool[N, N, N];
		Array.Copy (obsSpace, oldObsSpace, N * N * N);
		int newObsCount = 0;
		for (int i = 1; i < N-1; i++) {
			for (int j = 1; j < N-1; j++) {
				for (int k = 1; k < N-1; k++) {
					if (oldObsSpace [i - 1, j, k] && oldObsSpace [i + 1, j, k]
					    && oldObsSpace [i, j - 1, k] && oldObsSpace [i, j + 1, k]
					    && oldObsSpace [i, j, k - 1] && oldObsSpace [i, j, k + 1]) {
						obsSpace [i, j, k] = false;
					}
				}
			}
		}
		// Create the objects
		for (int i = 1; i < N-1; i++) {
			for (int j = 1; j < N-1; j++) {
				for (int k = 1; k < N-1; k++) {
					if (obsSpace [i, j, k]) {
						// Create the object
						GameObject go = Instantiate (spaceObject, _configParent);
						Transform transform = go.GetComponent<Transform> ();
						if (transform)
							transform.position = new Vector3 (i, j, k);
					}
				}
			}
		}
		// Notify of completion
		Debug.Log ("Finished configuration space generation");
	}

	bool IsObstacleSpace ()
	{
		foreach (Collider2D coll in _obstacles) {
			if (_collider.IsTouching (coll)) {
				return true;
			}
		}
		return false;
	}
}
